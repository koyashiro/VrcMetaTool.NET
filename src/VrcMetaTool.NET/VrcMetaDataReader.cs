using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KoyashiroKohaku.PngChunkUtil;
using KoyashiroKohaku.VrcMetaTool.Properties;

namespace KoyashiroKohaku.VrcMetaTool
{
    /// <summary>
    /// VrcMetaDataReader
    /// </summary>
    public static class VrcMetaDataReader
    {
        /// <summary>
        /// PNG画像のシグネチャ
        /// </summary>
        private static ReadOnlySpan<byte> PngSignature => new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };

        /// <summary>
        /// vrc_meta_toolで扱うChunk Type
        /// </summary>
        public static ReadOnlySpan<string> VrcMetaDataChunkTypes => new string[] { "vrCd", "vrCw", "vrCp", "vrCu" };

        /// <summary>
        /// バイト配列がPNG画像のとき<see cref="true"/>を返します。
        /// </summary>
        /// <param name="buffer">バイト配列</param>
        /// <returns>バイト配列がPNG画像のとき<see cref="true"/>, それ以外のとき<see cref="false"/></returns>
        public static bool IsPng(ReadOnlySpan<byte> buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return ChunkReader.IsPng(buffer);
        }

        /// <summary>
        /// PNG画像のバイト配列からmeta情報を抽出します。
        /// </summary>
        /// <param name="buffer">バイト配列</param>
        /// <returns>meta情報</returns>
        public static VrcMetaData Read(ReadOnlySpan<byte> buffer)
        {
            #region Argument Check
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var span = buffer;

            if (!span[..8].SequenceEqual(PngSignature))
            {
                throw new ArgumentException(Resources.VrcMetaReader_Read_ArgumentException, nameof(buffer));
            }
            #endregion

            var chunks = ChunkReader.GetChunks(buffer, ChunkTypeFilter.AdditionalChunkOnly);

            var dateChunk = chunks.SingleOrDefault(c => c.TypeString == "vrCd");

            DateTime? date;
            if (dateChunk != null)
            {
                if (DateTime.TryParseExact(dateChunk.DataString, "yyyyMMddHHmmssfff", CultureInfo.CurrentCulture, DateTimeStyles.None, out var parsedDate))
                {
                    date = parsedDate;
                }
                else
                {
                    date = null;
                }
            }
            else
            {
                date = null;
            }

            var worldChunk = chunks.SingleOrDefault(c => c.TypeString == "vrCw");

            var photographerChunk = chunks.SingleOrDefault(c => c.TypeString == "vrCp");
            var userChunks = chunks.Where(c => c.TypeString == "vrCu").ToList();

            var vrcMetaData = new VrcMetaData
            {
                Date = date,
                World = worldChunk?.DataString,
                Photographer = photographerChunk?.DataString
            };

            vrcMetaData.Users.AddRange(userChunks.Select(c => new User(c.DataString)).ToList());

            return vrcMetaData;
        }

        /// <summary>
        /// ファイルパスからPNG画像を読み込みmeta情報を抽出します。
        /// </summary>
        /// <param name="path">PNG画像のファイルパス</param>
        /// <returns>meta情報</returns>
        public static VrcMetaData Read(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            return Read(File.ReadAllBytes(path));
        }

        /// <summary>
        /// ファイルパスからPNG画像を読み込みmeta情報を抽出します。
        /// </summary>
        /// <param name="path">PNG画像のファイルパス</param>
        /// <returns>meta情報</returns>
        public static async Task<VrcMetaData> ReadAsync(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            return await ReadAsync(await File.ReadAllBytesAsync(path).ConfigureAwait(false)).ConfigureAwait(false);
        }

        /// <summary>
        /// PNG画像のバイナリデータからmeta情報を抽出します。
        /// </summary>
        /// <param name="buffer">PNG画像のバイナリ</param>
        /// <returns>meta情報</returns>
        public static Task<VrcMetaData> ReadAsync(byte[] buffer) => Task.Run(() => Read(buffer));
    }
}
