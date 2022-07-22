using System;
using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    public class UsuarioPerfil
    {
        public int IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string UsuarioGraba { get; set; }
        public Boolean Estado { get; set; }
        public string EstadoDescripcion { get; set; }
    }
}
