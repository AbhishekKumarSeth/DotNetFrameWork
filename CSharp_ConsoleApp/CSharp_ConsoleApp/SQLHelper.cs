using CSharp_ConsoleApp.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ConsoleApp
{
    public class SQLHelper
    {
        static string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        static string[] auditColumnList = { "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate" };

        public static void InsertDataToSql()
        {
            var userId = Guid.NewGuid();
            Employee emp = GetEmployeeData();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                var sqlqueryData = CreateInsertQuery<Employee>(emp, "Employee", userId);
                using (SqlCommand cmd = new SqlCommand(sqlqueryData.Item1, conn))
                {
                    cmd.Parameters.AddRange(sqlqueryData.Item2);

                    conn.Open();

                    int noOfRowEffected = cmd.ExecuteNonQuery();
                    Console.WriteLine(noOfRowEffected + "row insterted to database table");

                    conn.Close();
                }
            }
        }

        private static Employee GetEmployeeData()
        {
            Employee emp = new Employee();
            emp.ID = Guid.NewGuid();
            emp.FullName = "Abhishek Kumar";
            emp.DOB = DateTime.UtcNow;
            emp.DOJ = DateTime.UtcNow;
            emp.Designation = "Architect";
            return emp;
        }

        public static void UpdateData()
        {
            var userId = Guid.NewGuid();
            Employee emp = GetEmployeeData();
            emp = UpdateAuditColumnValue<Employee>(emp, userId);

            Console.WriteLine(emp.CreatedBy);
            Console.WriteLine(emp.CreatedDate);
            Console.WriteLine(emp.ModifiedBy);
            Console.WriteLine(emp.ModifiedDate);
        }

        private static Tuple<string, SqlParameter[]> CreateInsertQuery<T>(T item, string tableName, Guid curretUserId) where T : new()
        {
            string columnDetails = string.Empty;
            string columnParams = string.Empty;

            List<SqlParameter> parameterList = new List<SqlParameter>();

            Type t = typeof(T);
            PropertyInfo[] propertiesInfo = t.GetProperties();
            foreach (PropertyInfo prop in propertiesInfo)
            {
                var propName = prop.Name;
                var propvalue = prop.GetValue(item, new object[] { });

                Type columnType = prop.PropertyType;

                bool isAuditColumn = auditColumnList.Any(c => c == propName);
                if (isAuditColumn)
                {
                    if (propName == "CreatedDate" || propName == "ModifiedDate")
                    {
                        propvalue = DateTime.UtcNow;
                        columnDetails += "[" + propName + "], ";
                        columnParams += "@" + propName + ", ";
                        parameterList.Add(new SqlParameter("@" + propName, propvalue));
                    }
                    if (propName == "CreatedBy" || propName == "ModifiedBy")
                    {
                        propvalue = curretUserId;
                        columnDetails += "[" + propName + "], ";
                        columnParams += "@" + propName + ", ";
                        parameterList.Add(new SqlParameter("@" + propName, propvalue));
                    }

                    continue;
                }
                else
                {
                    columnDetails += "[" + propName + "], ";
                    columnParams += "@" + propName + ", ";

                    parameterList.Add(new SqlParameter("@" + propName, propvalue ?? (object)DBNull.Value));
                }
            }

            columnDetails = columnDetails.TrimEnd(", ".ToCharArray());
            columnParams = columnParams.TrimEnd(", ".ToCharArray());

            SqlParameter[] parameters = parameterList.ToArray();
            var query = string.Format("INSERT INTO [dbo].[{0}] ({1}) VALUES ({2})", tableName, columnDetails, columnParams);

            return new Tuple<string, SqlParameter[]>(query, parameters);
        }

        private static T UpdateAuditColumnValue<T>(T item, Guid currentUserId) where T : class
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
