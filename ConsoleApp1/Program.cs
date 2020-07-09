using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Process p = new Process();
            string command = @"D:\Tools\ffmpeg-20190312-d227ed5-win64-static\bin\ffmpeg.exe";

            ExecuteCommand(p, command, out string output, out string error);
            Console.Write(output);
            Console.Write(error);
            Console.ReadLine();
        }
        private static void ExecuteCommand(Process pc, string command,out string output, out string error)
        {
            try
            {
                //创建进程
                pc.StartInfo.FileName = command;
                pc.StartInfo.UseShellExecute = false;
                pc.StartInfo.RedirectStandardOutput = true;
                pc.StartInfo.RedirectStandardError = true;
                pc.StartInfo.CreateNoWindow = false;
                //pc.StartInfo.Arguments = @" -re -i rtmp://10.20.129.54:1935/123/222 -c copy -f flv D:\temp\time.mp4";
                pc.StartInfo.Arguments = @" -re -i D:\BaiduNetdiskDownload\friend.mp4 -c copy -f flv rtmp://10.20.129.54:1935/123/111";
                //pc.StartInfo.Arguments = @" -re -i D:\BaiduNetdiskDownload\4K_2160p.webm -c copy -f flv rtmp://10.20.129.54:1935/123/111";
                //启动进程
                pc.Start();

                //准备读出输出流及错误流
                string outputData = string.Empty;
                string errorData = string.Empty;
                pc.BeginOutputReadLine();
                pc.BeginErrorReadLine();
                
                pc.OutputDataReceived += (ss, ee) =>
                {
                    outputData += ee.Data;
                };

                pc.ErrorDataReceived += (ss, ee) =>
                {
                    errorData += ee.Data;
                };

                //等待执行结束后退出
                pc.WaitForExit();

                //关闭进程
                pc.Close();

                //返回结果
                output = outputData;
                error = errorData;
            }
            catch (Exception e)
            {
                output = null;
                error = e.Message;
            }
        }
    }
}