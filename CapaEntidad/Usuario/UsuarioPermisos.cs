using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;

namespace CapaEntidad
{

    /// <summary>
    /// entidad que permite controlar los permisos del usuario
    /// </summary>
    public class UsuarioPermisos
    {
        public Usuario Usuario { get; set; }
        public UsuarioModulo Modulo { get; set; }
        public UsuarioControl Actividad { get; set; }
        public UsuarioPerfil Perfil { get; set; }
    }
}
