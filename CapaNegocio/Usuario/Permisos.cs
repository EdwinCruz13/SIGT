using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class Permisos
    {
        //propiedad objeto a la capa de datos
        private CapaDatos.SQLContext Access = new CapaDatos.SQLContext();


        //referencia a otras capas
        private CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();


        /// <summary>
        /// Metodo para obtener la lista de permisos del usuario
        /// </summary>
        /// <param name="_user">cuenta de usuario</param>
        /// <returns>la lista de permisos</returns>
        public List<CapaEntidad.UsuarioPermisos> ObtenerPermisos(string _user)
        {
            //Objeto que retornara la informacion del usuario
            List<CapaEntidad.UsuarioPermisos> permisos = new List<CapaEntidad.UsuarioPermisos>();
            //Datatable para recibir los registros encontrados en la bd
            DataTable dtPermisos = new DataTable();
            try{
                //ejecutar la consulta y el resultado guardarlo en el datatable
                dtPermisos = Access.ObtenerDatos("spr_UsuarioPermisos '" + _user + "'").Tables[0];

                //verificar si existen datos
                if (dtPermisos.Rows.Count > 0){
                    for (int i = 0; i < dtPermisos.Rows.Count; i++){
                        permisos.Add(new CapaEntidad.UsuarioPermisos{
                            Usuario = NUsuario.DetalleUsuario(dtPermisos.Rows[i]["Usuario"].ToString()),
                            Perfil = this.DetallePerfiles(Convert.ToInt32(dtPermisos.Rows[i]["IdPerfil"])),
                            Modulo = this.DetalleModulo(dtPermisos.Rows[i]["CodModulo"].ToString()),
                            Actividad = this.DetalleActividad(dtPermisos.Rows[i]["CodControl"].ToString())
                        });
                    }

                    return permisos;

                }

                else
                    return null;
            }
            catch (Exception){
                return null;
            }
        }

        /// <summary>
        /// Obtiene los controles asignados por perfil
        /// </summary>
        /// <param name="idPerfil">IdPerfil</param>
        /// <param name="modo">tipo de filtro, si es permiso retornara todos los persmiso asociad
        /// al perfil, si es usuario retornara los usuarios asociados al perfil</param>
        /// <returns></returns>
        public List<CapaEntidad.UsuarioPermisos> ObtenerPermisos(int idPerfil, string modo)
        {
            //Objeto que retornara la informacion del usuario
            List<CapaEntidad.UsuarioPermisos> permisos = new List<CapaEntidad.UsuarioPermisos>();
            //Datatable para recibir los registros encontrados en la bd
            DataTable dtPermisos = new DataTable();
            String strString = "";
            try
            {
                switch (modo){
                    case "Permisos": 
                        strString = "spr_UsuarioPerfiles_Permisos " + idPerfil;
                    break;

                    case "Usuarios":
                        strString = "spr_UsuarioPerfiles_Usuarios " + idPerfil;
                     break;
                }
                //ejecutar la consulta y el resultado guardarlo en el datatable
                dtPermisos = Access.ObtenerDatos(strString).Tables[0];

                //verificar si existen datos
                if (dtPermisos.Rows.Count > 0)
                {
                    for (int i = 0; i < dtPermisos.Rows.Count; i++)
                    {
                        permisos.Add(new CapaEntidad.UsuarioPermisos
                        {
                            Usuario = (dtPermisos.Columns.Contains("Usuario")) ? NUsuario.DetalleUsuario(dtPermisos.Rows[i]["Usuario"].ToString())  : null,
                            Perfil = this.DetallePerfiles(Convert.ToInt32(dtPermisos.Rows[i]["IdPerfil"])),
                            Modulo = (dtPermisos.Columns.Contains("CodModulo")) ? this.DetalleModulo(dtPermisos.Rows[i]["CodModulo"].ToString()) : null,
                            Actividad = (dtPermisos.Columns.Contains("CodControl")) ? this.DetalleActividad(dtPermisos.Rows[i]["CodControl"].ToString()) : null
                        });
                    }

                    return permisos;

                }

                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }



        /// <summary>
        /// permite obtener las actividades y modulo que posee el usuario activo del sistema
        /// recibe como parametro la sesion activa del usuario, la actividad a evaluar, si
        /// existe registro devolvera true, caso contrario false (asshhh pero que obvio, mr. obvio)
        /// </summary>
        /// <param name="usuario">usuario a comprarar</param>
        /// <param name="actividad">actividad a comprarar</param>
        /// <returns>bool</returns>
        public bool AccesoModulos(string usuario, string actividad)
        {
            DataTable dtAcceso = new DataTable();
            try
            {
                //obtener los datos en la consulta para verificar el acceso
                dtAcceso = Access.ObtenerDatos("spr_UsuarioModulo '" + usuario + "', '" + actividad + "'").Tables[0];
                //verificar si posee registros
                if (dtAcceso.Rows.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }


        /// <summary>
        /// Lista de perfiles de usuarios
        /// </summary>
        /// <returns>Lista de perfiles</returns>
        public List<CapaEntidad.UsuarioPerfil> ListaPerfiles()
        {
            List<CapaEntidad.UsuarioPerfil> perfiles = new List<CapaEntidad.UsuarioPerfil>();
            DataTable dtResult = new DataTable();

            try{
                dtResult = Access.ObtenerDatos("spr_UsuarioPerfiles").Tables[0];
                if (dtResult.Rows.Count > 0){
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        perfiles.Add(new CapaEntidad.UsuarioPerfil {
                            IdPerfil = Convert.ToInt32(dtResult.Rows[i]["IdPerfil"]),
                            NombrePerfil = dtResult.Rows[i]["NombrePerfil"].ToString(),
                            FechaCreacion = Convert.ToDateTime(dtResult.Rows[i]["FechaCreacion"]),
                            UsuarioGraba = dtResult.Rows[i]["UsuarioAsigna"].ToString(),
                            Estado = Convert.ToBoolean(dtResult.Rows[i]["Estado"]),
                            EstadoDescripcion = (Convert.ToBoolean(dtResult.Rows[i]["Estado"]) == true) ? "ACTIVO" : "INACTIVO"
                        });
                    }
                }
            }
            catch (Exception){
                return null;
            }

            return perfiles;
        }



        /// <summary>
        /// Informacion detallada del perfil
        /// </summary>
        /// <param name="id">Id de perfil a evaluar</param>
        /// <returns></returns>
        public CapaEntidad.UsuarioPerfil DetallePerfiles(int id)
        {
            CapaEntidad.UsuarioPerfil perfiles = new CapaEntidad.UsuarioPerfil();
            DataTable dtResult = new DataTable();

            try{
                dtResult = Access.ObtenerDatos("spr_UsuarioPerfiles " + id).Tables[0];
                if (dtResult.Rows.Count > 0){
                    for (int i = 0; i < dtResult.Rows.Count; i++){
                        perfiles.IdPerfil = Convert.ToInt32(dtResult.Rows[i]["IdPerfil"]);
                        perfiles.NombrePerfil = dtResult.Rows[i]["NombrePerfil"].ToString();
                        perfiles.FechaCreacion = Convert.ToDateTime(dtResult.Rows[i]["FechaCreacion"]);
                        perfiles.UsuarioGraba = dtResult.Rows[i]["UsuarioAsigna"].ToString();
                    }
                }
            }
            catch (Exception){
                return null;
            }

            return perfiles;
        }


        /// <summary>
        /// Lista de controles existentes
        /// </summary>
        /// <returns>lista de controles existentes</returns>
        public List<CapaEntidad.UsuarioControl> ListaControles()
        {
            List<CapaEntidad.UsuarioControl> controles = new List<UsuarioControl>();
            DataTable dtResult = new DataTable();

            try{
                dtResult = Access.ObtenerDatos("SELECT * FROM tblAcceso_Control ORDER BY CodModulo ASC").Tables[0];
                if (dtResult.Rows.Count > 0){
                    for (int i = 0; i < dtResult.Rows.Count; i++){
                        controles.Add(new CapaEntidad.UsuarioControl { 
                            CodActividad = dtResult.Rows[i]["CodControl"].ToString(),
                            Modulo = this.DetalleModulo(dtResult.Rows[i]["CodModulo"].ToString()),
                            DescripcionActividad = dtResult.Rows[i]["NombreActividad"].ToString()
                        });
                    }
                }
            }
            catch (Exception){
                return null;
            }

            return controles;
        }


        /// <summary>
        /// Obterner la informacion del un modulo
        /// </summary>
        /// <param name="id">modulo a detallar</param>
        /// <returns></returns>
        public CapaEntidad.UsuarioModulo DetalleModulo(string id)
        {
            CapaEntidad.UsuarioModulo modulo = new CapaEntidad.UsuarioModulo();
            DataTable dtResult = new DataTable();

            try
            {
                dtResult = Access.ObtenerDatos("SELECT * FROM tblAcceso_Modulo WHERE CodModulo = " + id).Tables[0];
                if (dtResult.Rows.Count > 0){
                    for (int i = 0; i < dtResult.Rows.Count; i++){
                        modulo.CodModulo = dtResult.Rows[i]["CodModulo"].ToString();
                        modulo.NombreModulo = dtResult.Rows[i]["NombreModulo"].ToString();
                        modulo.DescripcionModulo = dtResult.Rows[i]["Descripcion"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return modulo;
        }

        /// <summary>
        /// Obtiene el detalle de la activiidad
        /// </summary>
        /// <param name="id">actividad a evaliar</param>
        /// <returns></returns>
        public CapaEntidad.UsuarioControl DetalleActividad(string id)
        {
            CapaEntidad.UsuarioControl modulo = new CapaEntidad.UsuarioControl();
            DataTable dtResult = new DataTable();

            try
            {
                dtResult = Access.ObtenerDatos("SELECT * FROM tblAcceso_Control WHERE CodControl = '" + id + "'").Tables[0];
                if (dtResult.Rows.Count > 0)
                {
                    for (int i = 0; i < dtResult.Rows.Count; i++)
                    {
                        modulo.CodActividad = dtResult.Rows[i]["CodControl"].ToString();
                        modulo.DescripcionActividad = dtResult.Rows[i]["NombreActividad"].ToString();
                        modulo.Modulo = new UsuarioModulo
                        {
                            CodModulo = dtResult.Rows[i]["CodModulo"].ToString()
                        };
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return modulo;
        }


    }
}
