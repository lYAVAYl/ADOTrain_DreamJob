using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DreamJobConnectedLayer
{
    public class StaffDAL
    {
        private SqlConnection connect = null;

        public void OpenConnection(string connectionString)
        {
            connect = new SqlConnection(connectionString);
            connect.Open();
        }

        public void CloseConnection()
        {
            connect.Close();
        }

        public void InsertWorker(int WId, string WName)
        {
            string sql = string.Format($"INSERT INTO STAFF (WorkerID, WorkerName)" +
                                       $"VALUES (@WorkerID, @WorkerName)");

            using (SqlCommand cmd = new SqlCommand(sql, this.connect))
            {
                cmd.Parameters.AddWithValue("@WorkerID", WId);
                cmd.Parameters.AddWithValue("@WorkerName", WName);

                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetStaffAndSalary(string sql)
        {
            DataTable inv = new DataTable();

            using (SqlCommand cmd = new SqlCommand(sql, this.connect))
            {
                SqlDataReader dr = cmd.ExecuteReader();

                inv.Load(dr);
                dr.Close();
            }
            
            return inv;
        }
    }
}
