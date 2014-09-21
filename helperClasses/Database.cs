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
        private static SqlConnection conn;

        public static void Query(string sqlStr, List<SqlParameter> parameters)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnection"].ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    if (parameters.Count > 0)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.ParameterName, param.Value);
                        }
                    }
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                conn.Dispose();
            }
        }
        public static void Query(string sqlStr)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnection"].ConnectionString)) {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sqlStr, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
                conn.Dispose();
            }
        }
        public static DataTable GetAsDataTable(string sqlStr)
        {
            var table = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnection"].ConnectionString))
            {
                conn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlStr, conn))
                {
                    table = new DataTable();
                    
                    da.Fill(table);
                }

                conn.Close();
                conn.Dispose();
            }
            return table;
        }
        public static DataTable GetAsDataTable(string sqlStr, SqlParameterCollection parameters)
        {
            var table = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnection"].ConnectionString))
            {
                conn.Open();

                using (SqlDataAdapter da = new SqlDataAdapter(sqlStr, conn))
                {
                    table = new DataTable();

                    da.Fill(table);
                }

                conn.Close();
                conn.Dispose();
            }
            return table;
        }
        public static string DataTable_To_JSON(DataTable dt)
        {
            return JsonConvert.SerializeObject(dt, Formatting.None);
        }
    }
}