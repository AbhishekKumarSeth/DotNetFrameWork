using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ConsoleApp
{
    public class ZIPServie
    {
        public static void CreateZip()
        {
            string zipPath = @"D:\Temp Doc\Final.zip";

            using (ZipFile zip = new ZipFile())
            {
                zip.Password = "ABCD";
                zip.AddFile(@"D:\Temp Doc\file.csv", "");
                zip.AddFile(@"D:\Temp Doc\Test.txt", "");
                zip.Save(zipPath);
            }
        }

        public static void ExtratZIPFile()
        {
            using (ZipFile zip = ZipFile.Read(@"D:\Temp Doc\Final.zip"))
            {
                zip.Password = "ABCD";
                zip.ExtractAll(@"D:\Temp Doc\test\", ExtractExistingFileAction.DoNotOverwrite);
                var files = zip.ToList();
            }
        }
    }
}
