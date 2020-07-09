using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace ConsoleApp11
{
    class PipedInputStream : Stream
    {
        bool closedByWriter = false;
        volatile bool closedByReader = false;
        public bool Connected = false;

        /* REMIND: identification of the read and write sides needs to be
           more sophisticated.  Either using thread groups (but what about
           pipes within a thread?) or using finalization (but it may be a
           long time until the next GC). */
        Thread readSide;
        Thread writeSide;

        private static readonly int DEFAULT_PIPE_SIZE = 1024;

        /**
         * The default size of the pipe's circular input buffer.
         * @since   JDK1.1
         */
        // This used to be a constant before the pipe size was allowed
        // to change. This field will continue to be maintained
        // for backward compatibility.
        protected static readonly int PIPE_SIZE = DEFAULT_PIPE_SIZE;

        /**
         * The circular buffer into which incoming data is placed.
         * @since   JDK1.1
         */
        protected byte[] buffer;

        /**
         * The index of the position in the circular buffer at which the
         * next byte of data will be stored when received from the connected
         * piped output stream. <code>in&lt;0</code> implies the buffer is empty,
         * <code>in==out</code> implies the buffer is full
         * @since   JDK1.1
         */
        public int inn = -1;

        /**
         * The index of the position in the circular buffer at which the next
         * byte of data will be read by this piped input stream.
         * @since   JDK1.1
         */
        public int outt = 0;

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /**
         * Creates a <code>PipedInputStream</code> so
         * that it is connected to the piped output
         * stream <code>src</code>. Data bytes written
         * to <code>src</code> will then be  available
         * as input from this stream.
         *
         * @param      src   the stream to connect to.
         * @exception  IOException  if an I/O error occurs.
         */
        public PipedInputStream(PipedOutputStream src) : this(src, DEFAULT_PIPE_SIZE)
        {
        }

        /**
         * Creates a <code>PipedInputStream</code> so that it is
         * connected to the piped output stream
         * <code>src</code> and uses the specified pipe size for
         * the pipe's buffer.
         * Data bytes written to <code>src</code> will then
         * be available as input from this stream.
         *
         * @param      src   the stream to connect to.
         * @param      pipeSize the size of the pipe's buffer.
         * @exception  IOException  if an I/O error occurs.
         * @exception  IllegalArgumentException if {@code pipeSize <= 0}.
         * @since      1.6
         */
        public PipedInputStream(PipedOutputStream src, int pipeSize)
        {
            initPipe(pipeSize);
            connect(src);
        }

        /**
         * Creates a <code>PipedInputStream</code> so
         * that it is not yet {@linkplain #connect(java.io.PipedOutputStream)
         * connected}.
         * It must be {@linkplain java.io.PipedOutputStream#connect(
         * java.io.PipedInputStream) connected} to a
         * <code>PipedOutputStream</code> before being used.
         */
        public PipedInputStream()
        {
            initPipe(DEFAULT_PIPE_SIZE);
        }

        /**
         * Creates a <code>PipedInputStream</code> so that it is not yet
         * {@linkplain #connect(java.io.PipedOutputStream) connected} and
         * uses the specified pipe size for the pipe's buffer.
         * It must be {@linkplain java.io.PipedOutputStream#connect(
         * java.io.PipedInputStream)
         * connected} to a <code>PipedOutputStream</code> before being used.
         *
         * @param      pipeSize the size of the pipe's buffer.
         * @exception  IllegalArgumentException if {@code pipeSize <= 0}.
         * @since      1.6
         */
        public PipedInputStream(int pipeSize)
        {
            initPipe(pipeSize);
        }

        private void initPipe(int pipeSize)
        {
            if (pipeSize <= 0)
            {
                //throw new IllegalArgumentException("Pipe Size <= 0");
            }
            buffer = new byte[pipeSize];
        }

        /**
         * Causes this piped input stream to be connected
         * to the piped  output stream <code>src</code>.
         * If this object is already connected to some
         * other piped output  stream, an <code>IOException</code>
         * is thrown.
         * <p>
         * If <code>src</code> is an
         * unconnected piped output stream and <code>snk</code>
         * is an unconnected piped input stream, they
         * may be connected by either the call:
         *
         * <pre><code>snk.connect(src)</code> </pre>
         * <p>
         * or the call:
         *
         * <pre><code>src.connect(snk)</code> </pre>
         * <p>
         * The two calls have the same effect.
         *
         * @param      src   The piped output stream to connect to.
         * @exception  IOException  if an I/O error occurs.
         */
        public void Connect(PipedOutputStream src)
        {
            src.Connect(this);
        }

        /**
         * Receives a byte of data.  This method will block if no input is
         * available.
         * @param b the byte being received
         * @exception IOException If the pipe is <a href="#BROKEN"> <code>broken</code></a>,
         *          {@link #connect(java.io.PipedOutputStream) unconnected},
         *          closed, or if an I/O error occurs.
         * @since     JDK1.1
         */
        public async void Receive(int b)
        {
            CheckStateForReceive();
            writeSide = Thread.CurrentThread;
            if (inn == outt)
                awaitSpace();
            if (inn < 0)
            {
                inn = 0;
                outt = 0;
            }
            buffer[inn++] = (byte)(b & 0xFF);
            if (inn >= buffer.Length)
            {
                inn = 0;
            }
        }

        /**
         * Receives data into an array of bytes.  This method will
         * block until some input is available.
         * @param b the buffer into which the data is received
         * @param off the start offset of the data
         * @param len the maximum number of bytes received
         * @exception IOException If the pipe is <a href="#BROKEN"> broken</a>,
         *           {@link #connect(java.io.PipedOutputStream) unconnected},
         *           closed,or if an I/O error occurs.
         */
        public async void Receive(byte[] b, int off, int len)
        {
            CheckStateForReceive();
            writeSide = Thread.CurrentThread;
            int bytesToTransfer = len;
            while (bytesToTransfer > 0)
            {
                if (inn == outt)
                    awaitSpace();
                int nextTransferAmount = 0;
                if (outt < inn)
                {
                    nextTransferAmount = buffer.Length - inn;
                }
                else if (inn < outt)
                {
                    if (inn == -1)
                    {
                        inn = outt = 0;
                        nextTransferAmount = buffer.Length - inn;
                    }
                    else
                    {
                        nextTransferAmount = outt - inn;
                    }
                }
                if (nextTransferAmount > bytesToTransfer)
                    nextTransferAmount = bytesToTransfer;
                assert(nextTransferAmount > 0);
                System.arraycopy(b, off, buffer, inn, nextTransferAmount);
                bytesToTransfer -= nextTransferAmount;
                off += nextTransferAmount;
                inn += nextTransferAmount;
                if (inn >= buffer.Length)
                {
                    inn = 0;
                }
            }
        }

        private void CheckStateForReceive()
        {
            if (!connected)
            {
                throw new IOException("Pipe not connected");
            }
            else if (closedByWriter || closedByReader)
            {
                throw new IOException("Pipe closed");
            }
            else if (readSide != null && !readSide.IsAlive)
            {
                throw new IOException("Read end dead");
            }
        }

        private void AwaitSpace()
        {
            while (inn == outt)
            {
                CheckStateForReceive();

                /* full: kick any waiting readers */
                notifyAll();
                try
                {
                    wait(1000);
                }
                catch (InterruptedException ex)
                {
                    throw new java.io.InterruptedIOException();
                }
            }
        }

        /**
         * Notifies all waiting threads that the last byte of data has been
         * received.
         */
        public async void ReceivedLast()
        {
            closedByWriter = true;
            notifyAll();
        }

        /**
         * Reads the next byte of data from this piped input stream. The
         * value byte is returned as an <code>int</code> in the range
         * <code>0</code> to <code>255</code>.
         * This method blocks until input data is available, the end of the
         * stream is detected, or an exception is thrown.
         *
         * @return     the next byte of data, or <code>-1</code> if the end of the
         *             stream is reached.
         * @exception  IOException  if the pipe is
         *           {@link #connect(java.io.PipedOutputStream) unconnected},
         *           <a href="#BROKEN"> <code>broken</code></a>, closed,
         *           or if an I/O error occurs.
         */
        public async int Read()
        {
            if (!Connected)
            {
                throw new IOException("Pipe not connected");
            }
            else if (closedByReader)
            {
                throw new IOException("Pipe closed");
            }
            else if (writeSide != null && !writeSide.IsAlive && !closedByWriter && (inn < 0))
            {
                throw new IOException("Write end dead");
            }

            readSide = Thread.CurrentThread;
            int trials = 2;
            while (inn < 0)
            {
                if (closedByWriter)
                {
                    /* closed by writer, return EOF */
                    return -1;
                }
                if ((writeSide != null) && (!writeSide.IsAlive) && (--trials < 0))
                {
                    throw new IOException("Pipe broken");
                }
                /* might be a writer waiting */
                notifyAll();
                try
                {
                    wait(1000);
                }
                catch (InterruptedException ex)
                {
                    throw new java.io.InterruptedIOException();
                }
            }
            int ret = buffer[outt++] & 0xFF;
            if (outt >= buffer.Length)
            {
                outt = 0;
            }
            if (inn == outt)
            {
                /* now empty */
                inn = -1;
            }

            return ret;
        }

        /**
         * Reads up to <code>len</code> bytes of data from this piped input
         * stream into an array of bytes. Less than <code>len</code> bytes
         * will be read if the end of the data stream is reached or if
         * <code>len</code> exceeds the pipe's buffer size.
         * If <code>len </code> is zero, then no bytes are read and 0 is returned;
         * otherwise, the method blocks until at least 1 byte of input is
         * available, end of the stream has been detected, or an exception is
         * thrown.
         *
         * @param      b     the buffer into which the data is read.
         * @param      off   the start offset in the destination array <code>b</code>
         * @param      len   the maximum number of bytes read.
         * @return     the total number of bytes read into the buffer, or
         *             <code>-1</code> if there is no more data because the end of
         *             the stream has been reached.
         * @exception  NullPointerException If <code>b</code> is <code>null</code>.
         * @exception  IndexOutOfBoundsException If <code>off</code> is negative,
         * <code>len</code> is negative, or <code>len</code> is greater than
         * <code>b.length - off</code>
         * @exception  IOException if the pipe is <a href="#BROKEN"> <code>broken</code></a>,
         *           {@link #connect(java.io.PipedOutputStream) unconnected},
         *           closed, or if an I/O error occurs.
         */
        public async int Read(byte[] b, int off, int len)
        {
            if (b == null)
            {
                throw new NullPointerException();
            }
            else if (off < 0 || len < 0 || len > b.Length - off)
            {
                throw new IndexOutOfBoundsException();
            }
            else if (len == 0)
            {
                return 0;
            }

            /* possibly wait on the first character */
            int c = Read();
            if (c < 0)
            {
                return -1;
            }
            b[off] = (byte)c;
            int rlen = 1;
            while ((inn >= 0) && (len > 1))
            {

                int available;

                if (inn > outt)
                {
                    available = Math.Min((buffer.Length - outt), (inn - outt));
                }
                else
                {
                    available = buffer.Length - outt;
                }

                // A byte is read beforehand outside the loop
                if (available > (len - 1))
                {
                    available = len - 1;
                }
                System.arraycopy(buffer, outt, b, off + rlen, available);
                outt += available;
                rlen += available;
                len -= available;

                if (outt >= buffer.Length)
                {
                    outt = 0;
                }
                if (inn == outt)
                {
                    /* now empty */
                    inn = -1;
                }
            }
            return rlen;
        }

        /**
         * Returns the number of bytes that can be read from this input
         * stream without blocking.
         *
         * @return the number of bytes that can be read from this input stream
         *         without blocking, or {@code 0} if this input stream has been
         *         closed by invoking its {@link #close()} method, or if the pipe
         *         is {@link #connect(java.io.PipedOutputStream) unconnected}, or
         *          <a href="#BROKEN"> <code>broken</code></a>.
         *
         * @exception  IOException  if an I/O error occurs.
         * @since   JDK1.0.2
         */
        public async int Available()
        {
            if (inn < 0)
                return 0;
            else if (inn == outt)
                return buffer.Length;
            else if (inn > outt)
                return inn - outt;
            else
                return inn + buffer.Length - outt;
        }

        /**
         * Closes this piped input stream and releases any system resources
         * associated with the stream.
         *
         * @exception  IOException  if an I/O error occurs.
         */
        public void Close()
        {
            closedByReader = true;
            lock (this)
            {
                inn = -1;
            }
        }

        public override void Flush()
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

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}