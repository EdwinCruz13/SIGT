using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class AreaRendimiento
    {
        public string area { get; set; }
        public List<List<UsuarioRendimiento>> RendimientoUsuario { get; set; }
    }
}
