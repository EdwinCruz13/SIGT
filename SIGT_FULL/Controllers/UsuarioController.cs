using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CapaEntidad;
using CapaNegocio;
using Newtonsoft.Json;

namespace SIGT.Controllers
{

    public class UsuarioController : Controller
    {

        //Referencia a la capa de negocio
        private CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();
        private CapaNegocio.Turno NTickets = new CapaNegocio.Turno();
        
        //referencia a la capa de negocio seguridad
        private CapaNegocio.Seguridad seguridad = new CapaNegocio.Seguridad();
        //referencia a la capa de negocio zona
        private CapaNegocio.Zona NZona = new CapaNegocio.Zona();


        //sesiones activas
        private System.Web.HttpContext Context = System.Web.HttpContext.Current;


        /// <summary>
        /// Mostrar la pagina principal cuando el
        /// el usuario se logee con exito
        /// </summary>
        /// <returns>ActionResult</returns>
        public ActionResult Index(string usuario)
        {
            try{
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

                ////Obtener la lista de tickets del en general y guardarlo en un viewbag
                List<CapaEntidad.Ticket> ListaTicket = new List<Ticket>();
                ViewBag.ListaTicket = new CapaNegocio.Reportes().ListaTicket();


                // ammm? why did i do twice work?
                //List<CapaEntidad.Ticket> ListaTicket2 = new List<Ticket>();
                //ListaTicket2 = new CapaNegocio.Turno().ListaTicket();re



                return View(NUsuario.DetalleUsuario(usuario));

            }
            catch (Exception){
                return RedirectToAction("Logout", "Home");
            }

        }









        ///<summary>
        ///Obtiene la lista de usuarios existente
        ///<returns>ActionResult</returns>
        ///recibe el usuario logeado
        ///</summary>
        public ActionResult ListaUsuario(string usuario)
        {

            try
            {
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


                //obtener la lista de usuarios
                List<CapaEntidad.Usuario> objLista = new List<CapaEntidad.Usuario>();
                objLista = NUsuario.ListarUsuarios();
                return View(objLista);
            }



            catch (Exception)
            {
                return RedirectToAction("Logout", "Home");
            }

        }



        ///<summary>
        ///Obtiene la informacion detallada del usuario
        ///<returns>ActionResult</returns>
        ///recibe el usuario logeado
        ///</summary>
        public JsonResult DetalleUsuario(string usuario)
        {

            try{
                //usar la capa de negocio para obtener la informacion
                //del usuario
                return Json(NUsuario.DetalleUsuario(usuario), JsonRequestBehavior.AllowGet);
            }

            catch (Exception){
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        ///<summary>
        ///muestra la vista de creaccion de usuarios
        ///al crear el usuarios, se creará la estacion
        ///<returns>ActionResult</returns>
        ///recibe el usuario
        ///</summary>
        public ActionResult CrearUsuario()
        {
            try
            {
                //verificar si existe la variable de sesion
                if (Context.Session["User"] == null)
                {
                    return RedirectToAction("Logout", "Home");
                }

                //verificar si el usuario enviado por get coincide con la sesion
                //si omite la condicion, esta todo bien, la solicitud get vs sesion activa es correcta
                //En este punto, rediccionara al controlador administrador o al controlador usuario(cliente)
                if (seguridad.VerificarSesion("User", Session["User"].ToString()) == false)
                {
                    return RedirectToAction("Logout", "Home");
                }

                //En este punto, rediccionara al controlador administrador o al controlador usuario(cliente)
                if ((int)Context.Session["TypeUser"] == 0)
                {
                    return RedirectToAction("Index", "Monitor");
                }


                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        ///<summary>
        ///METODO POST Para crear usuario
        ///al crear el usuarios, se creará la estacion
        ///<returns>ActionResult</returns>
        ///recibe el usuario
        ///</summary>
        [HttpPost]
        public JsonResult CrearUsuario(CapaEntidad.Usuario usuario)
        {
            try
            {
                CapaNegocio.Usuario objUsuario = new CapaNegocio.Usuario();
                return Json(objUsuario.CrearUsuario(usuario), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Metodo POST para EditarUsuario
        /// se procedera a editar usuario
        /// </summary>
        /// <param name="usuario">recibe la entidad usuario con las propiedades cedula y contraseña</param>
        /// <returns>JsonResult</returns>
        [HttpPost]
        public JsonResult EditarUsuario(CapaEntidad.Usuario usuario)
        {
            try
            {
                return Json(NUsuario.EditarUsuario(usuario), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Metodo POST para Habilitar o inhabilitar cuenta de usuario
        /// se procedera a editar usuario
        /// </summary>
        /// <param name="usuario">recibe el string usuario</param>
        /// <returns>JsonResult</returns>
        public JsonResult EstadoCuenta(string usuario, int proceso)
        {
            try
            {
                CapaNegocio.Usuario objUsuario = new CapaNegocio.Usuario();
                return Json(objUsuario.HabilitarUsuario(usuario, proceso), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Obtener el catalogo de areas disponibles
        /// </summary>
        /// <returns></returns>
        public JsonResult CatalogoZona()
        {
            try{
                //Obtener la informacion de cliente
                List<CapaEntidad.Zona> zona = new List<CapaEntidad.Zona>();
                zona = NZona.ListarZona();
                return Json(new SelectList(zona, "IdZona", "Descripcion"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }




        /// <summary>
        /// Lista de ticket en general
        /// </summary>
        /// <returns>retornará un objeto mapeado a Json</returns>
        public JsonResult ListaTicket()
        {
            try{
                return Json(NTickets.ListaTicket_Hoy(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

       

    }
}