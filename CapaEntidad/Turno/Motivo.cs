using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Motivo
    {
        public int IdMotivo { get; set; }
        public Zona Zona { get; set; }
        public string Descripcion { get; set; }
        public int Prioridad { get; set; }
        public bool Asignado { get; set; }


        //tiempo promedio
        public int Hora { get; set; }
        public int Minuto { get; set; }
        public int Segundos { get; set; }
    }
}
