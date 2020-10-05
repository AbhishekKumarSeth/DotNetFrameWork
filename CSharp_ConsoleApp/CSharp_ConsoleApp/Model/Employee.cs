using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static List<Employee> GetAllEmployeeData()
        {
            var employees = new List<Employee>();

            var userId = Guid.NewGuid();

            for (int i = 0; i < 10; i++)
            {
                Employee emp = new Employee();
                emp.ID = Guid.NewGuid();
                emp.FullName = "Abhishek Kumar";
                emp.DOB = DateTime.UtcNow;
                emp.DOJ = DateTime.UtcNow;
                emp.Designation = "Architect";



                //update audit column Value
                //emp.CreatedBy = userId;
                //emp.CreatedDate = DateTime.UtcNow;
                //emp.ModifiedBy = userId;
                //emp.ModifiedDate = DateTime.UtcNow;

                emp = UpdateAuditColumnValue<Employee>(emp, userId);

                employees.Add(emp);
            }

            return employees;
        }


        public static T UpdateAuditColumnValue<T>(T item, Guid currentUserId) where T : class
        {
            Type t = typeof(T);
            PropertyInfo[] propertiesInfo = t.GetProperties();
            foreach (PropertyInfo prop in propertiesInfo)
            {
                var propName = prop.Name;
                switch (propName)
                {
                    case "CreatedBy":
                        prop.SetValue(item, currentUserId);
                        break;
                    case "CreatedDate":
                        prop.SetValue(item, DateTime.UtcNow);
                        break;
                    case "ModifiedBy":
                        prop.SetValue(item, currentUserId);
                        break;
                    case "ModifiedDate":
                        prop.SetValue(item, DateTime.UtcNow);
                        break;
                }
            }

            return item;
        }
    }

}
