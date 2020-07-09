using System;
using System.IO;
using FFmpeg.AutoGen;

namespace ConsoleApp8
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            rtsp();
        }
        static void rtmp()
        {

        }
        static void rtsp()
        {
            //Console.WriteLine("Hello World!");
            //变量
            AVFormatContext* pFormatCtx;
            string filepath = @"rtmp://10.20.129.54:1935/123/111";
            AVPacket* packet;
            //初始化
            //ffmpeg.av_register_all();
            ffmpeg.avformat_network_init();
            pFormatCtx = ffmpeg.avformat_alloc_context();
            AVDictionary* options = null;
            ffmpeg.av_dict_set(&options, "buffer_size", "102400", 0); //设置缓存大小，1080p可将值调大
            ffmpeg.av_dict_set(&options, "rtsp_transport", "tcp", 0); //以udp方式打开，如果以tcp方式打开将udp替换为tcp
            ffmpeg.av_dict_set(&options, "stimeout", "2000000", 0); //设置超时断开连接时间，单位微秒
            ffmpeg.av_dict_set(&options, "max_delay", "500000", 0); //设置最大时延
            //packet = (AVPacket*)ffmpeg.av_malloc(sizeof(AVPacket));
            packet = ffmpeg.av_packet_alloc();

            //打开网络流或文件流  
            if (ffmpeg.avformat_open_input(&pFormatCtx, filepath, null, null) != 0)
            {
                Console.WriteLine("Couldn't open input stream.");
                //printf("Couldn't open input stream.\n");
                return;
            }
            //查找码流信息
            if (ffmpeg.avformat_find_stream_info(pFormatCtx, null) < 0)
            {
                Console.WriteLine("Couldn't find stream information.");
                //printf("Couldn't find stream information.\n");
                return;
            }
            //查找码流中是否有视频流
            int videoindex = -1;
            for (int i = 0; i < pFormatCtx->nb_streams; i++)
                if (pFormatCtx->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    videoindex = i;
                    break;
                }
            if (videoindex == -1)
            {
                Console.WriteLine("Didn't find a video stream.");
                //printf("Didn't find a video stream.\n");
                return;
            }
            //保存一段时间的视频流，写入文件中
            //FILE* fpSave;
            var fs=File.Open("geth264.h264", FileMode.Create);
            for (int i = 0; i < 1000; i++)
            {
                if (ffmpeg.av_read_frame(pFormatCtx, packet) >= 0)
                {
                    if (packet->stream_index == videoindex)
                    {
                        using (var packetStream = new UnmanagedMemoryStream(packet->data, packet->size)) packetStream.CopyTo(fs);
                        //fwrite(packet->data, 1, packet->size, fpSave);//写数据到文件中  
                    }
                    ffmpeg.av_packet_unref(packet);
                }
            }
            //释放内存
            ffmpeg.av_free(pFormatCtx);
            ffmpeg.av_free(packet);
        }
    }
}
