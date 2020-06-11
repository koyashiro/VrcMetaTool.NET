using KoyashiroKohaku.VrcMetaTool.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace KoyashiroKohaku.VrcMetaTool
{
    public static class VrcMetaChunk
    {
        /// <summary>
        /// vrCd
        /// </summary>
        public static ReadOnlySpan<byte> DateChunk => new byte[] { 0x76, 0x72, 0x43, 0x64 };

        /// <summary>
        /// vrCw
        /// </summary>
        public static ReadOnlySpan<byte> WorldChunk => new byte[] { 0x76, 0x72, 0x43, 0x77 };

        /// <summary>
        /// vrCp
        /// </summary>
        public static ReadOnlySpan<byte> PhotographerChunk => new byte[] { 0x76, 0x72, 0x43, 0x70 };

        /// <summary>
        /// vrCu
        /// </summary>
        public static ReadOnlySpan<byte> UserChunk => new byte[] { 0x76, 0x72, 0x43, 0x75 };

        /// <summary>
        /// vrc_meta_toolのmeta情報かどうかを判定します。
        /// </summary>
        /// <param name="chunkType"></param>
        /// <returns></returns>
        public static bool IsVrcMetaChunk(ReadOnlySpan<byte> chunkType)
        {
            if (chunkType == null)
            {
                throw new ArgumentNullException(nameof(chunkType));
            }

            if (chunkType.Length != 4)
            {
                throw new ArgumentException(Resources.VrcMetaChunk_IsVrcMetaChunk_ArgumentException, nameof(chunkType));
            }

            return chunkType.SequenceEqual(DateChunk)
                || chunkType.SequenceEqual(WorldChunk)
                || chunkType.SequenceEqual(PhotographerChunk)
                || chunkType.SequenceEqual(UserChunk);
        }

        /// <summary>
        /// chunk typeを文字列に変換します。
        /// </summary>
        /// <param name="chunkType"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ConvertToString(ReadOnlySpan<byte> chunkType, Encoding? encoding = null)
        {
            if (chunkType == null)
            {
                throw new ArgumentNullException(nameof(chunkType));
            }

            if (chunkType.Length != 4)
            {
                throw new ArgumentException(Resources.VrcMetaChunk_IsVrcMetaChunk_ArgumentException, nameof(chunkType));
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return encoding.GetString(chunkType);
        }
    }
}
