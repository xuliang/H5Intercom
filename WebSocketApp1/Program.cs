using System;
using System.IO;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DotNetty.Codecs.Http;
using DotNetty.Common;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Libuv;
using Examples.Common;

namespace WebSocketApp1
{
    class Program
    {
        static Program()
        {
            ResourceLeakDetector.Level = ResourceLeakDetector.DetectionLevel.Disabled;
        }
        static void Main() => RunServerAsync().Wait();

        static async Task RunServerAsync()
        {
            Console.WriteLine(
                $"\n{RuntimeInformation.OSArchitecture} {RuntimeInformation.OSDescription}"
                + $"\n{RuntimeInformation.ProcessArchitecture} {RuntimeInformation.FrameworkDescription}"
                + $"\nProcessor Count : {Environment.ProcessorCount}\n");

            bool useLibuv = ServerSettings.UseLibuv;
            Console.WriteLine("Transport type : " + (useLibuv ? "Libuv" : "Socket"));

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            }

            Console.WriteLine($"Server garbage collection : {(GCSettings.IsServerGC ? "Enabled" : "Disabled")}");
            Console.WriteLine($"Current latency mode for garbage collection: {GCSettings.LatencyMode}");
            Console.WriteLine("\n");

            /*
             Netty 提供了许多不同的 EventLoopGroup 的实现用来处理不同的传输。
             在这个例子中我们实现了一个服务端的应用，因此会有2个 NioEventLoopGroup 会被使用。
             第一个经常被叫做‘boss’，用来接收进来的连接。第二个经常被叫做‘worker’，用来处理已经被接收的连接，一旦‘boss’接收到连接，就会把连接信息注册到‘worker’上。
             如何知道多少个线程已经被使用，如何映射到已经创建的 Channel上都需要依赖于 IEventLoopGroup 的实现，并且可以通过构造函数来配置他们的关系。
             */

            // 主工作线程组，设置为1个线程
            // Boss线程：由这个线程池提供的线程是boss种类的，用于创建、连接、绑定socket， （有点像门卫）然后把这些socket传给worker线程池。
            // 在服务器端每个监听的socket都有一个boss线程来处理。在客户端，只有一个boss线程来处理所有的socket。
            IEventLoopGroup bossGroup;

            // 子工作线程组，----默认为内核数*2的线程数
            // Worker线程：Worker线程执行所有的异步I/O，即处理操作
            IEventLoopGroup workGroup;
            if (useLibuv)
            {
                var dispatcher = new DispatcherEventLoopGroup();
                bossGroup = dispatcher;
                workGroup = new WorkerEventLoopGroup(dispatcher);
            }
            else
            {
                bossGroup = new MultithreadEventLoopGroup(1);
                workGroup = new MultithreadEventLoopGroup();
            }

            X509Certificate2 tlsCertificate = null;
            if (ServerSettings.IsSsl)
            {
                tlsCertificate = new X509Certificate2(Path.Combine(ExampleHelper.ProcessDirectory, "dotnetty.com.pfx"), "password");
            }
            try
            {
                // 声明一个服务端Bootstrap，每个Netty服务端程序，都由ServerBootstrap控制，通过链式的方式组装需要的参数
                // ServerBootstrap 启动NIO服务的辅助启动类,负责初始话netty服务器，并且开始监听端口的socket请求
                var bootstrap = new ServerBootstrap();

                // 设置主和工作线程组
                bootstrap.Group(bossGroup, workGroup);

                if (useLibuv)
                {
                    // 申明服务端通信通道为TcpServerChannel
                    // 设置非阻塞,用它来建立新accept的连接,用于构造serversocketchannel的工厂类
                    bootstrap.Channel<TcpServerChannel>();
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        bootstrap
                            .Option(ChannelOption.SoReuseport, true)
                            .ChildOption(ChannelOption.SoReuseaddr, true);
                    }
                }
                else
                {
                    bootstrap.Channel<TcpServerSocketChannel>();
                }

                // ChildChannelHandler 对出入的数据进行的业务操作,其继承ChannelInitializer
                bootstrap
                    // 设置网络IO参数等
                    .Option(ChannelOption.SoBacklog, 8192)
                    /*
                    * ChannelInitializer 是一个特殊的处理类，他的目的是帮助使用者配置一个新的 Channel。
                    * 也许你想通过增加一些处理类比如DiscardServerHandler 来配置一个新的 Channel 或者其对应的ChannelPipeline 来实现你的网络程序。
                    * 当你的程序变的复杂时，可能你会增加更多的处理类到 pipline 上，然后提取这些匿名类到最顶层的类上。
                    */
                    // 设置工作线程参数
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                    /*
                    * 工作线程连接器是设置了一个管道，服务端主线程所有接收到的信息都会通过这个管道一层层往下传输，
                    * 同时所有出栈的消息 也要这个管道的所有处理器进行一步步处理。
                    */
                        IChannelPipeline pipeline = channel.Pipeline;
                        if (tlsCertificate != null)
                        {
                            pipeline.AddLast(TlsHandler.Server(tlsCertificate));
                        }
                        pipeline.AddLast(new HttpServerCodec());
                        pipeline.AddLast(new HttpObjectAggregator(65536));

                    //业务handler ，这里是实际处理业务的Handler
                    //pipeline.AddLast(new WebSocketServerHandler());
                    //自己写的业务类
                    pipeline.AddLast(new SendFunction());
                    }));

                // bootstrap绑定到指定端口的行为 就是服务端启动服务，同样的Serverbootstrap可以bind到多个端口
                int port = ServerSettings.Port;
                IChannel bootstrapChannel = await bootstrap.BindAsync(IPAddress.Loopback, port);
                // 似乎没有成功阻塞 而是连接服务端后 就马上执行下一句了 导致连接一次就关闭   （是成功进入 ChannelActive 判断的）也就是无法保持长连接
                // 添加长连接即可，参考EchoClient

                Console.WriteLine("Open your web browser and navigate to "
                    + $"{(ServerSettings.IsSsl ? "https" : "http")}"
                    + $"://127.0.0.1:{port}/");
                Console.WriteLine("Listening on "
                    + $"{(ServerSettings.IsSsl ? "wss" : "ws")}"
                    + $"://127.0.0.1:{port}/websocket");
                Console.ReadLine();

                // 关闭服务
                await bootstrapChannel.CloseAsync();
            }
            finally
            {
                // 释放工作组线程
                workGroup.ShutdownGracefullyAsync().Wait();
                bossGroup.ShutdownGracefullyAsync().Wait();
            }
        }

    }
}