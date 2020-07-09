using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Groups;
using Examples.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static DotNetty.Codecs.Http.HttpResponseStatus;
using static DotNetty.Codecs.Http.HttpVersion;

namespace WebSocketApp1
{
    public class SendFunction : SimpleChannelInboundHandler<object>
    {
        const string WebsocketPath = "/websocket";

        WebSocketServerHandshaker handshaker;

        static volatile IChannelGroup groups;

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        //客户端连接异常
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            WebSocketClose(context);
            Console.WriteLine(" SendFunction Exception: " + exception);
            context.CloseAsync();
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
        {
            if (msg is IFullHttpRequest request)
            {
                this.HandleHttpRequest(ctx, request);
            }
            else if (msg is WebSocketFrame frame)
            {
                this.HandleWebSocketFrame(ctx, frame);
            }
        }

        void HandleHttpRequest(IChannelHandlerContext ctx, IFullHttpRequest req)
        {
            if (!req.Result.IsSuccess)
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, BadRequest));
                return;
            }

            if (!Equals(req.Method, HttpMethod.Get))
            {
                SendHttpResponse(ctx, req, new DefaultFullHttpResponse(Http11, Forbidden));
                return;
            }

            var wsFactory = new WebSocketServerHandshakerFactory(GetWebSocketLocation(req), null, true, 5 * 1024 * 1024);
            this.handshaker = wsFactory.NewHandshaker(req);
            if (this.handshaker == null)
            {
                WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
            }
            else
            {
                this.handshaker.HandshakeAsync(ctx.Channel, req);
            }

            base.HandlerAdded(ctx);
            IChannelGroup g = groups;
            if (g == null)
            {
                lock (this)
                {
                    if (groups == null)
                    {
                        g = groups = new DefaultChannelGroup(ctx.Executor);
                    }
                }
            }
            g.Add(ctx.Channel);

            //主动向当前连接的客户端发送信息
            TextWebSocketFrame tst = new TextWebSocketFrame($"欢迎：{ctx.Channel.RemoteAddress}-{ctx.Channel.Id}加入.");
            groups.WriteAndFlushAsync(tst);

            //保存连接对象
            lock (ConnChannelList)
            {
                if (ConnChannelList.Count > 0)
                {
                    if (ConnChannelList.ContainsKey(ctx.Channel.Id.ToString()))
                    {
                        ConnChannelList.Remove(ctx.Channel.Id.ToString());
                    }
                }
                ConnChannelList.Add(ctx.Channel.Id.ToString(), ctx.Channel.Id);
                Console.WriteLine($"当前在线数：{ConnChannelList.Count}");
            }

            Console.WriteLine("---------首次到达----------");
            Console.WriteLine($"连接成功,欢迎：{ctx.Channel.RemoteAddress}加入");
        }

        public static volatile Dictionary<string, IChannelId> ConnChannelList = new Dictionary<string, IChannelId>();
        void HandleWebSocketFrame(IChannelHandlerContext ctx, WebSocketFrame frame)
        {
            //客户端关闭连接
            if (frame is CloseWebSocketFrame)
            {
                WebSocketClose(ctx);

                Console.WriteLine($"连接关闭：{ctx.Channel.RemoteAddress}");
                this.handshaker.CloseAsync(ctx.Channel, (CloseWebSocketFrame)frame.Retain());

                return;
            }

            if (frame is PingWebSocketFrame)
            {
                ctx.WriteAsync(new PongWebSocketFrame((IByteBuffer)frame.Content.Retain()));
                return;
            }

            if (frame is TextWebSocketFrame textFrame)
            {
                Console.WriteLine("---------消息到达----------");
                Console.WriteLine("Received from client string：" + frame.Content.ToString(Encoding.UTF8));

                ctx.WriteAsync(frame.Retain());

                return;
            }

            if (frame is BinaryWebSocketFrame)
            {
                Console.WriteLine("Received from client binary data：" + frame.Content);

                ctx.WriteAsync(frame.Retain());
            }
        }

        static void SendHttpResponse(IChannelHandlerContext ctx, IFullHttpRequest req, IFullHttpResponse res)
        {
            if (res.Status.Code != 200)
            {
                IByteBuffer buf = Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes(res.Status.ToString()));
                res.Content.WriteBytes(buf);
                buf.Release();
                HttpUtil.SetContentLength(res, res.Content.ReadableBytes);
            }

            Task task = ctx.Channel.WriteAndFlushAsync(res);
            if (!HttpUtil.IsKeepAlive(req) || res.Status.Code != 200)
            {
                task.ContinueWith((t, c) => ((IChannelHandlerContext)c).CloseAsync(),
                    ctx, TaskContinuationOptions.ExecuteSynchronously);
            }
        }

        static string GetWebSocketLocation(IFullHttpRequest req)
        {
            bool result = req.Headers.TryGet(HttpHeaderNames.Host, out ICharSequence value);
            Debug.Assert(result, "Host header does not exist.");
            string location = value.ToString() + WebsocketPath;

            if (ServerSettings.IsSsl)
            {
                return "wss://" + location;
            }
            else
            {
                return "ws://" + location;
            }
        }


        /// <summary>
        /// 关闭ws连接
        /// </summary>
        /// <param name="ctx"></param>
        static void WebSocketClose(IChannelHandlerContext ctx)
        {
            lock (ConnChannelList)
            {
                string channelId = ctx.Channel.Id.ToString();
                if (ConnChannelList.ContainsKey(channelId))
                {
                    ConnChannelList.Remove(channelId);
                }
                Console.WriteLine($"当前在线数：{ConnChannelList.Count}");
            }
        }

    }
}
