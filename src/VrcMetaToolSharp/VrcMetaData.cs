using System;
using System.Collections.Generic;

namespace KoyashiroKohaku.VrcMetaToolSharp
{
    public class VrcMetaData
    {
        public DateTime? Date { get; set; }
        public string Photographer { get; set; }
        public string World { get; set; }
        public List<User> Users { get; } = new List<User>();
    }
}
