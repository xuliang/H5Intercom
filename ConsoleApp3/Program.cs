using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace ConsoleApp3
{
    class Program
    {
        private int port;
        public Program(int port)
        {
            this.port = port;
        }
        static void Main(string[] args)
        {
            new Program(6666).run();
        }
        private async void run()
        {
            // 申明一个主回路调度组
            var dispatcher = new DispatcherEventLoopGroup();
            IEventLoopGroup bossGroup = new DispatcherEventLoopGroup();
            IEventLoopGroup workerGroup = new WorkerEventLoopGroup(dispatcher);
            try
            {
                ServerBootstrap b = new ServerBootstrap();
                b.Group(bossGroup, workerGroup)
                    .Channel<TcpServerChannel>()
                    // 设置网络IO参数等
                    .Option(ChannelOption.SoBacklog, 100) // (5)
                    // 在主线程组上设置一个打印日志的处理器
                    .Handler(new LoggingHandler("SRV-LSTN"))
                    .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;
                        pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                        //pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        pipeline.AddLast(new RtpMessageDecoder());
                        pipeline.AddLast(new RtpPacketHandler());
                    }))
                    .Option(ChannelOption.SoBacklog, 128)
                    .ChildOption(ChannelOption.SoKeepalive, true);

                IChannel boundChannel = await b.BindAsync(port); // (6)
                //ChannelFuture f = b.bind(port).sync();
                //f.channel().closeFuture().sync();
                Console.WriteLine("wait the client input");
                Console.ReadLine();

                // 关闭服务
                await boundChannel.CloseAsync();
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }
            finally
            {
                // 释放指定工作组线程
                await Task.WhenAll( // (7)
                bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1))
                );
            }
        }
    }
}
