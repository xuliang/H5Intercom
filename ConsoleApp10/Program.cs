using System;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace ConsoleApp10
{
    unsafe class Program
    {
        //FILE* fp_open;
        //FILE* fp_write;

        ////Read File
        //static int read_buffer(void* opaque, byte* buf, int buf_size)
        //{
        //    if (!feof(fp_open))
        //    {
        //        int true_size = fread(buf, 1, buf_size, fp_open);
        //        return true_size;
        //    }
        //    else
        //    {
        //        return -1;
        //    }
        //}

        ////Write File
        //static int write_buffer(void* opaque, byte* buf, int buf_size)
        //{
        //    if (!feof(fp_write))
        //    {
        //        int true_size = fwrite(buf, 1, buf_size, fp_write);
        //        return true_size;
        //    }
        //    else
        //    {
        //        return -1;
        //    }
        //}
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Hello World!");
        //    int ret;
        //    AVFormatContext* ifmt_ctx = null;
        //    AVFormatContext* ofmt_ctx = null;
        //    AVPacket packet, enc_pkt;
        //    AVFrame* frame = null;

        //    AVMediaType type;
        //    int stream_index;
        //    int i = 0;
        //    int got_frame, enc_got_frame;

        //    AVStream* out_stream;
        //    AVStream* in_stream;
        //    AVCodecContext* dec_ctx;
        //    AVCodecContext* enc_ctx;
        //    AVCodec* encoder;

        //    fp_open = fopen("cuc60anniversary_start.ts", "rb"); //视频源文件 
        //    fp_write = fopen("cuc60anniversary_start.h264", "wb+"); //输出文件

        //    //av_register_all();
        //    ifmt_ctx = ffmpeg.avformat_alloc_context();
        //    ffmpeg.avformat_alloc_output_context2(&ofmt_ctx, null, "h264", null);

        //    byte* inbuffer = null;
        //    byte* outbuffer = null;
        //    inbuffer = (byte*)ffmpeg.av_malloc(32768);
        //    outbuffer = (byte*)ffmpeg.av_malloc(32768);
        //    AVIOContext* avio_in = null;
        //    AVIOContext* avio_out = null;
        //    /*open input file*/
        //    avio_in = ffmpeg.avio_alloc_context(inbuffer, 32768, 0, null, new avio_alloc_context_read_packet(read_buffer) , null, null);
        //    if (avio_in == null)
        //        goto end;
        //    /*open output file*/
        //    avio_out = ffmpeg.avio_alloc_context(outbuffer, 32768, 1, null, null, new avio_alloc_context_write_packet(write_buffer), null);
        //    if (avio_out == null)
        //        goto end;

        //    ifmt_ctx->pb = avio_in;
        //    ifmt_ctx->flags = ffmpeg.AVFMT_FLAG_CUSTOM_IO;
        //    if ((ret = ffmpeg.avformat_open_input(&ifmt_ctx, "whatever", null, null)) < 0)
        //    {
        //        ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot open input file\n");
        //        return ;
        //    }
        //    if ((ret = ffmpeg.avformat_find_stream_info(ifmt_ctx, null)) < 0)
        //    {
        //        ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Cannot find stream information\n");
        //        return ;
        //    }
        //    for (i = 0; i < ifmt_ctx->nb_streams; i++)
        //    {
        //        AVStream* stream;
        //        AVCodecContext* codec_ctx;
        //        stream = ifmt_ctx->streams[i];
        //        codec_ctx = stream->codec;
        //        /* Reencode video & audio and remux subtitles etc. */
        //        if (codec_ctx->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
        //        {
        //            /* Open decoder */
        //            ret = ffmpeg.avcodec_open2(codec_ctx,
        //                ffmpeg.avcodec_find_decoder(codec_ctx->codec_id), null);
        //            if (ret < 0)
        //            {
        //                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, string.Format("Failed to open decoder for stream {0}\n", i));
        //                return ;
        //            }
        //        }
        //    }
        //    //av_dump_format(ifmt_ctx, 0, "whatever", 0);


        //    //avio_out->write_packet=write_packet;
        //    ofmt_ctx->pb = avio_out;
        //    ofmt_ctx->flags = ffmpeg.AVFMT_FLAG_CUSTOM_IO;
        //    for (i = 0; i < 1; i++)
        //    {
        //        out_stream = ffmpeg.avformat_new_stream(ofmt_ctx, null);
        //        if (out_stream == null)
        //        {
        //            ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Failed allocating output stream\n");
        //            return;// ffmpeg.AVERROR_UNKNOWN;
        //        }
        //        in_stream = ifmt_ctx->streams[i];
        //        dec_ctx = in_stream->codec;
        //        enc_ctx = out_stream->codec;
        //        if (dec_ctx->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
        //        {
        //            encoder = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_H264);
        //            enc_ctx->height = dec_ctx->height;
        //            enc_ctx->width = dec_ctx->width;
        //            enc_ctx->sample_aspect_ratio = dec_ctx->sample_aspect_ratio;
        //            enc_ctx->pix_fmt = encoder->pix_fmts[0];
        //            enc_ctx->time_base = dec_ctx->time_base;
        //            //enc_ctx->time_base.num = 1;
        //            //enc_ctx->time_base.den = 25;
        //            //H264的必备选项，没有就会错
        //            enc_ctx->me_range = 16;
        //            enc_ctx->max_qdiff = 4;
        //            enc_ctx->qmin = 10;
        //            enc_ctx->qmax = 51;
        //            enc_ctx->qcompress = 0.6f;
        //            enc_ctx->refs = 3;
        //            enc_ctx->bit_rate = 500000;

        //            ret = ffmpeg.avcodec_open2(enc_ctx, encoder, null);
        //            if (ret < 0)
        //            {
        //                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, string.Format("Cannot open video encoder for stream {0}\n", i));
        //                return ;
        //            }
        //        }
        //        else if (dec_ctx->codec_type == AVMediaType.AVMEDIA_TYPE_UNKNOWN)
        //        {
        //            ffmpeg.av_log(null, ffmpeg.AV_LOG_FATAL, string.Format("Elementary stream {0} is of unknown type, cannot proceed\n", i));
        //            return;// ffmpeg.AVERROR_INVALIDDATA;
        //        }
        //        else
        //        {
        //            /* if this stream must be remuxed */
        //            ret = ffmpeg.avcodec_parameters_copy(ofmt_ctx->streams[i]->codecpar, ifmt_ctx->streams[i]->codecpar);
        //            if (ret < 0)
        //            {
        //                ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Copying stream context failed\n");
        //                return ;
        //            }
        //        }
        //        if ((ofmt_ctx->oformat->flags & ffmpeg.AVFMT_GLOBALHEADER) != 0)
        //            enc_ctx->flags |= ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER;
        //    }
        //    //av_dump_format(ofmt_ctx, 0, "whatever", 1);
        //    /* init muxer, write output file header */
        //    ret = ffmpeg.avformat_write_header(ofmt_ctx, null);
        //    if (ret < 0)
        //    {
        //        ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Error occurred when opening output file\n");
        //        return ;
        //    }

        //    i = 0;
        //    /* read all packets */
        //    while (true)
        //    {
        //        i++;
        //        if ((ret = ffmpeg.av_read_frame(ifmt_ctx, &packet)) < 0)
        //            break;
        //        stream_index = packet.stream_index;
        //        if (stream_index != 0)
        //            continue;
        //        type = ifmt_ctx->streams[packet.stream_index]->codec->codec_type;
        //        ffmpeg.av_log(null, ffmpeg.AV_LOG_DEBUG, string.Format("Demuxer gave frame of stream_index %u\n", stream_index));

        //        ffmpeg.av_log(null, ffmpeg.AV_LOG_DEBUG, "Going to reencode the frame\n");
        //        frame = ffmpeg.av_frame_alloc();
        //        if (frame==null)
        //        {
        //            ret = ffmpeg.AVERROR(ffmpeg.ENOMEM);
        //            break;
        //        }
        //        packet.dts = ffmpeg.av_rescale_q_rnd(packet.dts,
        //            ifmt_ctx->streams[stream_index]->time_base,
        //            ifmt_ctx->streams[stream_index]->codec->time_base,
        //            (AVRounding)(AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX));
        //        packet.pts = ffmpeg.av_rescale_q_rnd(packet.pts,
        //            ifmt_ctx->streams[stream_index]->time_base,
        //            ifmt_ctx->streams[stream_index]->codec->time_base,
        //            (AVRounding)(AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX));
        //        //ret = ffmpeg.avcodec_decode_video2(ifmt_ctx->streams[stream_index]->codec, frame, &got_frame, &packet);
        //        ret = ffmpeg.avcodec_send_packet(ifmt_ctx->streams[stream_index]->codec,  &packet);
        //        //ffmpeg.avcodec_receive_frame(ifmt_ctx->streams[stream_index]->codec,frame);

        //        Console.WriteLine("Decode 1 Packet\tsize:{0}\tpts:(1)\n", packet.size, packet.pts);

        //        if (ret < 0)
        //        {
        //            ffmpeg.av_frame_free(&frame);
        //            ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Decoding failed\n");
        //            break;
        //        }
        //        //if (got_frame>=0)
        //        //{
        //            frame->pts = frame->best_effort_timestamp;
        //            frame->pict_type = AVPictureType.AV_PICTURE_TYPE_NONE;

        //            enc_pkt.data = null;
        //            enc_pkt.size = 0;
        //            ffmpeg.av_init_packet(&enc_pkt);
        //            //ret = ffmpeg.avcodec_encode_video2(ofmt_ctx->streams[stream_index]->codec, &enc_pkt, frame, &enc_got_frame);
        //            ret = ffmpeg.avcodec_receive_frame(ofmt_ctx->streams[stream_index]->codec,  frame);

        //            Console.WriteLine("Encode 1 Packet\tsize:{0}\tpts:{1}\n", enc_pkt.size, enc_pkt.pts);

        //            ffmpeg.av_frame_free(&frame);
        //            if (ret < 0)
        //                goto end;
        //           // if (enc_got_frame<0)
        //              //  continue;
        //            /* prepare packet for muxing */
        //            enc_pkt.stream_index = stream_index;
        //            enc_pkt.dts = ffmpeg.av_rescale_q_rnd(enc_pkt.dts,
        //                ofmt_ctx->streams[stream_index]->codec->time_base,
        //                ofmt_ctx->streams[stream_index]->time_base,
        //                (AVRounding)(AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX));
        //            enc_pkt.pts = ffmpeg.av_rescale_q_rnd(enc_pkt.pts,
        //                ofmt_ctx->streams[stream_index]->codec->time_base,
        //                ofmt_ctx->streams[stream_index]->time_base,
        //                (AVRounding)(AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX));
        //            enc_pkt.duration = ffmpeg.av_rescale_q(enc_pkt.duration,
        //                ofmt_ctx->streams[stream_index]->codec->time_base,
        //                ofmt_ctx->streams[stream_index]->time_base);
        //            ffmpeg.av_log(null, ffmpeg.AV_LOG_INFO, string.Format("Muxing frame {0}\n", i));
        //            /* mux encoded frame */
        //            ffmpeg.av_write_frame(ofmt_ctx, &enc_pkt);
        //            if (ret < 0)
        //                goto end;
        //        //}
        //        //else
        //        //{
        //        //    ffmpeg.av_frame_free(&frame);
        //        //}

        //        //ffmpeg.av_free_packet(&packet);
        //    }

        //    /* flush encoders */
        //    for (i = 0; i < 1; i++)
        //    {
        //        /* flush encoder */
        //        ret = flush_encoder(ofmt_ctx, i);
        //        if (ret < 0)
        //        {
        //            ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Flushing encoder failed\n");
        //            goto end;
        //        }
        //    }
        //    ffmpeg.av_write_trailer(ofmt_ctx);
        //end:
        //    ffmpeg.av_freep(avio_in);
        //    ffmpeg.av_freep(avio_out);
        //    ffmpeg.av_free(inbuffer);
        //    ffmpeg.av_free(outbuffer);
        //    //ffmpeg.av_free_packet(&packet);
        //    ffmpeg.av_frame_free(&frame);
        //    ffmpeg.avformat_close_input(&ifmt_ctx);
        //    ffmpeg.avformat_free_context(ofmt_ctx);

        //    //ffmpeg.fclose(fp_open);
        //    //ffmpeg.fclose(fp_write);

        //    //if (ret < 0)
        //       // ffmpeg.av_log(null, ffmpeg.AV_LOG_ERROR, "Error occurred\n");
        //    //return (ret ? 1 : 0);

        //}

        //static int flush_encoder(AVFormatContext* fmt_ctx, int stream_index)
        //{
        //    int ret;
        //    int got_frame;
        //    AVPacket enc_pkt;
        //    if (!(fmt_ctx->streams[stream_index]->codec->codec->capabilities & ffmpeg.AV_CODEC_CAP_DELAY))
        //        return 0;
        //    while (true)
        //    {
        //        ffmpeg.av_log(null, ffmpeg.AV_LOG_INFO, string.Format("Flushing stream #{0} encoder\n", stream_index));
        //        //ret = encode_write_frame(NULL, stream_index, &got_frame);
        //        enc_pkt.data = null;
        //        enc_pkt.size = 0;
        //        ffmpeg.av_init_packet(&enc_pkt);
        //        ret = ffmpeg.avcodec_encode_video2(fmt_ctx->streams[stream_index]->codec, &enc_pkt, null, &got_frame);
        //        ffmpeg.av_frame_free(null);
        //        if (ret < 0)
        //            break;
        //        if (!got_frame)
        //        { ret = 0; break; }
        //        /* prepare packet for muxing */
        //        enc_pkt.stream_index = stream_index;
        //        enc_pkt.dts = ffmpeg.av_rescale_q_rnd(enc_pkt.dts,
        //                fmt_ctx->streams[stream_index]->codec->time_base,
        //                fmt_ctx->streams[stream_index]->time_base,
        //                (AVRounding)(AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX));
        //        enc_pkt.pts = ffmpeg.av_rescale_q_rnd(enc_pkt.pts,
        //                fmt_ctx->streams[stream_index]->codec->time_base,
        //                fmt_ctx->streams[stream_index]->time_base,
        //                (AVRounding)(AVRounding.AV_ROUND_NEAR_INF | AVRounding.AV_ROUND_PASS_MINMAX));
        //        enc_pkt.duration = ffmpeg.av_rescale_q(enc_pkt.duration,
        //                fmt_ctx->streams[stream_index]->codec->time_base,
        //                fmt_ctx->streams[stream_index]->time_base);
        //        ffmpeg.av_log(null, ffmpeg.AV_LOG_DEBUG, "Muxing frame\n");
        //        /* mux encoded frame */
        //        ret = ffmpeg.av_write_frame(fmt_ctx, &enc_pkt);
        //        if (ret < 0)
        //            break;
        //    }
        //    return ret;
        //}


    }
}
