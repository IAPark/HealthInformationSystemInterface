using System;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace HealthInformationSystemInterface.App_Code
{
    public class DbUtil
    {
        public static string cns = ConfigurationManager.ConnectionStrings["sqlconnection"].ConnectionString;

        private DbUtil() { }

        public static SqlConnection getConnection()
        {
            SqlConnection connection = new SqlConnection(cns);
            connection.Open();
            return connection;
        }

        public static WebGrid getWebGridFromSelection(String query)
        {
            var table = getDataTable(query);
            var result = new List<dynamic>();
            foreach (DataRow row in table.Rows)
            {
                var obj = (IDictionary<string, object>)new ExpandoObject();
                foreach (DataColumn col in table.Columns)
                {
                    obj.Add(col.ColumnName, row[col.ColumnName]);
                }
                result.Add(obj);
            }
            return new WebGrid(result);
        }

        public static DataTable getDataTable(String query)
        {
            SqlConnection connection = DbUtil.getConnection();

            SqlDataAdapter adp = new SqlDataAdapter(query, connection);
            DataTable table = new DataTable();
            adp.Fill(table);
            return table;
        }
    }
}