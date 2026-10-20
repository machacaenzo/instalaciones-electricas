using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dao
{
    public class ConnectionToDB
    {
        private readonly String connectionString;
        public ConnectionToDB()
        {
            connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\CAMILA\\Desktop\\TP_FINAL_LPOOII\\BDD\\BDD_InstalacionesElectricas.mdf;Integrated Security=True;Connect Timeout=30";
        }
        protected SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
