//package net.che_mi.handler;

//import io.netty.channel.ChannelHandlerContext;
//import io.netty.channel.SimpleChannelInboundHandler;
//import net.che_mi.message.RtpMessage;
//import net.che_mi.push.PushManager;
//import net.che_mi.push.PushTask;
//import org.slf4j.Logger;
//import org.slf4j.LoggerFactory;
using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace ConsoleApp3
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
            base.ChannelUnregistered(ctx);
            PushTask task = PushManager.Get(ctx.Name);
            if (task != null)
            {
                try
                {
                    task.Shutdown();
                }
                catch (Exception e)
                {
                }
                PushManager.Remove(ctx.Name);
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            base.ExceptionCaught(context, exception);
        }
        protected override void ChannelRead0(IChannelHandlerContext ctx, RtpMessage rtpMessage)
        {
            //throw new System.NotImplementedException();
            //System.out.println(rtpMessage);

            if (rtpMessage.getDataType() != 3 && rtpMessage.getDataType() != 4)
            {
                String taskName = rtpMessage.getSimNum() + "_" + rtpMessage.getLogicChnnel();
                /*
                PushTask task = new PushTask(ctx.Name);
               Thread t = new Thread(new ThreadStart(task.Run));
                t.Name = taskName;
                t.Start();
                task.Write(rtpMessage.getBody());
                if (rtpMessage.getFlag() == 0 || rtpMessage.getFlag() == 2)
                {
                    task.Flush();
                }
                */
                /*
                PushTask task = PushManager.Get(ctx.Name);

                if (task == null)
                {
                    Thread t = new Thread(new ThreadStart(task.Run));
                    task = PushManager.NewPublishTask(ctx.Name, taskName);
                    task.Start();
                }
                task.Write(rtpMessage.getBody());
                if (rtpMessage.getFlag() == 0 || rtpMessage.getFlag() == 2)
                {
                    task.Flush();
                }
                */
            }
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
}