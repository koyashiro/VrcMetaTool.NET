using Koyashiro.VrcMetaTool.Properties;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Koyashiro.VrcMetaTool
{
    /// <summary>
    /// VrcMetaDataReader
    /// </summary>
    public static class VrcMetaDataReader
    {
        /// <summary>
        /// バイト配列がPNG画像のとき<see cref="true"/>を返します。
        /// </summary>
        /// <param name="buffer">バイト配列</param>
        /// <returns>バイト配列がPNG画像のとき<see cref="true"/>, それ以外のとき<see cref="false"/></returns>
        public static bool IsPng(ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// PNG画像のバイト配列からmeta情報を抽出します。
        /// </summary>
        /// <param name="buffer">バイト配列</param>
        /// <returns>meta情報</returns>
        public static VrcMetaData Read(ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// PNG画像のバイト配列からmeta情報を抽出します。
        /// </summary>
        /// <param name="buffer">バイト配列</param>
        /// <returns>meta情報</returns>
        public static bool TryRead(ReadOnlySpan<byte> buffer, out VrcMetaData? vrcMetaData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ファイルパスからPNG画像を読み込みmeta情報を抽出します。
        /// </summary>
        /// <param name="path">PNG画像のファイルパス</param>
        /// <returns>meta情報</returns>
        public static VrcMetaData Read(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ファイルパスからPNG画像を読み込みmeta情報を抽出します。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="vrcMetaData"></param>
        /// <returns></returns>
        public static bool TryRead(string path, out VrcMetaData? vrcMetaData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ファイルパスからPNG画像を読み込みmeta情報を抽出します。
        /// </summary>
        /// <param name="path">PNG画像のファイルパス</param>
        /// <returns>meta情報</returns>
        public static Task<VrcMetaData> ReadAsync(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// PNG画像のバイナリデータからmeta情報を抽出します。
        /// </summary>
        /// <param name="buffer">PNG画像のバイナリ</param>
        /// <returns>meta情報</returns>
        public static Task<VrcMetaData> ReadAsync(byte[] buffer)
        {
            throw new NotImplementedException();
        }
    }
}
