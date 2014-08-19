using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using FinalUniProject.Models;
using System.Web;
using System.Data;
using FinalUniProject.NERModels;
using Newtonsoft.Json;

namespace FinalUniProject.helperClasses
{
    public static class Database
    {
        private static SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnection"].ConnectionString);

        public static bool Query(string sqlStr)
        {
            try
            {
                conn.Open();
                var cmd = new SqlCommand(sqlStr);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException sqlEx)
            {
                return false;
            }
        }
        public static DataTable GetAsDataTable(string sqlStr)
        {
            using (var da = new SqlDataAdapter(sqlStr, conn))
            {
                var table = new DataTable();
                da.Fill(table);
                return table;
            }
        }
        public static string DataTable_To_JSON(DataTable dt)
        {
            return JsonConvert.SerializeObject(dt, Formatting.None);
        }
        //public List<Entity<Tweet>> ConvertToObject(string sqlStr)
        //{
        //    using (conn)
        //    {
        //        SqlCommand cmd = new SqlCommand(sqlStr, conn);
        //        conn.Open();

        //        SqlDataReader reader = cmd.ExecuteReader();

        //        if (reader.HasRows)
        //        {
        //            while (reader.Read())
        //            {
        //                //var entity = new Entity<Tweet>()
        //                //{
        //                //    Name = reader["entityName"].ToString(),
                            

        //                //};
        //            }
        //        }
        //    }
        //}
    }
}