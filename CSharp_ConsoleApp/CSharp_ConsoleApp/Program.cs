using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //ExcelService.ConvertDataTableToExcel();
            new OTPManager().GenerateOTP();
            //new EmailManager().SendEmail();

            Console.ReadKey();
        }
    }
}
