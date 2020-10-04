using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ConsoleApp.Model
{
    public class Employee
    {
        // initialize with test data
        public static Employee GetEmployeeData()
        {
            Employee emp = new Employee();
            emp.ID = Guid.NewGuid();
            emp.FullName = "Abhishek Kumar";
            emp.DOB = DateTime.UtcNow;
            emp.DOJ = DateTime.UtcNow;
            emp.Designation = "Architect";
            return emp;
        }

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
