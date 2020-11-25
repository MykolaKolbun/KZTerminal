using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KZ_Ingenico_EPI
{
    class SQLConnect
    {
        string connectionString;
        SqlConnection conn;
        SqlCommand cmd;
        string machineID;
        public SQLConnect(string machineID)
        {
            string compName = Environment.GetEnvironmentVariable("COMPUTERNAME");
            string srvName = compName.Split('-')[0] + "-01";
            this.machineID = machineID;
            //this.connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PARK_DB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            this.connectionString = string.Format(StringValue.SQLServerConnectionString, "10.10.50.1");
        }

        public void AddLinePurch(string devID, string trID, string tckNR, string rb, int amnt, string crdNR)
        {
            conn = new SqlConnection(connectionString);
            conn.Open();
            cmd = new SqlCommand("AddTransaction", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DeviceID", devID);
            cmd.Parameters.AddWithValue("@TransactionType", 1);
            cmd.Parameters.AddWithValue("@TransactionNR", trID);
            cmd.Parameters.AddWithValue("@ReceiptBody", rb);
            cmd.Parameters.AddWithValue("@Amount", amnt);
            cmd.Parameters.AddWithValue("@TicketNR", tckNR);
            cmd.Parameters.AddWithValue("@CardNR", crdNR);
            cmd.Parameters.AddWithValue("@IsPrinted", 0);
            cmd.Parameters.AddWithValue("@VisaDiscount", 0);
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                cmd.Dispose();
            }
            catch
            {
                //MessageBox.Show("Terminal: " + e.Message);
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
            }
        }

        public void AddLinePurch(string devID, string trID, string tckNR, string rb, int amnt, string crdNR, bool discount)
        {
            conn = new SqlConnection(connectionString);
            conn.Open();
            cmd = new SqlCommand("AddTransaction", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DeviceID", devID);
            cmd.Parameters.AddWithValue("@TransactionType", 1);
            cmd.Parameters.AddWithValue("@TransactionNR", trID);
            cmd.Parameters.AddWithValue("@ReceiptBody", rb);
            cmd.Parameters.AddWithValue("@Amount", amnt);
            cmd.Parameters.AddWithValue("@TicketNR", tckNR);
            cmd.Parameters.AddWithValue("@CardNR", crdNR);
            cmd.Parameters.AddWithValue("@IsPrinted", 0);
            cmd.Parameters.AddWithValue("@VisaDiscount", 1);
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                cmd.Dispose();
            }
            catch
            {
                //MessageBox.Show("Terminal: " + e.Message);
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
            }
        }

        public void AddLineToVisaTable(string devID, string devName, string trID, string tckNR, string rb, int amnt, string crdNR, int discount, string rrn)
        {
            conn = new SqlConnection(connectionString);
            conn.Open();
            cmd = new SqlCommand("AddVisaTransaction", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DeviceID", devID);
            cmd.Parameters.AddWithValue("@DeviceName", devName);
            cmd.Parameters.AddWithValue("@TransactionNR", trID);
            cmd.Parameters.AddWithValue("@ReceiptBody", rb);
            cmd.Parameters.AddWithValue("@Amount", amnt);
            cmd.Parameters.AddWithValue("@TicketNR", tckNR);
            cmd.Parameters.AddWithValue("@CardNR", crdNR);
            cmd.Parameters.AddWithValue("@Discount", discount);
            cmd.Parameters.AddWithValue("@RRN", rrn);
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                cmd.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show("Terminal: " + e.Message);
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
            }
        }

        //public void AddLineDecline(string trID, string tckNR, string rb, int amnt, string crdNR)
        //{
        //    conn = new SqlConnection(connectionString);
        //    conn.Open();
        //    cmd = new SqlCommand("AddTransaction", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@DeviceID", machineID);
        //    cmd.Parameters.AddWithValue("@TransactionType", 1);
        //    cmd.Parameters.AddWithValue("@TransactionNR", trID);
        //    cmd.Parameters.AddWithValue("@ReceiptBody", rb);
        //    cmd.Parameters.AddWithValue("@Amount", amnt);
        //    cmd.Parameters.AddWithValue("@TicketNR", tckNR);
        //    cmd.Parameters.AddWithValue("@CardNR", crdNR);
        //    cmd.Parameters.AddWithValue("@IsPrinted", 0);
        //    try
        //    {
        //        SqlDataReader reader = cmd.ExecuteReader();
        //        reader.Close();
        //        cmd.Dispose();
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("Terminal: " + e.Message);
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        public void AddLineSettlement(string trID, string rb)
        {
            conn = new SqlConnection(connectionString);
            conn.Open();
            cmd = new SqlCommand("AddTransaction", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DeviceID", machineID);
            cmd.Parameters.AddWithValue("@TransactionType", 2);
            cmd.Parameters.AddWithValue("@TransactionNR", trID);
            cmd.Parameters.AddWithValue("@ReceiptBody", rb);
            cmd.Parameters.AddWithValue("@Amount", 0);
            cmd.Parameters.AddWithValue("@TicketNR", " ");
            cmd.Parameters.AddWithValue("@CardNR", " ");
            cmd.Parameters.AddWithValue("@IsPrinted", 0);
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Close();
                cmd.Dispose();
            }
            catch
            {
                //MessageBox.Show("Terminal: " + e.Message);
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
            }
        }

        /// <summary>
        /// Check connection to SQL instance
        /// </summary>
        /// <returns>connection state</returns>
        public bool IsSQLOnline()
        {
            bool isOnline = false;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();
                isOnline = true;
                //conn.Close();
            }
            catch (SqlException)
            {
                isOnline = false;
            }
            return isOnline;
        }
    }
}
