using System;
using System.Runtime.InteropServices;
using System.Text;

namespace HSignManaged
{
    public class HSign
    {
        [DllImport("DetachedSignLib.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int SignDetached(
            byte[] message,
            int cbmess_len,
            IntPtr signMessage,
            out int sm_len,
            byte[] thumbprint,
            StringBuilder errMessage,
            int em_len);
        [DllImport("DetachedSignLib.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        static extern int SignAttached(
            byte[] message,
            int cbmess_len,
            IntPtr signMessage,
            out int sm_len,
            byte[] thumbprint,
            StringBuilder errMessage,
            int em_len);

        private const int errorBufferSize = 1024;
        private const int sha1Length = 20;
        public delegate int DSign(byte[] message, int cbmess_len, IntPtr signMessage, out int sm_len, byte[] thumbprint, StringBuilder errMessage, int em_len);
        public static byte[] Sign(byte[] message, string sthumbprint)
        {
            return Sign_(message, sthumbprint, SignDetached);
        }
        public static byte[] AttachedSign(byte[] message, string sthumbprint)
        {
            return Sign_(message, sthumbprint, SignAttached);
        }

        private static byte[] Sign_(byte[] message, string sthumbprint, DSign dSign)
        {
            if (sthumbprint.Length != sha1Length * 2) return null;
            var thumbprint = new byte[sha1Length];
            for (int i = 0; i < sha1Length; i++)
            {
                thumbprint[i] = Byte.Parse(sthumbprint.Substring(i * 2, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            IntPtr signMessagePtr = IntPtr.Zero;
            StringBuilder errorMessage = new StringBuilder(errorBufferSize);
            int sm_len;
            byte[] signMessage = null;
            int res = dSign(
                message,
                message.Length,
                signMessagePtr,
                out sm_len,
                thumbprint,
                errorMessage,
                errorMessage.Capacity);

            if (res == 0)
            {
                errorMessage.Clear();
                signMessagePtr = Marshal.AllocHGlobal(sm_len);
                res = dSign(
                    message,
                    message.Length,
                    signMessagePtr,
                    out sm_len,
                    thumbprint,
                    errorMessage,
                    errorMessage.Capacity);
                if (res == 1)
                {
                    signMessage = new byte[sm_len];
                    Marshal.Copy(signMessagePtr, signMessage, 0, sm_len);
                    Marshal.FreeHGlobal(signMessagePtr);
                    return signMessage;
                }
                else
                {
                    Marshal.FreeHGlobal(signMessagePtr);
                    throw new Exception(errorMessage.ToString());
                }
            }
            else
            {
                throw new Exception(errorMessage.ToString());
            }
        }
    }
}
