using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace ConsoleApp11
{
    class PipedOutputStream : Stream
    {

        /* REMIND: identification of the read and write sides needs to be
           more sophisticated.  Either using thread groups (but what about
           pipes within a thread?) or using finalization (but it may be a
           long time until the next GC). */
        private PipedInputStream sink;

        /**
         * Creates a piped output stream connected to the specified piped
         * input stream. Data bytes written to this stream will then be
         * available as input from <code>snk</code>.
         *
         * @param      snk   The piped input stream to connect to.
         * @exception  IOException  if an I/O error occurs.
         */
        public PipedOutputStream(PipedInputStream snk)
        {
            Connect(snk);
        }

        /**
         * Creates a piped output stream that is not yet connected to a
         * piped input stream. It must be connected to a piped input stream,
         * either by the receiver or the sender, before being used.
         *
         * @see     java.io.PipedInputStream#connect(java.io.PipedOutputStream)
         * @see     java.io.PipedOutputStream#connect(java.io.PipedInputStream)
         */
        public PipedOutputStream()
        {
        }

        /**
         * Connects this piped output stream to a receiver. If this object
         * is already connected to some other piped input stream, an
         * <code>IOException</code> is thrown.
         * <p>
         * If <code>snk</code> is an unconnected piped input stream and
         * <code>src</code> is an unconnected piped output stream, they may
         * be connected by either the call:
         * <blockquote><pre>
         * src.connect(snk)</pre></blockquote>
         * or the call:
         * <blockquote><pre>
         * snk.connect(src)</pre></blockquote>
         * The two calls have the same effect.
         *
         * @param      snk   the piped input stream to connect to.
         * @exception  IOException  if an I/O error occurs.
         */
        public async void Connect(PipedInputStream snk)
        {
            if (snk == null)
            {
                //throw new NullPointerException();
            }
            else if (sink != null || snk.Connected)
            {
                throw new IOException("Already connected");
            }
            sink = snk;
            snk.inn = -1;
            snk.outt = 0;
            snk.Connected = true;
        }

        /**
         * Writes the specified <code>byte</code> to the piped output stream.
         * <p>
         * Implements the <code>write</code> method of <code>OutputStream</code>.
         *
         * @param      b   the <code>byte</code> to be written.
         * @exception IOException if the pipe is <a href=#BROKEN> broken</a>,
         *          {@link #connect(java.io.PipedInputStream) unconnected},
         *          closed, or if an I/O error occurs.
         */
        public  void Write(int b)
        {
            if (sink == null)
            {
                throw new IOException("Pipe not connected");
            }
            sink.Receive(b);
        }

        /**
         * Writes <code>len</code> bytes from the specified byte array
         * starting at offset <code>off</code> to this piped output stream.
         * This method blocks until all the bytes are written to the output
         * stream.
         *
         * @param      b     the data.
         * @param      off   the start offset in the data.
         * @param      len   the number of bytes to write.
         * @exception IOException if the pipe is <a href=#BROKEN> broken</a>,
         *          {@link #connect(java.io.PipedInputStream) unconnected},
         *          closed, or if an I/O error occurs.
         */
        public override void Write(byte[] b, int off, int len)
        {
            if (sink == null)
            {
                throw new IOException("Pipe not connected");
            }
            else if (b == null)
            {
                //throw new NullPointerException();
            }
            else if ((off < 0) || (off > b.Length) || (len < 0) || ((off + len) > b.Length) || ((off + len) < 0))
            {
                //throw new IndexOutOfBoundsException();
            }
            else if (len == 0)
            {
                return;
            }
            sink.Receive(b, off, len);
        }

        /**
         * Flushes this output stream and forces any buffered output bytes
         * to be written out.
         * This will notify any readers that bytes are waiting in the pipe.
         *
         * @exception IOException if an I/O error occurs.
         */
        public override async void Flush()
        {
            if (sink != null)
            {
                lock (sink)
                {
                    sink.notifyAll();
                }
            }
        }

        /**
         * Closes this piped output stream and releases any system resources
         * associated with this stream. This stream may no longer be used for
         * writing bytes.
         *
         * @exception  IOException  if an I/O error occurs.
         */
        public override void Close()
        {
            if (sink != null)
            {
                sink.ReceivedLast();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    }
}