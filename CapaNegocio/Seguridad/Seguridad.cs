using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Management;

using CapaEntidad;
using CapaDatos;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Contexts;



using System.Web;

namespace CapaNegocio
{
    public class Seguridad
    {
        //propiedad objeto a la capa de datos
        private CapaDatos.SQLContext DContexto = new CapaDatos.SQLContext();

        //referencia a la capas de entidad
        private CapaEntidad.Usuario EUsuario = new CapaEntidad.Usuario();

        //referencia a las demas capas de negocio
        private CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();
        private CapaNegocio.Zona NZona = new CapaNegocio.Zona();

        private System.Web.HttpContext Context = System.Web.HttpContext.Current;

        //metodo que permite crear las sesiones, recibe como parametros correo y contraseña para comprobar la existencia del cliente
        //mientras existe y este active, las variables de sesion se instanciaran
        //retornar true cuando se haya creado la sesion
        public bool CrearSesiones(string _user, string _pass, string _ip)
        {
            bool flag = false;
            try{
                //verificar si ya existe variables de sesion creada
                if (Context.Session["User"] == null || Context.Session["Pass"] == null || Context.Session["IP"] == null){
                    //Obtener la informacion del usuario
                    EUsuario = this.VerificarCuenta(_user, _pass);
                    //crear las sesiones si el cliente existe y este activo
                    if (EUsuario != null && (EUsuario.Estado != 0)){
                        //Actualizar la tabla de sesiones de usuario y verificar que la variable de salida del proc. almacenado sea 1
                        if (Convert.ToBoolean(((IList<object>)DContexto.EjecutarProcedimiento("spr_UsuarioRegistrar_Sesion", 0, _user, 1, EUsuario.IP, ""))[0]) == true){
                            //crear variables de sesiones
                            Context.Session["User"] = EUsuario.Cuenta;
                            Context.Session["Pass"] = EUsuario.Contrasena;
                            Context.Session["Name"] = EUsuario.NombreCompleto;
                            Context.Session["State"] = EUsuario.Estado;
                            Context.Session["TypeUser"] = EUsuario.Tipo.IdTipo;
                            Context.Session["Zone"] = EUsuario.Area.IdZona;
                            Context.Session["Profile"] = EUsuario.Perfil.NombrePerfil;
                            Context.Session["IP"] = EUsuario.IP; //usar la ip donde se conectó
                            Context.Session["IdConection"] = ""; 
                            Context.Session["Log"] = "OK";

                            flag = true;
                        }
                    }
                    else{
                        flag = false;
                    }
                }
                else{
                    //sesiones ya creadas con anterioridad
                    return true;
                }

                return flag;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Metodo que destruirá la sesion existente
        /// </summary>
        public bool DestruirSession(string user = "", string ip = "")
        {
            bool flag = false;
            try{
                if (Convert.ToBoolean(((IList<object>)DContexto.EjecutarProcedimiento("spr_UsuarioRegistrar_Sesion", 0, user, 0, ip, ""))[0]) == true){
                    Context.Session["User"] = null;
                    Context.Session["Pass"] = null;
                    Context.Session["Name"] = null;
                    Context.Session["State"] = null;
                    Context.Session["Zone"] = null;
                    Context.Session["TypeUser"] = null;
                    Context.Session["IP"] = null;
                    Context.Session["IdConection"] = null;
                    Context.Session["Log"] = "Session destruida";

                    Context.Session.Abandon();
                    flag = true;

                }

                return flag;

            }
            catch (Exception){
                return false;
            }
        }





        /// <summary>
        /// metodo que permite verificar la sesion del usuario actual
        /// recibe como parametro la variable a evaluar vs la sesion activa
        /// </summary>
        /// <param name="_param">tipo de sesion</param>
        /// <param name="_value">valor de sesion a comparar</param>
        /// <returns></returns>
        public bool VerificarSesion(string _param, string _value)
        {

            try
            {
                if (((string)Context.Session["User"] == ""))
                    return false;


                switch (_param)
                {
                    case "User":
                        if ((string)Context.Session["User"] == _value) { return true; }
                        else { return false; }

                    case "Mac":
                        if ((string)Context.Session["Mac"] == _value) { return true; }
                        else { return false; }
                }

            }
            catch (Exception)
            {
                return false;
            }

            return false;

        }


        /// <summary>
        /// Obtiene la MAC fisica del equipo
        /// una obtenido los adaptadores 
        /// devolvera el adaptador de tipo ethernet
        /// </summary>
        /// <returns>mac</returns>
        public string ObtenerMAC()
        {
            string strMAC = "";
            string MAC = "";
            try{

                //obtiene todas los adaptadores/interfaces de red del equipo
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

                //recorrer todos los adaptadores de red
                foreach (NetworkInterface adapter in nics)
                {
                    //obtener las propiedades del adaptador
                    if (adapter.NetworkInterfaceType.ToString() == "Ethernet"){
                        MAC = "";
                        strMAC = adapter.GetPhysicalAddress().ToString();
                        char[] caracterMAC = strMAC.ToCharArray();
                        //agregar los ":"
                        for (int i = 0; i < caracterMAC.Length; i++){
                            if (i == 1 || i == 3 || i == 5 || i == 7 || i == 9)
                                MAC += caracterMAC[i] + ":";

                            else
                                MAC += caracterMAC[i];
                        }
                    }
                        
                }


                return MAC;
            }
            catch (Exception e){
                return e.Message;
            }
        }

        /// <summary>
        /// Metodo de VerificarCuenta
        /// </summary>
        /// <param name="_user">cuenta de usuario</param>
        /// <param name="_pass">ccontraseña de usuario</param>
        /// <returns>retornará un objeto con la informacion del usuario a buscar</returns>
        public CapaEntidad.Usuario VerificarCuenta(string _user, string _pass)
        {
            //Objeto que retornara la informacion del usuario
            CapaEntidad.Usuario _Usuario = new CapaEntidad.Usuario();
            //Datatable para recibir los registros encontrados en la bd
            DataTable dtUsuarios = new DataTable();
            try
            {
                //ejecutar la consulta y el resultado guardarlo en el datatable
                dtUsuarios = DContexto.ObtenerDatos("spr_UsuarioAcceso '" + _user + "', '" + _pass + "'").Tables[0];

                //verificar si existen datos
                if (dtUsuarios.Rows.Count > 0)
                {
                    _Usuario.CodEmpleado = dtUsuarios.Rows[0]["CodEmpleado"].ToString().Trim();
                    _Usuario.Cuenta = dtUsuarios.Rows[0]["Usuario"].ToString();
                    _Usuario.Contrasena = dtUsuarios.Rows[0]["Contrasena"].ToString();
                    _Usuario.NombreCompleto = dtUsuarios.Rows[0]["NombreCompleto"].ToString();
                    _Usuario.Estado = Convert.ToInt32(dtUsuarios.Rows[0]["Estado"]);
                    _Usuario.IP = dtUsuarios.Rows[0]["IP_Local"].ToString();

                    _Usuario.Perfil = new UsuarioPerfil
                    {
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



        //Obtener la informacion del usuario usando el del equipo
        public CapaEntidad.Usuario Obtener_por_IP(string ip)
        {
            CapaEntidad.Usuario _Usuario = new CapaEntidad.Usuario();
            //Datatable para recibir los registros encontrados en la bd
            DataTable dtUsuarios = new DataTable();
            try
            {
                //ejecutar la consulta y el resultado guardarlo en el datatable
                dtUsuarios = DContexto.ObtenerDatos("spr_ListaUsuario_IP '" + ip + "'").Tables[0];

                //verificar si existen datos
                if (dtUsuarios.Rows.Count > 0)
                {
                    _Usuario.CodEmpleado = dtUsuarios.Rows[0]["CodEmpleado"].ToString();
                    _Usuario.Cuenta = dtUsuarios.Rows[0]["Usuario"].ToString();
                    _Usuario.Contrasena = dtUsuarios.Rows[0]["Contrasena"].ToString();
                    _Usuario.NombreCompleto = dtUsuarios.Rows[0]["NombreCompleto"].ToString();
                    _Usuario.Estado = Convert.ToInt32(dtUsuarios.Rows[0]["Estado"]);
                    _Usuario.IP = dtUsuarios.Rows[0]["IP_Local"].ToString();

                    _Usuario.Perfil = new UsuarioPerfil
                    {
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



    }

    
}
