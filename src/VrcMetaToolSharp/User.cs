namespace KoyashiroKohaku.VrcMetaToolSharp
{
    public class User
    {
        public string UserName { get; set; }
        public string TwitterScreenName { get; set; }
        public bool HasTwitterScreenName => TwitterScreenName != null;
    }
}
