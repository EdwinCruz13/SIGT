using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Cliente
    {
        // Datos de la cuenta
        public string IdCliente { get; set; }
        public string Cedula { get; set; }
        public string CodEmpleado { get; set; }
        public string NoAsegurado { get; set; }
   
        // Datos generales
        public string NombreCompleto { get; set; }

        // Datos Institucionales
        public string Id_Institucion { get; set; }
        public string NombreInstitucion { get; set; }
        public string FechaIngreso { get; set; }
        public string Cargo { get; set; }
        public string Ocupacion { get; set; }

        // Datos de afiliacion
        public string Id_TipoPension { get; set; }
        public string TipoPension { get; set; }

        //Otros datos
        
        public string Pasaporte { get; set; }
        public string CodFiscal { get; set; }
        public string Sexo { get; set; }
        public int TipoCliente { get; set; } //si existe en inversiones o no
        public string DCliente { get; set; }
        public int Estado { get; set; }
    }
}
