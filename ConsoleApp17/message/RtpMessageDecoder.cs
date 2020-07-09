//package net.che_mi.message;

//import io.netty.buffer.ByteBuf;
//import io.netty.channel.ChannelHandlerContext;
//import io.netty.handler.codec.ByteToMessageDecoder;
//import net.che_mi.util.StringUtil;

//import java.util.List;
using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;

namespace ConsoleApp17
{
    /***
     * RTP 消息解码器
     *
     * @author 徐万利
     * @date 2018/7/16 0016 18:08
     */
    public class RtpMessageDecoder : ByteToMessageDecoder
    {

        //RTP 封包头部最大长度（可能某些字段没有，所以应该取最大的那个长度）
        private readonly int MIN_HEADER_LENGTH = 34;//30;//钟仁修改

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            //throw new System.NotImplementedException();
            
            if (input == null || input.ReadableBytes <= MIN_HEADER_LENGTH)  //最坏打算，至少30个字节时才能读到数据体长度
            {
                return;
            }
            
            input.MarkReaderIndex();
            //跳过无关紧要的数据
            input.SkipBytes(5);//in.skipBytes(1);//钟仁修改

            RtpMessage msg = new RtpMessage();
            //M（1 bit）、PT（7 bit） 共占用 1 个字节
            byte b = input.ReadByte();

            msg.setM((byte)((b >> 7) & 0x1));
            msg.setPT((byte)(b & 0x7f));

            msg.setSeq(input.ReadShort());
            byte[] simNum = new byte[6];
            input.ReadBytes(simNum);
            msg.setSimNum(StringUtil.ConvertByteToHexStringWithoutSpace(simNum));

            msg.setLogicChnnel(input.ReadByte());

            //数据类型（4 bit）、分包处理标记（4 bit）共占用一个字节
            b = input.ReadByte();

            msg.setDataType((byte)(b >> 4));
            msg.setFlag((byte)(b & 0x0f));

            if (msg.getDataType() != 4)
            {   //不为透传数据类型
                msg.setTimestamp(input.ReadLong());
            }

            if (msg.getDataType() != 3 && msg.getDataType() != 4)
            { //视频数据类型才有以下字段
                msg.setLIFI(input.ReadShort());
                msg.setLFI(input.ReadShort());
            }


            //数据体长度
            msg.setLength(input.ReadShort());

            if (input.ReadableBytes < msg.getLength()) {
                input.ResetReaderIndex();
                return;
            }
            //数据体
            byte[] body = new byte[msg.getLength()];
            input.ReadBytes(body);
            msg.setBody(body);
            output.Add(msg);
        }
    }

    }
