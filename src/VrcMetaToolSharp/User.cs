namespace KoyashiroKohaku.VrcMetaToolSharp
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        /// <summary>
        /// VRCのdisplay name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Twitterのscreen name
        /// </summary>
        public string TwitterScreenName { get; set; }

        /// <summary>
        /// <see cref="TwitterScreenName"/>が格納されているときに<see cref="true"/>を返します。
        /// </summary>
        public bool HasTwitterScreenName => TwitterScreenName != null;
    }
}
