using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ConsoleApp
{
    public class QRCodeService
    {
        public static void GenerateQR()
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("www.google.com", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            //Set color by using HTML hex color notation
            //Bitmap qrCodeImage = qrCode.GetGraphic(20, "#000ff0", "#0ff000");

            //Bitmap qrCodeImage = qrCode.GetGraphic(20, Color.Black, Color.White, (Bitmap)Bitmap.FromFile(@"D:\Temp Doc\QR\sample.jpeg"));

            string outputFileName = @"D:\Temp Doc\QR\QRCode.png";
            using (MemoryStream ms = new MemoryStream())
            {
                qrCodeImage.Save(ms, ImageFormat.Png);
                byte[] bytes = ms.ToArray();

                using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}
