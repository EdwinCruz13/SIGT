using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{

    /// <summary>
    /// entidad que guardara en sus propiedades las cantidades de tickets que 
    /// sera atendidos en los tiempos definidos
    /// </summary>
    public class TiempoAtencion_Area
    {
        public string IdArea { get; set; }
        public string Area { get; set; }
        public int min0_10 { get; set; }
        public int min10_30 { get; set; }
        public int min30_60 { get; set; }
        public int min60_max { get; set; }
        public int TotalTickets { get; set; }
    }
}
