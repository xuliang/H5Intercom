using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void OutputStream2InputStreamUsingPipe()
        {
            int iStreamSize = 128 * 1024;

            var t = new Thread((data) =>
            {
                using (var pipe = new AnonymousPipeClientStream(PipeDirection.Out,(string)data))
                {
                    for (var i = 0; i < iStreamSize; i++)
                        pipe.WriteByte((byte)'A');
                }
            });

            using (var ms = new MemoryStream())
            {
                using (var pipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
                {
                    t.Start(pipe.GetClientHandleAsString());

                    var buffer = new byte[8 * 1024];
                    int len;
                    while ((len = pipe.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        //Thread.Sleep(100);
                        ms.Write(buffer, 0, len);
                    }
                }
                t.Join();
                Assert.AreEqual(iStreamSize, ms.Length);
            }
        }

    }
}
