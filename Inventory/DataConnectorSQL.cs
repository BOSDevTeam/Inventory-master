using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;

namespace Inventory
{
    public class DataConnectorSQL
    {
        private string CONNECTION_STRING;
        private SqlConnection con = null;

        public SqlConnection Connect()
        {
            CONNECTION_STRING = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"].ToString();
            try
            {
                if (con == null) con = new SqlConnection(CONNECTION_STRING);
                if (con.State == ConnectionState.Closed) con.Open();
                return con;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {

            }
        }

        public void Close()
        {
            con = null;
        }
    }
}