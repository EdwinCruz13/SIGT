using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    //Usuarios conectados a la estacion de trabajo
    public class EstacionTrabajo
    {
        public int nAsignacion { get; set; }
        public string Cuenta { get; set; }

        // referencia a la ticket
        public string CodTicket { get; set; }
        public string Ticket { get; set; }
        public string Institucion { get; set; }
        public Ticket Turno { get; set; }

        // referencia al usuario que utiliza la estacion
        public Usuario Usuario { get; set; }

        // estado de la estacion
        public int Estado { get; set; }
        public string FechaAsignacion { get; set; }
        public int NAtendidos { get; set; }
        public string NombreEstacion { get; set; }
        public string IpEstacion { get; set; }
        public int CodEstacion { get; set; }

        //tipo de activacion
        public int IdAsignacion { get; set; }
        public string TipoAsignacion { get; set; }

        //sesion activa
        public string ConnectionID { get; set; }


    }
}
