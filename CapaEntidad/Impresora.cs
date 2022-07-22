using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Impresora
    {
        public int Id { get; set; }
        public String Descripcion { get; set; }
        public String Mac { get; set; }
        public String IP { get; set; }
        public Boolean Status { get; set; }
    }
}
