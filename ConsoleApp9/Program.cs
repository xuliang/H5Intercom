using System;
using FFmpeg.AutoGen;

namespace ConsoleApp9
{
    class Program
    {
        unsafe static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            AVOutputFormat* ofmt = null;
            //Input AVFormatContext and Output AVFormatContext
            AVFormatContext* ifmt_ctx_v = null;
            AVFormatContext* ifmt_ctx_a = null;
            AVFormatContext* ofmt_ctx = null;
            AVPacket pkt;
            int ret, i;
            int videoindex_v = -1, videoindex_out = -1;
            int audioindex_a = -1, audioindex_out = -1;
            int frame_index = 0;
            Int64 cur_pts_v = 0, cur_pts_a = 0;

            //const char *in_filename_v = "cuc_ieschool.ts";//Input file URL
            //const char *in_filename_v = "cuc_ieschool.h264";
            //string in_filename_v = @"D:\BaiduNetdiskDownload\friend.mp4";
            string in_filename_v = @"D:\BaiduNetdiskDownload\out.h264";

            //const char *in_filename_a = "cuc_ieschool.mp3";
            //const char *in_filename_a = "gowest.m4a";
            //const char *in_filename_a = "gowest.aac";
            //const char *in_filename_a = "huoyuanjia.mp3";
            string in_filename_a = @"D:\BaiduNetdiskDownload\hyj.mp3";

            string out_filename = @"D:\BaiduNetdiskDownload\xxxx.mp4";//Output file URL
            //string out_filename = @"rtmp://10.20.129.54:1935/123/111";///111

            //ffmpeg.av_register_all();
            //Input
            if ((ret = ffmpeg.avformat_open_input(&ifmt_ctx_v, in_filename_v, null, null)) < 0)
            {
                Console.WriteLine("Could not open input file.");
                //printf("Could not open input file.");
                goto end;
            }
            if ((ret = ffmpeg.avformat_find_stream_info(ifmt_ctx_v, null)) < 0)
            {
                Console.WriteLine("Failed to retrieve input stream information.");
                //printf("Failed to retrieve input stream information");
                goto end;
            }

            if ((ret = ffmpeg.avformat_open_input(&ifmt_ctx_a, in_filename_a, null, null)) < 0)
            {
                Console.WriteLine("Could not open input file.");
                //printf("Could not open input file.");
                goto end;
            }
            if ((ret = ffmpeg.avformat_find_stream_info(ifmt_ctx_a, null)) < 0)
            {
                Console.WriteLine("Failed to retrieve input stream information.");
                //printf("Failed to retrieve input stream information");
                goto end;
            }
            Console.WriteLine("===========Input Information==========");
            ffmpeg.av_dump_format(ifmt_ctx_v, 0, in_filename_v, 0);
            ffmpeg.av_dump_format(ifmt_ctx_a, 0, in_filename_a, 0);
            Console.WriteLine("=================================");
            //Output
            ffmpeg.avformat_alloc_output_context2(&ofmt_ctx, null, null, out_filename);
            //ffmpeg.avformat_alloc_output_context2(&ofmt_ctx, null, "flv", out_filename);
            if (ofmt_ctx==null)
            {
                Console.WriteLine("Could not create output context");
                ret = ffmpeg.AVERROR_UNKNOWN;
                goto end;
            }
            ofmt = ofmt_ctx->oformat;

            for (i = 0; i < ifmt_ctx_v->nb_streams; i++)
            {
                //Create output AVStream according to input AVStream
                if (ifmt_ctx_v->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    AVStream* in_stream = ifmt_ctx_v->streams[i];
                    AVStream* out_stream = ffmpeg.avformat_new_stream(ofmt_ctx, in_stream->codec->codec);
                    videoindex_v = i;
                    if (out_stream==null)
                    {
                        Console.WriteLine("Failed allocating output stream\n");
                        ret = ffmpeg.AVERROR_UNKNOWN;
                        goto end;
                    }
                    videoindex_out = out_stream->index;
                    //Copy the settings of AVCodecContext
                    if (ffmpeg.avcodec_parameters_copy(out_stream->codecpar, in_stream->codecpar) < 0)
                    {
                        Console.WriteLine("Failed to copy context from input to output stream codec context\n");
                        goto end;
                    }
                    out_stream->codec->codec_tag = 0;
                    if ((ofmt_ctx->oformat->flags & ffmpeg.AVFMT_GLOBALHEADER)!=0)
                        out_stream->codec->flags |= ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER;
                    break;
                }
            }

            for (i = 0; i < ifmt_ctx_a->nb_streams; i++)
            {
                //Create output AVStream according to input AVStream
                if (ifmt_ctx_a->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
                {
                    AVStream* in_stream = ifmt_ctx_a->streams[i];
                    AVStream* out_stream = ffmpeg.avformat_new_stream(ofmt_ctx, in_stream->codec->codec);
                    audioindex_a = i;
                    if (out_stream==null)
                    {
                        Console.WriteLine("Failed allocating output stream");
                        ret = ffmpeg.AVERROR_UNKNOWN;
                        goto end;
                    }
                    audioindex_out = out_stream->index;
                    //Copy the settings of AVCodecContext
                    if (ffmpeg.avcodec_parameters_copy(out_stream->codecpar, in_stream->codecpar) < 0)
                    {
                        Console.WriteLine("Failed to copy context from input to output stream codec context");
                        goto end;
                    }
                    out_stream->codec->codec_tag = 0;
                    if ((ofmt_ctx->oformat->flags & ffmpeg.AVFMT_GLOBALHEADER)!=0)
                        out_stream->codec->flags |= ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER;

                    break;
                }
            }

            Console.WriteLine("==========Output Information==========\n");
            ffmpeg.av_dump_format(ofmt_ctx, 0, out_filename, 1);
            Console.WriteLine("======================================\n");
            //Open output file
            if ((ofmt->flags & ffmpeg.AVFMT_NOFILE)==0)
            {
                if (ffmpeg.avio_open(&ofmt_ctx->pb, out_filename, ffmpeg.AVIO_FLAG_WRITE) < 0)
                {
                    Console.WriteLine("Could not open output file {0}", out_filename);
                    goto end;
                }
            }
            //Write file header
            if (ffmpeg.avformat_write_header(ofmt_ctx, null) > 0)
            {
                Console.WriteLine("Error occurred when opening output file");
                goto end;
            }


            //FIX
#if USE_H264BSF
	AVBitStreamFilterContext* h264bsfc =  av_bitstream_filter_init("h264_mp4toannexb"); 
#endif
#if USE_AACBSF
	AVBitStreamFilterContext* aacbsfc =  av_bitstream_filter_init("aac_adtstoasc"); 
#endif

            while (true)
            {
                AVFormatContext* ifmt_ctx;
                int stream_index = 0;
                AVStream* in_stream;
                AVStream* out_stream;

                //Get an AVPacket
                if (ffmpeg.av_compare_ts(cur_pts_v, ifmt_ctx_v->streams[videoindex_v]->time_base, cur_pts_a, ifmt_ctx_a->streams[audioindex_a]->time_base) <= 0)
                {
                    ifmt_ctx = ifmt_ctx_v;
                    stream_index = videoindex_out;

                    if (ffmpeg.av_read_frame(ifmt_ctx, &pkt) >= 0)
                    {
                        do
                        {
                            in_stream = ifmt_ctx->streams[pkt.stream_index];
                            out_stream = ofmt_ctx->streams[stream_index];

                            if (pkt.stream_index == videoindex_v)
                            {
                                //FIX：No PTS (Example: Raw H.264)
                                //Simple Write PTS
                                if (pkt.pts == ffmpeg.AV_NOPTS_VALUE)
                                {
                                    //Write PTS
                                    AVRational time_base1 = in_stream->time_base;
                                    //Duration between 2 frames (us)
                                    Int64 calc_duration = (Int64)((double)ffmpeg.AV_TIME_BASE / ffmpeg.av_q2d(in_stream->r_frame_rate));
                                    //Parameters
                                    pkt.pts = (Int64)((double)(frame_index * calc_duration) / (double)(ffmpeg.av_q2d(time_base1) * ffmpeg.AV_TIME_BASE));
                                    pkt.dts = pkt.pts;
                                    pkt.duration = (Int64)((double)calc_duration / (double)(ffmpeg.av_q2d(time_base1) * ffmpeg.AV_TIME_BASE));
                                    frame_index++;
                                }

                                cur_pts_v = pkt.pts;
                                break;
                            }
                        } while (ffmpeg.av_read_frame(ifmt_ctx, &pkt) >= 0);
                        //ffmpeg.av_read_frame(ifmt_ctx, &pkt);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    ifmt_ctx = ifmt_ctx_a;
                    stream_index = audioindex_out;
                    if (ffmpeg.av_read_frame(ifmt_ctx, &pkt) >= 0)
                    {
                        do
                        {
                            in_stream = ifmt_ctx->streams[pkt.stream_index];
                            out_stream = ofmt_ctx->streams[stream_index];

                            if (pkt.stream_index == audioindex_a)
                            {
                                //FIX：No PTS
                                //Simple Write PTS
                                if (pkt.pts == ffmpeg.AV_NOPTS_VALUE)
                                {
                                    //Write PTS
                                    AVRational time_base1 = in_stream->time_base;
                                    //Duration between 2 frames (us)
                                    Int64 calc_duration = (Int64)((double)ffmpeg.AV_TIME_BASE / ffmpeg.av_q2d(in_stream->r_frame_rate));
                                    //Parameters
                                    pkt.pts = (Int64)((double)(frame_index * calc_duration) / (double)(ffmpeg.av_q2d(time_base1) * ffmpeg.AV_TIME_BASE));
                                    pkt.dts = pkt.pts;
                                    pkt.duration = (Int64)((double)calc_duration / (double)(ffmpeg.av_q2d(time_base1) * ffmpeg.AV_TIME_BASE));
                                    frame_index++;
                                }
                                cur_pts_a = pkt.pts;

                                break;
                            }
                        } while (ffmpeg.av_read_frame(ifmt_ctx, &pkt) >= 0);
                        //ffmpeg.av_read_frame(ifmt_ctx, &pkt);
                    }
                    else
                    {
                        break;
                    }

                }

                //FIX:Bitstream Filter
#if USE_H264BSF
		av_bitstream_filter_filter(h264bsfc, in_stream->codec, NULL, &pkt.data, &pkt.size, pkt.data, pkt.size, 0);
#endif
#if USE_AACBSF
		av_bitstream_filter_filter(aacbsfc, out_stream->codec, NULL, &pkt.data, &pkt.size, pkt.data, pkt.size, 0);
#endif


                //Convert PTS/DTS
                pkt.pts = ffmpeg.av_rescale_q_rnd(pkt.pts, in_stream->time_base, out_stream->time_base, (AVRounding)(AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX));
                pkt.dts = ffmpeg.av_rescale_q_rnd(pkt.dts, in_stream->time_base, out_stream->time_base, (AVRounding)(AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX));
                pkt.duration = ffmpeg.av_rescale_q(pkt.duration, in_stream->time_base, out_stream->time_base);
                pkt.pos = -1;
                pkt.stream_index = stream_index;

                Console.WriteLine("Write 1 Packet. size:{0}\tpts:(1)\n", pkt.size, pkt.pts);
                //Write
                if (ffmpeg.av_interleaved_write_frame(ofmt_ctx, &pkt) < 0)
                {
                    Console.WriteLine("Error muxing packet\n");
                    break;
                }
                //ffmpeg.av_free_packet(&pkt);

            }
            //Write file trailer
            ffmpeg.av_write_trailer(ofmt_ctx);

#if USE_H264BSF
	av_bitstream_filter_close(h264bsfc);
#endif
#if USE_AACBSF
	av_bitstream_filter_close(aacbsfc);
#endif

        end:
            ffmpeg.avformat_close_input(&ifmt_ctx_v);
            ffmpeg.avformat_close_input(&ifmt_ctx_a);
            /* close output */
            if (ofmt_ctx!=null && (ofmt->flags & ffmpeg.AVFMT_NOFILE)<0)
                ffmpeg.avio_close(ofmt_ctx->pb);
            ffmpeg.avformat_free_context(ofmt_ctx);
            if (ret < 0 && ret != ffmpeg.AVERROR_EOF)
            {
                Console.WriteLine("Error occurred.");

            }
            Console.ReadLine();
        }
    }
}
