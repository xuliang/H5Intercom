//package net.che_mi.push;

//import org.bytedeco.javacpp.avcodec;
//import org.bytedeco.javacv.*;

//import java.io.ByteArrayOutputStream;
//import java.io.IOException;
//import java.io.PipedInputStream;
//import java.io.PipedOutputStream;
using System;
using System.Threading;

namespace ConsoleApp11
{
    public class PushTask {

    private volatile bool stop = false;
    private string name;
    //private FFmpegFrameGrabber grabber;
    //private FFmpegFrameRecorder recorder;
    //private PipedOutputStream pos;
    //private PipedInputStream pis;

        //private ByteArrayOutputStream bos = new ByteArrayOutputStream();
     private System.IO.MemoryStream bos = new System.IO.MemoryStream();

    public PushTask(string name){// throws IOException {
        this.name = name;
        //pos = new PipedOutputStream();
        //pis = new PipedInputStream(65536);
        //pis.connect(pos);
    }

    public void Run() {
        //try {

        //    grabber = new FFmpegFrameGrabber(pis);
        //    grabber.start();

        //    recorder = new FFmpegFrameRecorder("rtmp://127.0.0.1/live/" + name, 352,288,0);
        //    recorder.setInterleaved(true);
        //    recorder.setVideoCodec(avcodec.AV_CODEC_ID_H264); // 28
        //    recorder.setFormat("flv"); // rtmp的类型
        //    recorder.setFrameRate(25);
        //    recorder.setPixelFormat(0); // yuv420p

        //    recorder.start();
        //} catch (Exception e) {
        //   // e.printStackTrace();
        //}
        //while (!stop && !this.isInterrupted()) {
        //    try {
        //        Frame frame = grabber.grab();
        //        recorder.record(frame);
        //    } catch (FrameGrabber.Exception e) {
        //        e.printStackTrace();
        //    } catch (FrameRecorder.Exception e) {
        //        e.printStackTrace();
        //    }
        //}
    }

    public void Write(byte[] buff){// throws IOException {
        //bos.Write(buff);
    }

    public void Flush() {//throws IOException {
        //bos.flush();
        //pos.write(bos.toByteArray());
        //pos.flush();
        //bos.reset();
    }

    public void Shutdown(){// throws FrameGrabber.Exception, FrameRecorder.Exception {
        //grabber.stop();
        //recorder.stop();
    }
}
}