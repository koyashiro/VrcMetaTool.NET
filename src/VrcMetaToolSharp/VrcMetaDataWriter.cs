using System;
using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KoyashiroKohaku.PngMetaDataUtil;

namespace KoyashiroKohaku.VrcMetaToolSharp
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
            var chunks = ChunkReader.GetChunks(image);

            foreach (var chunk in chunks.Where(c => c.TypeString == "vrCd" || c.TypeString == "vrCw" || c.TypeString == "vrCp" || c.TypeString == "vrCu").ToArray())
            {
                chunks.Remove(chunk);
            }

            chunks.Insert(chunks.Count - 1, new Chunk("vrCd", vrcMetaData.Date?.ToString("yyyyMMddHHmmssfff")));
            chunks.Insert(chunks.Count - 1, new Chunk("vrCw", vrcMetaData.World.ToString()));
            chunks.Insert(chunks.Count - 1, new Chunk("vrCp", vrcMetaData.Photographer.ToString()));

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
            if (path is null)
            {
                throw new ArgumentNullException($"Argument error. argument: '{nameof(path)}' is null.");
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File error. '{path}' does not exists.");
            }

            return Write(File.ReadAllBytes(path), vrcMetaData);
        }
    }
}
