using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSGraphAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            EmailService em = new EmailService();
            em.SendEmail();

            Console.ReadKey();
        }
    }
}
