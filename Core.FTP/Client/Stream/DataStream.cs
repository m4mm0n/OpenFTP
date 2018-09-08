using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.FTP.Helpers;

namespace Core.FTP.Client.Stream
{
    public class DataStream : SocketStream
    {
        public ActionReply CommandStatus;
        public FTPClient ControlConnection;

        private long m_length = 0;
        public override long Length => m_length;

        private long m_position = 0;
        public override long Position
        {
            get => m_position;
            set => throw new InvalidOperationException(
                "Cannot modify this value. This value is updated accordingly together with uploading.");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = base.Read(buffer, offset, count);
            m_position += read;
            return read;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            var read = await base.ReadAsync(buffer, offset, count, token);
            m_position += read;
            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            m_position += count;
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            await base.WriteAsync(buffer, offset, count, token);
            m_position += count;
        }

        public override void SetLength(long value)
        {
            m_length = value;
        }

        public void SetPosition(long pos)
        {
            m_position = pos;
        }

        public new ActionReply Close()
        {
            base.Close();
            try
            {
                if (ControlConnection != null)
                    return ControlConnection.CloseDataStream(this);
            }
            finally
            {
                CommandStatus = new ActionReply();
                ControlConnection = null;
            }

            return new ActionReply();
        }

        public DataStream(FTPClient conn) : base(conn.ConnectionInfo.Security)
        {
            if (conn == null)
                throw new ArgumentException("The control connection cannot be null.");

            ControlConnection = conn;
            // always accept certificate no matter what because if code execution ever
            // gets here it means the certificate on the control connection object being
            // cloned was already accepted.
            //ValidateCertificate += new FtpSocketStreamSslValidation(delegate (FtpSocketStream obj, FtpSslValidationEventArgs e) {
            //    e.Accept = true;
            //});

            m_position = 0;
        }
        ~DataStream()
        {
            try
            {
                Dispose(false);
            }
            catch (Exception ex)
            {
               // FtpTrace.WriteLine(FtpTraceLevel.Warn, "[Finalizer] Caught and discarded an exception while disposing the FtpDataStream: " + ex.ToString());
            }
        }
    }
}
