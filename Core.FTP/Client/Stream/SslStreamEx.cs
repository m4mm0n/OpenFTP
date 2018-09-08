using System;
using System.Net.Security;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using Core.Utils;

namespace Core.FTP.Client.Stream
{
    /// <summary>
    /// Based solely on FluentFTP's FtpSslStream
    /// https://github.com/robinrodricks/FluentFTP/blob/master/FluentFTP/Stream/FtpSslStream.cs
    /// </summary>
	internal class SslStreamEx : SslStream
    {

        private bool sentCloseNotify = false;

        public SslStreamEx(System.IO.Stream innerStream)
            : base(innerStream)
        {
        }
        public SslStreamEx(System.IO.Stream innerStream, bool leaveInnerStreamOpen)
            : base(innerStream, leaveInnerStreamOpen)
        {
        }
        public SslStreamEx(System.IO.Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback)
            : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback)
        {
        }
        public SslStreamEx(System.IO.Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback)
            : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback)
        {
        }
        public SslStreamEx(System.IO.Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy)
            : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback, encryptionPolicy)
        {
        }

        public override void Close()
        {
            try
            {
                if (!sentCloseNotify)
                {
                    SslDirectCall.CloseNotify(this);
                    sentCloseNotify = true;
                }
            }
            finally
            {
                base.Close();
            }
        }
    }

    internal unsafe static class SslDirectCall
    {
        /// <summary>
        /// Send an SSL close_notify alert.
        /// </summary>
        /// <param name="sslStream"></param>
        public static void CloseNotify(SslStream sslStream)
        {
            if (sslStream.IsAuthenticated && sslStream.CanWrite)
            {
                bool isServer = sslStream.IsServer;

                byte[] result;
                int resultSz;
                var asmbSystem = typeof(System.Net.Authorization).Assembly;

                int SCHANNEL_SHUTDOWN = 1;
                var workArray = BitConverter.GetBytes(SCHANNEL_SHUTDOWN);

                var sslstate = sslStream.GetField("_SslState"); //FtpReflection.GetField(sslStream, "_SslState");
                var context = sslstate.GetProperty("Context"); //FtpReflection.GetProperty(sslstate, "Context");

                var securityContext = context.GetField("m_SecurityContext"); //FtpReflection.GetField(context, "m_SecurityContext");
                var securityContextHandleOriginal = securityContext.GetField("_handle"); //FtpReflection.GetField(securityContext, "_handle");
                SslNativeApi.SSPIHandle securityContextHandle = default(SslNativeApi.SSPIHandle);
                securityContextHandle.HandleHi = (IntPtr) securityContextHandleOriginal.GetField("HandleHi"); //FtpReflection.GetField(securityContextHandleOriginal, "HandleHi");
                securityContextHandle.HandleLo = (IntPtr) securityContextHandleOriginal.GetField("HandleLo"); //FtpReflection.GetField(securityContextHandleOriginal, "HandleLo");

                var credentialsHandle = context.GetField("m_CredentialsHandle"); //FtpReflection.GetField(context, "m_CredentialsHandle");
                var credentialsHandleHandleOriginal = credentialsHandle.GetField("_handle"); //FtpReflection.GetField(credentialsHandle, "_handle");
                SslNativeApi.SSPIHandle credentialsHandleHandle = default(SslNativeApi.SSPIHandle);
                credentialsHandleHandle.HandleHi = (IntPtr) credentialsHandleHandleOriginal.GetField("HandleHi"); //FtpReflection.GetField(credentialsHandleHandleOriginal, "HandleHi");
                credentialsHandleHandle.HandleLo = (IntPtr) credentialsHandleHandleOriginal.GetField("HandleLo"); //FtpReflection.GetField(credentialsHandleHandleOriginal, "HandleLo");

                int bufferSize = 1;
                SslNativeApi.SecurityBufferDescriptor securityBufferDescriptor = new SslNativeApi.SecurityBufferDescriptor(bufferSize);
                SslNativeApi.SecurityBufferStruct[] unmanagedBuffer = new SslNativeApi.SecurityBufferStruct[bufferSize];

                fixed (SslNativeApi.SecurityBufferStruct* ptr = unmanagedBuffer)
                fixed (void* workArrayPtr = workArray)
                {
                    securityBufferDescriptor.UnmanagedPointer = (void*)ptr;

                    unmanagedBuffer[0].token = (IntPtr)workArrayPtr;
                    unmanagedBuffer[0].count = workArray.Length;
                    unmanagedBuffer[0].type = SslNativeApi.BufferType.Token;

                    SslNativeApi.SecurityStatus status;
                    status = (SslNativeApi.SecurityStatus)SslNativeApi.ApplyControlToken(ref securityContextHandle, securityBufferDescriptor);
                    if (status == SslNativeApi.SecurityStatus.OK)
                    {
                        unmanagedBuffer[0].token = IntPtr.Zero;
                        unmanagedBuffer[0].count = 0;
                        unmanagedBuffer[0].type = SslNativeApi.BufferType.Token;

                        SslNativeApi.SSPIHandle contextHandleOut = default(SslNativeApi.SSPIHandle);
                        SslNativeApi.ContextFlags outflags = SslNativeApi.ContextFlags.Zero;
                        long ts = 0;

                        var inflags = SslNativeApi.ContextFlags.SequenceDetect |
                                    SslNativeApi.ContextFlags.ReplayDetect |
                                    SslNativeApi.ContextFlags.Confidentiality |
                                    SslNativeApi.ContextFlags.AcceptExtendedError |
                                    SslNativeApi.ContextFlags.AllocateMemory |
                                    SslNativeApi.ContextFlags.InitStream;

                        if (isServer)
                        {
                            status = (SslNativeApi.SecurityStatus)SslNativeApi.AcceptSecurityContext(ref credentialsHandleHandle, ref securityContextHandle, null,
                                inflags, SslNativeApi.Endianness.Native, ref contextHandleOut, securityBufferDescriptor, ref outflags, out ts);
                        }
                        else
                        {
                            status = (SslNativeApi.SecurityStatus)SslNativeApi.InitializeSecurityContextW(ref credentialsHandleHandle, ref securityContextHandle, null,
                                inflags, 0, SslNativeApi.Endianness.Native, null, 0, ref contextHandleOut, securityBufferDescriptor, ref outflags, out ts);
                        }
                        if (status == SslNativeApi.SecurityStatus.OK)
                        {
                            byte[] resultArr = new byte[unmanagedBuffer[0].count];
                            Marshal.Copy(unmanagedBuffer[0].token, resultArr, 0, resultArr.Length);
                            Marshal.FreeCoTaskMem(unmanagedBuffer[0].token);
                            result = resultArr;
                            resultSz = resultArr.Length;
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format("AcceptSecurityContext/InitializeSecurityContextW returned [{0}] during CloseNotify.", status));
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("ApplyControlToken returned [{0}] during CloseNotify.", status));
                    }
                }

                var innerStream = (System.IO.Stream) sslstate.GetProperty("InnerStream");//FtpReflection.GetProperty(sslstate, "InnerStream");
                innerStream.Write(result, 0, resultSz);
            }
        }
    }

    internal unsafe static class SslNativeApi
    {
        internal enum BufferType
        {
            Empty,
            Data,
            Token,
            Parameters,
            Missing,
            Extra,
            Trailer,
            Header,
            Padding = 9,
            Stream,
            ChannelBindings = 14,
            TargetHost = 16,
            ReadOnlyFlag = -2147483648,
            ReadOnlyWithChecksum = 268435456
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SSPIHandle
        {
            public IntPtr HandleHi;
            public IntPtr HandleLo;
            public bool IsZero
            {
                get
                {
                    return this.HandleHi == IntPtr.Zero && this.HandleLo == IntPtr.Zero;
                }
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            internal void SetToInvalid()
            {
                this.HandleHi = IntPtr.Zero;
                this.HandleLo = IntPtr.Zero;
            }
            public override string ToString()
            {
                return this.HandleHi.ToString("x") + ":" + this.HandleLo.ToString("x");
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        internal class SecurityBufferDescriptor
        {
            public readonly int Version;
            public readonly int Count;
            public unsafe void* UnmanagedPointer;
            public SecurityBufferDescriptor(int count)
            {
                this.Version = 0;
                this.Count = count;
                this.UnmanagedPointer = null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SecurityBufferStruct
        {
            public int count;
            public BufferType type;
            public IntPtr token;
            public static readonly int Size = sizeof(SecurityBufferStruct);
        }

        internal enum SecurityStatus
        {
            OK,
            ContinueNeeded = 590610,
            CompleteNeeded,
            CompAndContinue,
            ContextExpired = 590615,
            CredentialsNeeded = 590624,
            Renegotiate,
            OutOfMemory = -2146893056,
            InvalidHandle,
            Unsupported,
            TargetUnknown,
            InternalError,
            PackageNotFound,
            NotOwner,
            CannotInstall,
            InvalidToken,
            CannotPack,
            QopNotSupported,
            NoImpersonation,
            LogonDenied,
            UnknownCredentials,
            NoCredentials,
            MessageAltered,
            OutOfSequence,
            NoAuthenticatingAuthority,
            IncompleteMessage = -2146893032,
            IncompleteCredentials = -2146893024,
            BufferNotEnough,
            WrongPrincipal,
            TimeSkew = -2146893020,
            UntrustedRoot,
            IllegalMessage,
            CertUnknown,
            CertExpired,
            AlgorithmMismatch = -2146893007,
            SecurityQosFailed,
            SmartcardLogonRequired = -2146892994,
            UnsupportedPreauth = -2146892989,
            BadBinding = -2146892986
        }
        [Flags]
        internal enum ContextFlags
        {
            Zero = 0,
            Delegate = 1,
            MutualAuth = 2,
            ReplayDetect = 4,
            SequenceDetect = 8,
            Confidentiality = 16,
            UseSessionKey = 32,
            AllocateMemory = 256,
            Connection = 2048,
            InitExtendedError = 16384,
            AcceptExtendedError = 32768,
            InitStream = 32768,
            AcceptStream = 65536,
            InitIntegrity = 65536,
            AcceptIntegrity = 131072,
            InitManualCredValidation = 524288,
            InitUseSuppliedCreds = 128,
            InitIdentify = 131072,
            AcceptIdentify = 524288,
            ProxyBindings = 67108864,
            AllowMissingBindings = 268435456,
            UnverifiedTargetName = 536870912
        }
        internal enum Endianness
        {
            Network,
            Native = 16
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern int ApplyControlToken(ref SSPIHandle contextHandle, [In] [Out] SecurityBufferDescriptor outputBuffer);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
        internal unsafe static extern int AcceptSecurityContext(ref SSPIHandle credentialHandle, ref SSPIHandle contextHandle, [In] SecurityBufferDescriptor inputBuffer, [In] ContextFlags inFlags, [In] Endianness endianness, ref SSPIHandle outContextPtr, [In] [Out] SecurityBufferDescriptor outputBuffer, [In] [Out] ref ContextFlags attributes, out long timeStamp);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
        internal unsafe static extern int InitializeSecurityContextW(ref SSPIHandle credentialHandle, ref SSPIHandle contextHandle, [In] byte* targetName, [In] ContextFlags inFlags, [In] int reservedI, [In] Endianness endianness, [In] SecurityBufferDescriptor inputBuffer, [In] int reservedII, ref SSPIHandle outContextPtr, [In] [Out] SecurityBufferDescriptor outputBuffer, [In] [Out] ref ContextFlags attributes, out long timeStamp);
    }
}
