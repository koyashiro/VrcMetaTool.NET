using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace KoyashiroKohaku.VrcMetaTool
{
    /// <summary>
    /// VrcMetaDataWriter
    /// </summary>
    public static class VrcMetaDataWriter
    {
        /// <summary>
        ///  PNG画像のバイト配列からmeta情報を書き込んだバイト配列を作成します。
        /// </summary>
        /// <param name="image">バイト配列</param>
        /// <param name="vrcMetaData">meta情報</param>
        /// <returns>meta情報を書き込んだバイト配列</returns>
        public static byte[] Write(ReadOnlySpan<byte> image, VrcMetaData vrcMetaData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///  PNG画像のバイト配列からmeta情報を書き込んだバイト配列を作成します。
        /// </summary>
        /// <param name="image">バイト配列</param>
        /// <param name="vrcMetaData">meta情報</param>
        /// <returns>meta情報を書き込んだバイト配列</returns>
        public static byte[] Write(string path, VrcMetaData vrcMetaData)
        {
            throw new NotImplementedException();
        }
    }
}
