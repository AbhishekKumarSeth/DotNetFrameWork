using CSharp_ConsoleApp.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ConsoleApp
{
    public class ExcelService
    {
        static string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        static string[] auditColumnList = { "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate" };

        #region Create Excel From DataTable

        public static void ConvertDataTableToExcel()
        {
            DataTable dt = GetFromDBTable();

            using(var package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Employee");
                ws.Cells["A1"].LoadFromDataTable(dt, true);
                package.SaveAs(new System.IO.FileInfo("data.xlsx"));
            }
        }

        private static DataTable GetFromDBTable()
        {
            DataTable dt = new DataTable();
            using(SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [EmployeeDB].[dbo].[Employee]", conn);
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                // this will query your database and return the result to your datatable
                da.Fill(dt);
                conn.Close();
            }

            return dt;
        }

        #endregion

        #region Create Excel

        public static void CreateExcel()
        {
            //emp = SQLHelper.UpdateAuditColumnValue<Employee>(emp, Guid.NewGuid());

            var employees = Employee.GetAllEmployeeData();
            
            using (ExcelPackage package = new ExcelPackage())
            {
                CreateSpreadsheet<Employee>(package, "Employee Data", employees);
                package.SaveAs(new System.IO.FileInfo(@"D:\Temp Doc\employees.xlsx"));
                Console.WriteLine("Excel File Created Successfully");
            }
        }

        private static void CreateSpreadsheet<T>(ExcelPackage package, string name, IList<T> items)
        {
            var worksheet = package.Workbook.Worksheets.Add(name);
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells.AutoFitColumns();
            var props = typeof(T).GetProperties();
            bool isFirst = true;
            int row = 1;

            // if no data is available then create the excel only with header
            if (items.Count() == 0)
            {
                int col = 1;
                foreach (var prop in props)
                {
                    worksheet.Cells[row, col].Value = prop.Name;
                    col++;
                }
            }
            else
            {
                foreach (var item in items)
                {
                    int col = 1;
                    if (isFirst)
                    {
                        foreach (var prop in props)
                        {
                            worksheet.Cells[row, col].Value = prop.Name;
                            col++;
                        }
                        row++;
                    }
                    col = 1;
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(item);
                        if (value != null)
                        {
                            worksheet.Cells[row, col].Value = prop.GetValue(item).ToString();
                        }

                        col++;
                    }

                    row++;
                    isFirst = false;
                }
            }
        }

        #endregion

        #region Read Excel

        public static void ReadExcel()
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                byte[] file = File.ReadAllBytes(@"D:\Temp Doc\employees.xlsx");
                using (MemoryStream ms = new MemoryStream(file))
                {
                    package.Load(ms);
                    List<Employee> employees = LoadSpreadsheet<Employee>(package, "Employee Data", new List<string>()).ToList();
                    Console.WriteLine("object count is {0}", employees.Count());
                }
            }
        }

        private static IList<T> LoadSpreadsheet<T>(ExcelPackage package, string name, List<string> ignoreColumnList) where T : new()
        {
            var result = new List<T>();
            var worksheet = package.Workbook.Worksheets[name];
            //var props = typeof(T).GetProperties();
            bool isFirst = true;
            var rowCnt = worksheet.Dimension.End.Row;
            var colCnt = worksheet.Dimension.End.Column;
            Type itemType = typeof(T);

            for (int row = 2; row <= rowCnt; row++)
            {
                var item = new T();
                for (int col = 1; col <= colCnt; col++)
                {
                    string colName = worksheet.Cells[1, col].Value.ToString();

                    bool isIgnore = ignoreColumnList.Any(c => c == colName);

                    if (!isIgnore)
                    {
                        var propertyInfo = itemType.GetProperty(colName);
                        var value = worksheet.Cells[row, col].Value;
                        if (value != null)
                        {
                            var converter =
                                TypeDescriptor.GetConverter(propertyInfo.PropertyType);

                            var newValue = converter.ConvertFromString(null,
                                CultureInfo.InvariantCulture, value.ToString());
                            propertyInfo.SetValue(item, newValue, null);
                        }
                    }
                }

                result.Add(item);
            }
            return result;
        }

        #endregion
    }
}
