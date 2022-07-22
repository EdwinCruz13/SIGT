using System;
using System.ComponentModel.DataAnnotations;

namespace CapaEntidad
{
    //clase que instanciará la informacion del usuario
    //que hace uso del sistema: cajero, monitores y analistas
    public class Usuario
    {
        public string Cuenta { get; set; }
        public string Contrasena { get; set; }
        public string CodEmpleado { get; set; }
        public string CedulaIdentidad { get; set; }
        public string NombreCompleto { get; set; }
        public UsuarioPerfil Perfil { get; set; }
        public Zona Area { get; set; }
        public UsuarioTipo Tipo { get; set; }
        public int Estado { get; set; }
        public string IP { get; set; }  //de donde se conecta a atender turno
        public string IdConnection { get; set; }
    }
}
