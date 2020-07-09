// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace ConsoleApp12
{
    using System;
    using System.IO;
    using System.Text;
    using DotNetty.Buffers;
    using DotNetty.Transport.Channels;

    public class EchoClientHandler : ChannelHandlerAdapter
    {
        readonly IByteBuffer initialMessage;

        public EchoClientHandler()
        {
            this.initialMessage = Unpooled.Buffer(ClientSettings.Size);
            byte[] messageBytes = Encoding.UTF8.GetBytes("Hello world");
            //byte[] messageBytes = File.ReadAllBytes(ClientSettings.File);
            this.initialMessage.WriteBytes(messageBytes);
        }

        public override void ChannelActive(IChannelHandlerContext context) => context.WriteAndFlushAsync(this.initialMessage);

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var byteBuffer = message as IByteBuffer;
            if (byteBuffer != null)
            {
                Console.WriteLine("Received from server: " + byteBuffer.ToString(Encoding.UTF8));
            }
            context.WriteAsync(message);
        }

        public override void ChannelReadComplete(IChannelHandlerContext context) => context.Flush();

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine("Exception: " + exception);
            context.CloseAsync();
        }
    }
}