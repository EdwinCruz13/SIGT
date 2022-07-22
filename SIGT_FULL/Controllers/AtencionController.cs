using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;

namespace SIGT.Controllers
{
    public class AtencionController : Controller
    {

        //referencia a la capa de negocio usuario
        private CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();
        //referencia a controlador seguridad
        private CapaNegocio.Seguridad seguridad = new CapaNegocio.Seguridad();
        //Alertas de estaciones
        private CapaNegocio.Alertas NNotificaciones = new CapaNegocio.Alertas();
        //retornar la vista con la lista de estaciones
        private CapaNegocio.Estaciones NEstaciones = new CapaNegocio.Estaciones();



        //sesiones activas
        private System.Web.HttpContext Context = System.Web.HttpContext.Current;

        
        // <summary>
        /// Metodo ger para Modulo de Atencion
        /// </summary>
        /// <returns>ActionResult</returns>
        /// retornará su vista correspondiente
        public ActionResult ModuloAtencion(string usuario)
        {
            try{
                //verificar si existe la variable de sesion
                if (Context.Session["User"] == null){
                    return RedirectToAction("Logout", "Home");
                }

                //verificar si el usuario enviado por get coincide con la sesion
                //si omite la condicion, esta todo bien, la solicitud get vs sesion activa es correcta
                //En este punto, rediccionara al controlador administrador o al controlador usuario(cliente)
                if (seguridad.VerificarSesion("User", usuario) == false){
                    return RedirectToAction("Logout", "Home");
                }

                //En este punto, rediccionara al controlador administrador o al controlador usuario(cliente)
                if ((int)Context.Session["TypeUser"] == 0){
                    return RedirectToAction("Index", "Monitor");
                }

                
                //Obtener la informacion del usuario a partir de la variable de sesion y retornar la vista
                //verificar valores nulos
                CapaEntidad.EstacionTrabajo estaciones = new CapaEntidad.EstacionTrabajo();
                estaciones = NEstaciones.DetalleEstacionesActiva(usuario, Context.Session["IP"].ToString());


                if (estaciones.CodTicket == null || estaciones.CodTicket == "") {
                    //instanciar los objetos internos al objeto estaciones
                    estaciones.Turno = new CapaEntidad.Ticket();
                    estaciones.Turno.Motivo = new CapaEntidad.Motivo();
                    estaciones.Turno.Motivo.Zona = new CapaEntidad.Zona();
                    estaciones.Turno.Cliente = new CapaEntidad.Cliente();
                    estaciones.Usuario = new CapaEntidad.Usuario();
                    estaciones.Usuario.Area = new CapaEntidad.Zona();

                    //asignar valores
                    estaciones.Turno.Motivo.Zona.IdZona = "";
                    estaciones.Turno.Motivo.Descripcion = "";
                    estaciones.Turno.Cliente.IdCliente = "";
                    estaciones.Turno.Cliente.NombreCompleto = "";
                    estaciones.Institucion = "";
                    estaciones.NombreEstacion = "Estación Inactiva";
                }

                ViewBag.User = usuario;
                return View(estaciones);


            }
            catch (Exception){
                return RedirectToAction("Logout", "Home");
            }
        }


        // <summary>
        /// Metodo ger para Modulo de ListaEstacion
        /// Retornara la lista de estaciones de un area segun el usuario que consulta
        /// </summary>
        /// <returns>ActionResult</returns>
        /// retornará su vista correspondiente
        public ActionResult ListaEstacion(string usuario)
        {
            try{
                //verificar si existe la variable de sesion
                if (Context.Session["User"] == null){
                    return RedirectToAction("Logout", "Home");
                }

                //verificar si el usuario enviado por get coincide con la sesion
                //si omite la condicion, esta todo bien, la solicitud get vs sesion activa es correcta
                //En este punto, rediccionara al controlador administrador o al controlador usuario(cliente)
                if (seguridad.VerificarSesion("User", usuario) == false) {
                    return RedirectToAction("Logout", "Home");
                }

                //En este punto, rediccionara al controlador administrador o al controlador usuario(cliente)
                if ((int)Context.Session["TypeUser"] == 0){
                    return RedirectToAction("Index", "Monitor");
                }

                //obtener la lista de estaciones segun la zona del usuario que consulta
                List<CapaEntidad.Estacion> estacionesActivas = new List<CapaEntidad.Estacion>();
                //List<CapaEntidad.Estacion> lista = new List<CapaEntidad.Estacion>();

                //Obtene la listas de las estaciones existentes
                estacionesActivas = NEstaciones.ListaEstacionesActiva();

                //devolver solo la lista de estaciones correspondiente a un area //usar linq
                List<CapaEntidad.Estacion> filtro = (Session["Zone"].ToString() == "01") ? estacionesActivas.Where(x => x.Area.IdZona == "03" || x.Area.IdZona == "01").ToList() : estacionesActivas.Where(x => x.Area.IdZona == Session["Zone"].ToString()).ToList();
                //List<CapaEntidad.Estacion> filtro = (Session["Zone"].ToString() == "01") ? (from x in estacionesActivas where x.Area.IdZona == "03" || x.Area.IdZona == "01" select x).ToList() :  (from y in estacionesActivas where y.Area.IdZona == Session["Zone"].ToString() select y).ToList();

                return View(filtro);
            }
            catch (Exception){
                return RedirectToAction("Logout", "Home");
            }
        }

        /// <summary>
        /// Vista parcial que permite cargar la informacion
        /// de las notificaciones de las estaciones
        /// obtendrá en la base todos los registros
        /// </summary>
        /// <returns>ActionResult</returns>
       public ActionResult _ListaNotificaciones()
       {
           //Obtener la lista de notificaciones
           List<CapaEntidad.Alertas> notificaciones = new List<CapaEntidad.Alertas>();
           notificaciones = NNotificaciones.ListaNotificaciones(Session["User"].ToString());

           //retornar a la vista parcial la lista de notificaciones
           return PartialView("_ListaNotificaciones", notificaciones);
       }
 
	}
}