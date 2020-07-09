using System;
using System.Threading.Tasks;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Libuv;
using Microsoft.Extensions.Logging.Console;

namespace ConsoleApp17
{
    class Program
    {

            static void Main() => RunServerAsync().Wait();
            static async Task RunServerAsync()
            {
                InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));
                // 申明一个主回路调度组
                var dispatcher = new DispatcherEventLoopGroup();
                IEventLoopGroup bossGroup = dispatcher; 
                IEventLoopGroup workerGroup = new WorkerEventLoopGroup(dispatcher);
                try
                {
                    ServerBootstrap b = new ServerBootstrap();
                    b.Group(bossGroup, workerGroup)
                        .Channel<TcpServerChannel>().Option(ChannelOption.RcvbufAllocator, new AdaptiveRecvByteBufAllocator())
                        // 设置网络IO参数等
                        .Option(ChannelOption.SoBacklog, 128) // (5)
                                                              // 在主线程组上设置一个打印日志的处理器
                        .Handler(new LoggingHandler("SRV-LSTN"))
                        .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                        {
                            IChannelPipeline pipeline = channel.Pipeline;
                            pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                            pipeline.AddLast(new RtpMessageDecoder());
                            pipeline.AddLast(new RtpPacketHandler());
                        }))
                        .ChildOption(ChannelOption.SoKeepalive, true);

                    IChannel boundChannel = await b.BindAsync(8007); // (6)

                    Console.WriteLine("等待客户端连接...");
                    Console.ReadLine();

                    // 关闭服务
                    await boundChannel.CloseAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    //e.printStackTrace();
                }
                finally
                {
                    Console.WriteLine("finally");
                    // 释放指定工作组线程
                    await Task.WhenAll( // (7)
                    bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1))
                    );
                }
            }
        }
    }
