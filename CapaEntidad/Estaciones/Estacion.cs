using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    //Lista de estaciones existentes
    public class Estacion
    {
        public int IdEstacion { get; set; }
        public string IpLocal { get; set; }
        public string NombreEstacion { get; set; }
        public Zona Area { get; set; }
        public string Estado { get; set; }

        public string UsuarioP { get; set; } //cuando el usuario este preparado


    }
}
