//package net.che_mi.handler;

//import io.netty.channel.ChannelHandlerContext;
//import io.netty.channel.SimpleChannelInboundHandler;
//import net.che_mi.message.RtpMessage;
//import net.che_mi.push.PushManager;
//import net.che_mi.push.PushTask;
//import org.slf4j.Logger;
//import org.slf4j.LoggerFactory;
using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
//using FFmpeg.AutoGen;

namespace ConsoleApp17
{
    /***
     * RTP 封包处理类
     *
     * @author 徐万利
     * @date 2018/7/16 0016 18:08
     */
    public class RtpPacketHandler : SimpleChannelInboundHandler<RtpMessage>
    {
        public override void ChannelRegistered(IChannelHandlerContext context)
        {
            base.ChannelRegistered(context);
        }

        public override void ChannelUnregistered(IChannelHandlerContext ctx)
        {
            //base.ChannelUnregistered(ctx);
            //RTMPTask task = PushManager.Get(ctx.Name);
            //if (task != null)
            //{
            //    try
            //    {
            //        //task.Shutdown();
            //    }
            //    catch (Exception e)
            //    {
            //    }
            //    PushManager.Remove(ctx.Name);
            //}
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            base.ExceptionCaught(context, exception);
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, RtpMessage rtpMessage)
        {
            //String taskName = rtpMessage.getSimNum() + "_" + rtpMessage.getLogicChnnel();
            //Tasker task = TaskManager.Get(ctx.Name);
            //if (task == null)
            //{
            //    task = TaskManager.NewTask(ctx.Name, taskName);
            //    task.Start();
            //}
            //task.Write(rtpMessage.getBody());
            //if (rtpMessage.getFlag() == 0 || rtpMessage.getFlag() == 2)
            //{
            //    task.Flush();
            //}
            //////////////////////////////////////////////////////////////////////////////////////////////////////
            //int iStreamSize = 128 * 1024;
            /*
            var t = new Thread((data) =>
            {
                using (var pipe = new AnonymousPipeClientStream(PipeDirection.Out, (string)data))
                {
                    //for (var i = 0; i < iStreamSize; i++)
                        pipe.Write(rtpMessage.getBody());//.WriteByte((byte)'A');
                }
            });

            using (var ms = new MemoryStream())
            {
                using (var pipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
                {
                    t.Start(pipe.GetClientHandleAsString());
                    var buffer = new byte[8 * 1024];
                    int len;
                    while ((len = pipe.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        //Thread.Sleep(100);
                        //ms.Write(buffer, 0, len);
                        FileStream fileStream = File.Open(@"D:\test.h264", FileMode.Append);//初始化文件流
                        //byte[] array = Encoding.Default.GetBytes("哈哈123abc");//给字节数组赋值
                        fileStream.Write(buffer, 0, len);//将字节数组写入文件流
                        fileStream.Close();//关闭流
                    }
                }
                t.Join();
            }
            */
        }
        /* 备份一下
        protected override void ChannelRead0(IChannelHandlerContext ctx, RtpMessage rtpMessage)
        {
            
            //throw new System.NotImplementedException();
            //if (rtpMessage.getDataType() != 3 && rtpMessage.getDataType() != 4)
            //{
            String taskName = rtpMessage.getSimNum() + "_" + rtpMessage.getLogicChnnel();
            //Console.WriteLine(rtpMessage.ToString());

            Tasker task = TaskManager.Get(ctx.Name);
            if (task == null)
            {
                task = TaskManager.NewTask(ctx.Name, taskName);
                task.Start();
            }
            task.Write(rtpMessage.getBody());
            if (rtpMessage.getFlag() == 0 || rtpMessage.getFlag() == 2)
            {
                task.Flush();
            }
            */
        //FileStream fileStream = File.Open(@"D:\test.h264", FileMode.Append);//初始化文件流
        ////byte[] array = Encoding.Default.GetBytes("哈哈123abc");//给字节数组赋值
        //fileStream.Write(rtpMessage.getBody(), 0, rtpMessage.getBody().Length);//将字节数组写入文件流
        //fileStream.Close();//关闭流
    }
        //private Logger logger = LoggerFactory.getLogger(RtpPacketHandler.class);

        /*
            @Override
            public void exceptionCaught(ChannelHandlerContext ctx, Throwable cause) throws Exception {
                super.exceptionCaught(ctx, cause);
                cause.printStackTrace();
            }

            @Override
            public void channelRegistered(ChannelHandlerContext ctx) throws Exception {
                super.channelRegistered(ctx);
                logger.debug(ctx.channel().remoteAddress().toString());
            }

            @Override
            public void channelUnregistered(ChannelHandlerContext ctx) throws Exception {
                super.channelUnregistered(ctx);
                PushTask task = PushManager.get(ctx.name());
                if (task != null) {
                    try {
                        task.shutdown();
                    } catch (Exception e) {
                    }
                    PushManager.remove(ctx.name());
                }
            }

            @Override
            protected void channelRead0(ChannelHandlerContext ctx, RtpMessage rtpMessage) throws Exception {
                //logger.debug(rtpMessage.toString());
                System.out.println(rtpMessage);

                if (rtpMessage.getDataType() != 3 && rtpMessage.getDataType() != 4) {
                    String taskName = rtpMessage.getSimNum() + "_" + rtpMessage.getLogicChnnel();
                    PushTask task = PushManager.get(ctx.name());
                    if (task == null) {
                        task = PushManager.newPublishTask(ctx.name(), taskName);
                        task.start();
                    }
                    task.write(rtpMessage.getBody());
                    if (rtpMessage.getFlag() == 0 || rtpMessage.getFlag() == 2) {
                        task.flush();
                    }
                }
            }
            */
    }
