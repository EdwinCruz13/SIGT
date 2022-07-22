using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{

    /// <summary>
    /// Entidad para las alertas
    /// de las estaciones a los responsables
    /// solicitando cieerre de estacion
    /// </summary>
    public class Alertas
    {
        public int ID { get; set; }
        public DateTime Fecha { get; set; }
        public string Mensage { get; set; }
        public string TipoAlerta { get; set; }
        public int Consecutivo { get; set; }


        //referencia a la estacion, al usuario y al ticket asignado
        public CapaEntidad.EstacionTrabajo EstacionTrabajo { get; set; }
        
    }

}
