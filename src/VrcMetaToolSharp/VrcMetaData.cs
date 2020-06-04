using System;
using System.Collections.Generic;

namespace KoyashiroKohaku.VrcMetaToolSharp
{
    /// <summary>
    /// VrcMetaData
    /// </summary>
    public class VrcMetaData
    {
        /// <summary>
        /// 撮影日時
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 撮影者
        /// </summary>
        public string Photographer { get; set; }

        /// <summary>
        /// ワールド名
        /// </summary>
        public string World { get; set; }

        /// <summary>
        /// ユーザ情報を格納するリスト
        /// </summary>
        public List<User> Users { get; } = new List<User>();
    }
}
