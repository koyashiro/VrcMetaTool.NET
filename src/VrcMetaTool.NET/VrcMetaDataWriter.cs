using KoyashiroKohaku.PngChunkUtil;
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
            if (image == null)
            {
                throw new ArgumentNullException(nameof(vrcMetaData));
            }

            if (vrcMetaData == null)
            {
                throw new ArgumentNullException(nameof(vrcMetaData));
            }

            var chunks = ChunkReader.SplitChunks(image);

            // 既存のmeta情報を削除
            foreach (var chunk in chunks.Where(c => VrcMetaChunk.IsVrcMetaChunk(c.TypePart)).ToArray())
            {
                chunks.Remove(chunk);
            }

            // 受け取ったmeta情報を末尾に追加
            chunks.Insert(chunks.Count - 1, new Chunk(VrcMetaChunk.ConvertToString(VrcMetaChunk.DateChunk), vrcMetaData.Date?.ToString("yyyyMMddHHmmssfff", new CultureInfo("en", false))));
            chunks.Insert(chunks.Count - 1, new Chunk(VrcMetaChunk.ConvertToString(VrcMetaChunk.WorldChunk), vrcMetaData.World));
            chunks.Insert(chunks.Count - 1, new Chunk(VrcMetaChunk.ConvertToString(VrcMetaChunk.PhotographerChunk), vrcMetaData.Photographer));

            foreach (var user in vrcMetaData.Users)
            {
                chunks.Insert(chunks.Count - 1, new Chunk(VrcMetaChunk.ConvertToString(VrcMetaChunk.UserChunk), user.ToString()));
            }

            return ChunkWriter.WriteImageBytes(chunks.ToArray());
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
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            if (vrcMetaData == null)
            {
                throw new ArgumentNullException(nameof(vrcMetaData));
            }

            return Write(File.ReadAllBytes(path), vrcMetaData);
        }
    }
}
