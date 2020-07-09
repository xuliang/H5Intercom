using System;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace ConsoleApp14
{
    unsafe class Program
    {
        static FileStream fileStream = new FileStream(@"D:\BaiduNetdiskDownload\xxxx.mp4", FileMode.Open, FileAccess.Read);
        ////定义存放文件信息的字节数组
        //byte[] bytes = new byte[fileStream.Length];
        ////读取文件信息
        //fileStream.Read(bytes, 0, bytes.Length);
        //    //将得到的字节型数组重写编码为字符型数组
        //    char[] c = Encoding.UTF8.GetChars(bytes);
        //Console.WriteLine("学生的学号为：");
        //    //输出学生的学号
        //    Console.WriteLine(c);
        //    //关闭流
        //    fileStream.Close();
        static int read_data(void* opaque, byte* buf, int buf_size)
        {
            try
            {
                byte[] b = new byte[buf_size];
                //InputStream is = inputStreams.get(opaque);
                int size = fileStream.Read(b, 0, buf_size);
                Console.WriteLine("-------------------"+size);
                if (size < 0)
                {
                    return 0;
                }
                else
                {
                    Marshal.Copy(b, 0, new IntPtr(buf), size);
                    //buf.put(b, 0, size);
                    return size;
                }
            }
            catch (Exception t)
            {
                Console.WriteLine("Error on InputStream.read(): " + t);
                return -1;
            }
        }
        static void Run()
        {
            AVCodec* pVideoCodec;
            AVCodec* pAudioCodec;
            AVCodecContext* pVideoCodecCtx = null;
            AVCodecContext* pAudioCodecCtx = null;
            AVIOContext* pb = null;
            AVInputFormat* piFmt = null;
            AVFormatContext* pFmt = null;

            byte* inbuffer = null;
            inbuffer = (byte*)ffmpeg.av_malloc(32768);//32768
            //step1:申请一个AVIOContext
            pb = ffmpeg.avio_alloc_context(inbuffer, 32786, 0, null, new avio_alloc_context_read_packet(read_data), null, null);
            if (pb == null)
            {
                Console.WriteLine("avio alloc failed!");
                return;
            }
            //step2:探测流格式
            if (ffmpeg.av_probe_input_buffer(pb, &piFmt, "", null, 0, 0) < 0)
            {
                Console.WriteLine("probe failed!");
                return;
            }
            else
            {
                Console.WriteLine("probe success!");
                //Console.WriteLine("format: {0}[{1}]", piFmt->name, piFmt->long_name);
            }
            pFmt = ffmpeg.avformat_alloc_context();
            pFmt->pb = pb; //step3:这一步很关键
                           //step4:打开流
            if (ffmpeg.avformat_open_input(&pFmt, "", piFmt, null) < 0)
            {
                Console.WriteLine("avformat open failed.");
                return ;
            }
            else
            {
                Console.WriteLine("open stream success!");
            }
            //以下就和文件处理一致了
            if (ffmpeg.avformat_find_stream_info(pFmt,null) < 0)
            {
                Console.WriteLine("could not fine stream.");
                return ;
            }
            ffmpeg.av_dump_format(pFmt, 0, "", 0);
            int videoindex = -1;
            int audioindex = -1;
            for (int i = 0; i < pFmt->nb_streams; i++)
            {
                if ((pFmt->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO) && (videoindex < 0))
                {
                    videoindex = i;
                }
                if ((pFmt->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO) && (audioindex < 0))
                {
                    audioindex = i;
                }
            }
            if (videoindex < 0 || audioindex < 0)
            {
                Console.WriteLine("videoindex={0}, audioindex={1}", videoindex, audioindex);
                return ;
            }
            AVStream* pVst;
            AVStream* pAst;
            pVst = pFmt->streams[videoindex];
            pAst = pFmt->streams[audioindex];
            pVideoCodecCtx = pVst->codec;
            pAudioCodecCtx = pAst->codec;
            AVDictionary* format_opts = null;
            ffmpeg.av_dict_set_int(&format_opts, "rtbufsize", 18432000, 0);

            pVideoCodec = ffmpeg.avcodec_find_decoder(pVideoCodecCtx->codec_id);
            if (pVideoCodec==null)
            {
                Console.WriteLine("could not find video decoder!");
                return ;
            }
            if (ffmpeg.avcodec_open2(pVideoCodecCtx, pVideoCodec, &format_opts) < 0)
            {
                Console.WriteLine("could not open video codec!");
                return ;
            }
            pAudioCodec = ffmpeg.avcodec_find_decoder(pAudioCodecCtx->codec_id);
            if (pAudioCodec==null)
            {
                Console.WriteLine("could not find audio decoder!");
                return ;
            }
            if (ffmpeg.avcodec_open2(pAudioCodecCtx, pAudioCodec,&format_opts) < 0)
            {
                Console.WriteLine("could not open audio codec!");
                return ;
            }
            //int got_picture;
            //uint8_t samples[AVCODEC_MAX_AUDIO_FRAME_SIZE * 3 / 2];
            AVFrame* pframe = ffmpeg.av_frame_alloc();// ffmpeg.avcodec_alloc_frame();
            AVPacket pkt;
            ffmpeg.av_init_packet(&pkt);
            while (true)
            {
                if (ffmpeg.av_read_frame(pFmt, &pkt) >= 0)
                {
                    ffmpeg.avcodec_send_packet(pVideoCodecCtx, &pkt);
                    ffmpeg.avcodec_receive_frame(pVideoCodecCtx, pframe);
                        Console.WriteLine("decode one video frame!");
                        /*
                        if (pkt.stream_index == videoindex)
                        {
                            //Console.WriteLine("pkt.size=%d,pkt.pts=%lld, pkt.data=0x%x.", pkt.size, pkt.pts, (unsignedint)pkt.data);
                            //ffmpeg.avcodec_decode_video2(pVideoCodecCtx, pframe, &got_picture, &pkt);

                            ffmpeg.avcodec_send_packet(pVideoCodecCtx, &pkt);
                            ffmpeg.avcodec_receive_frame(pVideoCodecCtx, pframe);
                            //if (got_picture)
                            {
                                Console.WriteLine("decode one video frame!");
                            }
                        }
                        else if (pkt.stream_index == audioindex)
                        {
                            //int frame_size = AVCODEC_MAX_AUDIO_FRAME_SIZE * 3 / 2;
                            if (ffmpeg.avcodec_decode_audio4(pAudioCodecCtx, pframe, null, &pkt) >= 0)
                            {
                                Console.WriteLine("decode one audio frame!");
                            }
                        }
                        */
                        ffmpeg.av_packet_unref(&pkt);
                }
            }
            //ffmpeg.av_free(buf);
            //av_free(pframe);
            //free_queue(&recvqueue);

        }
        static void Main(string[] args) => Run();
    }
}
