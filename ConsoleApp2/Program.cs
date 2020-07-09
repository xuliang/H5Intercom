using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static readonly string FFmpegPath = @"D:\Tools\ffmpeg-20190312-d227ed5-win64-static\bin\ffmpeg.exe";
        static void Main(string[] args)
        {
            string videoUrl = @"C:\Users\xuliang\Desktop\website-video_v2.mp4";
            string targetUrl = @"D:\Temp\newFile.mp4";

            //视频转码
            string para = string.Format("-i {0} -b 1024k -acodec copy -f mp4 {1}", videoUrl, targetUrl);
            RunMyProcess(para);

            Console.WriteLine("完成！");
            Console.ReadKey();

        }

        static void RunMyProcess(string Parameters)
        {
            var p = new Process();
            p.StartInfo.FileName = FFmpegPath;
            p.StartInfo.Arguments = Parameters;
            //是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            //不显示程序窗口
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            Console.WriteLine("\n开始转码...\n");
            p.WaitForExit();
            p.Close();
        }

    }
}