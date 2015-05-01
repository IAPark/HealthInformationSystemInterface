using System;
using System.Web.Helpers;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections.Specialized;

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

        public static DataTable getDataTable(string query)
        {
            SqlDataAdapter adp = getAdapter(query);
            DataTable table = new DataTable();
            adp.Fill(table);
            return table;
        }
        public static SqlDataAdapter getAdapter(string query)
        {
            SqlConnection connection = DbUtil.getConnection();
            var adp = new SqlDataAdapter(query, connection);
            new SqlCommandBuilder(adp);
            return adp;
        }

        //assumes the form has names to match the columns in the db
        public static void addFromForm(NameValueCollection form, string table_name)
        {
            SqlDataAdapter adp = DbUtil.getAdapter("select * from " + table_name + " where 1=0");

            DataTable table = new DataTable();
            adp.Fill(table);

            var row = table.NewRow();

            foreach (DataColumn column in table.Columns)
            {
                string name = column.ColumnName;
                //this check is probably not needed, but I'm not sure about behavior, especially with self incrementing primary keys
                if (form[name] != null && !form[name].Equals(""))
                {
                    row[name] = form[name];
                }
            }

            table.Rows.Add(row);
            adp.Update(table);
        }

    }
}