using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class PivoteRecuperaciones_Tickets
    {
        public String Area { get; set; }
        public int Cancelacion { get; set; }
        public int TramiteReembolso { get; set; }
        public int CancelacionHipoteca { get; set; }
        public int Liquidacion { get; set; }
        public int Otros { get; set; }

        public int TotalAdquirido { get; set; }
    }
}
