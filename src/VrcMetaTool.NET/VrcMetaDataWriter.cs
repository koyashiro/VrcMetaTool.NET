using System;
using System.Globalization;
using System.IO;
using System.Linq;
using KoyashiroKohaku.PngChunkUtil;

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
            if (vrcMetaData == null)
            {
                throw new ArgumentNullException(nameof(vrcMetaData));
            }

            var chunks = ChunkReader.GetChunks(image);

            foreach (var chunk in chunks.Where(c => c.TypeString == "vrCd" || c.TypeString == "vrCw" || c.TypeString == "vrCp" || c.TypeString == "vrCu").ToArray())
            {
                chunks.Remove(chunk);
            }

            chunks.Insert(chunks.Count - 1, new Chunk("vrCd", vrcMetaData.Date?.ToString("yyyyMMddHHmmssfff", new CultureInfo("ja-JP", false))));
            chunks.Insert(chunks.Count - 1, new Chunk("vrCw", vrcMetaData.World));
            chunks.Insert(chunks.Count - 1, new Chunk("vrCp", vrcMetaData.Photographer));

            foreach (var user in vrcMetaData.Users)
            {
                chunks.Insert(chunks.Count - 1, new Chunk("vrCu", user.ToString()));
            }

            return ChunkWriter.WriteImage(chunks.ToArray());
        }
        
        /// <summary>
        ///  PNG画像のバイト配列からmeta情報を書き込んだバイト配列を作成します。
        /// </summary>
        /// <param name="image">バイト配列</param>
        /// <param name="vrcMetaData">meta情報</param>
        /// <returns>meta情報を書き込んだバイト配列</returns>
        public static byte[] Write(string path, VrcMetaData vrcMetaData)
        {
            if (path == null)
            {
                throw new ArgumentNullException($"{nameof(path)}");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{path}");
            }

            return Write(File.ReadAllBytes(path), vrcMetaData);
        }
    }
}
