using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ConsoleApp.Model
{
    public class Employee
    {
        public Guid ID { get; set; }
        public string FullName { get; set; }
        public string ContactNo { get; set; }
        public DateTime DOB { get; set; }
        public DateTime DOJ { get; set; }
        public string Designation { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
