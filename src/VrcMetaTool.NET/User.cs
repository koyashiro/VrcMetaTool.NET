using System;
using System.Text.RegularExpressions;

namespace Koyashiro.VrcMetaTool
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        public User(string userName)
        {
            if (userName is null)
            {
                throw new ArgumentNullException(nameof(userName));
            }

            var match = new Regex(@"(?<userName>.*) : (?<twitterScreenName>@[0-9a-zA-Z_]*)").Match(userName);

            if (match.Success)
            {
                UserName = match.Groups["userName"].Value;
                TwitterScreenName = match.Groups["twitterScreenName"].Value;
            }
            else
            {
                UserName = userName;
            }
        }

        /// <summary>
        /// VRCのdisplay name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Twitterのscreen name
        /// </summary>
        public string? TwitterScreenName { get; set; }

        /// <summary>
        /// <see cref="TwitterScreenName"/>が格納されているときに<see cref="true"/>を返します。
        /// </summary>
        public bool HasTwitterScreenName => TwitterScreenName != null;

        public override string ToString()
        {
            if (HasTwitterScreenName)
            {
                return $"{UserName} : {TwitterScreenName}";
            }
            else
            {
                return UserName;
            }
        }
    }
}
