using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;

namespace FinalUniProject.helperClasses
{
    // This class contains CRUD action methods for management of any database work that may need to be done.
    public static class database
    {
        public static SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnection"].ConnectionString);

        public static DataTable get_db_content(string theSQL) {
            using (var da = new SqlDataAdapter(theSQL,conn))
            {
                var table = new DataTable();
                da.Fill(table);
                return table;
            }
        }
        public static bool execute_non_query(string theSQL)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(theSQL);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
        public static string datatable_to_json(DataTable theDT)
        {
            return JsonConvert.SerializeObject(theDT, Formatting.None);
        }
    }
}