using KoyashiroKohaku.VrcMetaToolSharp;
using System;
using System.IO;

namespace VrcMetaReader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("VrcMetaReader.exeに画像ファイルをドラッグアンドドロップしてください。");
                Console.ReadLine();
            }

            var path = args[0];

            if (!File.Exists(path))
            {
                Console.WriteLine("ファイルを読み込めませんでした。");
                Console.ReadLine();
            }

            VrcMetaData vrcMetaData;

            try
            {
                vrcMetaData = VrcMetaDataReader.Read(File.ReadAllBytes(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (vrcMetaData.Date != null)
            {
                Console.WriteLine($"Date: {vrcMetaData.Date}");
            }

            if (vrcMetaData.Photographer != null)
            {
                Console.WriteLine($"Photo by: {vrcMetaData.Photographer}");
            }

            if (vrcMetaData.World != null)
            {
                Console.WriteLine($"World: {vrcMetaData.World}");
            }

            foreach (var userName in vrcMetaData.Users)
            {
                Console.WriteLine($"User: {userName}");
            }

            Console.ReadLine();
        }
    }
}
