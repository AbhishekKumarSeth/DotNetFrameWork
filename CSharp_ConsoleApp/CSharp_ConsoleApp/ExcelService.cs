using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ConsoleApp
{
    public class ExcelService
    {
        static string connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

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

        public static DataTable GetFromDBTable()
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

        public static void CreateExcel<T>()
        {

        }

        public static void LoadExcel<T>()
        {

        }
    }
}
