using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class Configuraciones
    {
        //protected string strSigh = "Data Source = .; Initial Catalog = SIGT; User Id = sa; Password = 123456; Connect Timeout=30000";

        //Cadena de Conexión al Server 1.4
        protected string strSigh = "Data Source = 192.168.101.4; Initial Catalog = Sigt; User Id = Sigt; Password = jajajaja; Connect Timeout=30000";

        //Se utiliza como interface a fin de poder utilizarla en cualquier momento
        protected SqlConnection iConnection;
    }
}
