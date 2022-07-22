using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using CapaEntidad;
using CapaDatos;

namespace CapaNegocio
{
    public class Usuario
    {
        //Crear la referencia a la capa de negocio con un objeto
        CapaDatos.SQLContext Access = new CapaDatos.SQLContext();

        //referencia a las demas capas de negocio
        CapaNegocio.Zona NZona = new CapaNegocio.Zona();

        /// <summary>
        /// Metodo de ListarUsuario
        /// </summary>
        /// <returns>Lista con la informacion de los usuarios registrados</returns>
        public List<CapaEntidad.Usuario> ListarUsuarios()
        { 
            //crear lista para retornar
            List<CapaEntidad.Usuario> ListaUsuario = new List<CapaEntidad.Usuario>();

            //Datatable para recibir los registros encontrados en la bd
            DataTable dtUsuarios = new DataTable();
            try{
                //ejecutar la consulta y el resultado guardarlo en el datatable
                dtUsuarios = Access.ObtenerDatos("EXEC spr_UsuarioLista").Tables[0];

                //verificar si existen datos
                if (dtUsuarios.Rows.Count > 0){
                    //recorrer el datatable y guardar los elemento a la lista
                    for (int i = 0; i < dtUsuarios.Rows.Count; i++){
                        ListaUsuario.Add(new CapaEntidad.Usuario
                        {
                            CodEmpleado = dtUsuarios.Rows[i]["CodEmpleado"].ToString().Trim(),
                            Cuenta = dtUsuarios.Rows[i]["Usuario"].ToString(),
                            CedulaIdentidad = dtUsuarios.Rows[i]["Cedula"].ToString(),
                            Contrasena = dtUsuarios.Rows[i]["Contrasena"].ToString(),
                            NombreCompleto = dtUsuarios.Rows[i]["NombreCompleto"].ToString(),
                            Estado = Convert.ToInt32(dtUsuarios.Rows[i]["Estado"]),
                            //Perfil Asignado
                            Perfil = new UsuarioPerfil {
                                IdPerfil = Convert.ToInt32(dtUsuarios.Rows[i]["IdPerfil"]),
                                NombrePerfil = dtUsuarios.Rows[i]["NombrePerfil"].ToString(),
                                UsuarioGraba = dtUsuarios.Rows[i]["UsuarioGraba"].ToString(),
                                FechaCreacion = Convert.ToDateTime(dtUsuarios.Rows[i]["FechaCreacion"])
                            },
                            //Area asignada
                            Area = NZona.DetalleZona(dtUsuarios.Rows[i]["IdZona"].ToString()),

                            Tipo = new UsuarioTipo{
                                IdTipo = Convert.ToInt32(dtUsuarios.Rows[i]["IdTipo"]),
                                TipoUsuario = dtUsuarios.Rows[i]["TipoUsuario"].ToString()
                            },


                            IP = dtUsuarios.Rows[i]["IP_Local"].ToString()
                        });
                    }

                    return ListaUsuario;
                }
                else
                    return null;
            }
            catch (Exception){
                return null;
            }
        }

        /// <summary>
        /// Metodo de DetalleUsuario
        /// </summary>
        /// <param name="_user">cuenta de usuario</param>
        /// <returns>retornará un objeto con la informacion del usuario a buscar</returns>
        public CapaEntidad.Usuario DetalleUsuario(string _user)
        {
            //Objeto que retornara la informacion del usuario
            CapaEntidad.Usuario _Usuario = new CapaEntidad.Usuario();
            //Datatable para recibir los registros encontrados en la bd
            DataTable dtUsuarios = new DataTable();
            try
            {
                //ejecutar la consulta y el resultado guardarlo en el datatable
                dtUsuarios = Access.ObtenerDatos("spr_UsuarioLista '" + _user + "'").Tables[0];

                //verificar si existen datos
                if (dtUsuarios.Rows.Count > 0)
                {
                  _Usuario.CodEmpleado = dtUsuarios.Rows[0]["CodEmpleado"].ToString().Trim();
                  _Usuario.CedulaIdentidad = dtUsuarios.Rows[0]["Cedula"].ToString();
                    _Usuario.Cuenta = dtUsuarios.Rows[0]["Usuario"].ToString();
                  _Usuario.Contrasena = dtUsuarios.Rows[0]["Contrasena"].ToString();
                  _Usuario.NombreCompleto = dtUsuarios.Rows[0]["NombreCompleto"].ToString();
                  _Usuario.Estado = Convert.ToInt32(dtUsuarios.Rows[0]["Estado"]);
                  _Usuario.IP = dtUsuarios.Rows[0]["IP_Local"].ToString(); //IP LOCAL

                  _Usuario.Perfil = new UsuarioPerfil{
                      IdPerfil = Convert.ToInt32(dtUsuarios.Rows[0]["IdPerfil"]),
                      NombrePerfil = dtUsuarios.Rows[0]["NombrePerfil"].ToString(),
                      UsuarioGraba = dtUsuarios.Rows[0]["UsuarioGraba"].ToString(),
                      FechaCreacion = Convert.ToDateTime(dtUsuarios.Rows[0]["FechaCreacion"])
                  };



                  _Usuario.Area = NZona.DetalleZona(dtUsuarios.Rows[0]["IdZona"].ToString());

                  _Usuario.Tipo = new UsuarioTipo
                  {
                      IdTipo = Convert.ToInt32(dtUsuarios.Rows[0]["IdTipo"]),
                      TipoUsuario = dtUsuarios.Rows[0]["TipoUsuario"].ToString()
                  };


                  return _Usuario; 
                          
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
        /// Informacion del usuario a traves de su IP
        /// </summary>
        /// <param name="IP">IP a Valorar</param>
        /// <returns></returns>
        public CapaEntidad.Usuario DetalleUsuarioIP(string IP)
        {
            //Objeto que retornara la informacion del usuario
            CapaEntidad.Usuario _Usuario = new CapaEntidad.Usuario();
            //Datatable para recibir los registros encontrados en la bd
            DataTable dtUsuarios = new DataTable();
            try{
                //ejecutar la consulta y el resultado guardarlo en el datatable
                dtUsuarios = Access.ObtenerDatos("spr_UsuarioListaIP '" + IP + "'").Tables[0];

                //verificar si existen datos
                if (dtUsuarios.Rows.Count > 0){
                    _Usuario.CodEmpleado = dtUsuarios.Rows[0]["CodEmpleado"].ToString();
                    _Usuario.CedulaIdentidad = dtUsuarios.Rows[0]["Cedula"].ToString();
                    _Usuario.Cuenta = dtUsuarios.Rows[0]["Usuario"].ToString();
                    _Usuario.Contrasena = dtUsuarios.Rows[0]["Contrasena"].ToString();
                    _Usuario.NombreCompleto = dtUsuarios.Rows[0]["NombreCompleto"].ToString();
                    _Usuario.Estado = Convert.ToInt32(dtUsuarios.Rows[0]["Estado"]);
                    _Usuario.IP = dtUsuarios.Rows[0]["IP_Local"].ToString(); //IP LOCAL

                    _Usuario.Perfil = new UsuarioPerfil{
                        IdPerfil = Convert.ToInt32(dtUsuarios.Rows[0]["IdPerfil"]),
                        NombrePerfil = dtUsuarios.Rows[0]["NombrePerfil"].ToString(),
                        UsuarioGraba = dtUsuarios.Rows[0]["UsuarioGraba"].ToString(),
                        FechaCreacion = Convert.ToDateTime(dtUsuarios.Rows[0]["FechaCreacion"])
                    };
                    _Usuario.Area = NZona.DetalleZona(dtUsuarios.Rows[0]["IdZona"].ToString());
                    _Usuario.Tipo = new UsuarioTipo{
                        IdTipo = Convert.ToInt32(dtUsuarios.Rows[0]["IdTipo"]),
                        TipoUsuario = dtUsuarios.Rows[0]["TipoUsuario"].ToString()
                    };


                    return _Usuario;

                }

                else
                    return null;
            }
            catch (Exception){
                return null;
            }
        }

        /// <summary>
        /// Metodo de Crear usuario
        /// </summary>
        /// <param name="_usuario">cuenta de usuario</param>
        /// <returns>booleano true o false si completo la acción</returns>
        public bool CrearUsuario(CapaEntidad.Usuario _usuario)
        {
            //Declarar variable de salida
            object Resultado = new object();
            IList<object> collection;
            int input = 0;
            int Salida = 0;


            try
            {
                //Crear un nuevo afiliado en bases de datos WebIssdhu
                Resultado = Access.EjecutarProcedimiento("spr_UsuarioCrear", input, _usuario.Cuenta.ToUpper(), _usuario.Contrasena, _usuario.CodEmpleado, _usuario.CedulaIdentidad.ToUpper(), _usuario.NombreCompleto, _usuario.Perfil.IdPerfil, _usuario.Area.IdZona, _usuario.IP);
                //Convertir a lista para mayor control del Object -_-
                collection = (IList<object>)Resultado;
                Salida = Convert.ToInt32(collection[0]);
                //Verificar el resultado
                if (Salida == 0)
                    return false;

            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        /// <summary>
        /// Metodo de Editar
        /// </summary>
        /// <param name="_usuario">cuenta de usuario</param>
        /// <returns>booleano true o false si completo la acción</returns>
        public bool EditarUsuario(CapaEntidad.Usuario _usuario)
        {
            //Declarar variable de salida
            object Resultado = new object();
            IList<object> collection;
            int input = 0;
            int Salida = 0;


            try{
                //Crear un nuevo afiliado en bases de datos WebIssdhu
                Resultado = Access.EjecutarProcedimiento("spr_UsuarioEditar", input, _usuario.Cuenta, _usuario.Contrasena, _usuario.CodEmpleado, _usuario.CedulaIdentidad.ToUpper(), _usuario.NombreCompleto, _usuario.Perfil.IdPerfil, _usuario.Area.IdZona, _usuario.Estado, _usuario.IP);
                //Convertir a lista para mayor control del Object -_-
                collection = (IList<object>)Resultado;
                Salida = Convert.ToInt32(collection[0]);
                //Verificar el resultado
                if (Salida == 0)
                    return false;

            }
            catch (Exception ex){
                return false;
            }

            return true;

        }


        /// <summary>
        /// Metodo de habilitar o inhabilitar cuenta de usuario
        /// </summary>
        /// <param name="_usuario">cuenta de usuario</param>
        /// <returns>booleano true o false si completo la acción</returns>
        public bool HabilitarUsuario(string _usuario, int proceso)
        {
            //Declarar variable de salida
            object Resultado = new object();
            IList<object> collection;
            int input = 0;
            int Salida = 0;


            try
            {
                //Crear un nuevo afiliado en bases de datos WebIssdhu
                Resultado = Access.EjecutarProcedimiento("spr_UsuarioActualizar_Estado", input, _usuario, proceso);
                //Convertir a lista para mayor control del Object -_-
                collection = (IList<object>)Resultado;
                Salida = Convert.ToInt32(collection[0]);
                //Verificar el resultado
                if (Salida == 0)
                    return false;

            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        /// <summary>
        /// Obtiene la lista de tipo de usuarios existentes
        /// </summary>
        /// <returns></returns>
        public List<UsuarioTipo> ListaTipo_Usuario()
        {
            DataTable dt = new DataTable();
            List<UsuarioTipo> list = new List<UsuarioTipo>();

            try{
                dt = Access.ObtenerDatos("SELECT * FROM tblUsuario_Tipo").Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++){
                    list.Add(new UsuarioTipo
                    {
                        IdTipo = Convert.ToInt32(dt.Rows[i]["IdTipo"]),
                        TipoUsuario = dt.Rows[i]["TipoUsuario"].ToString()
                    });
                }
            }
            catch (Exception) {

                return null;
            }
           

            return list;
        }






    }
}
