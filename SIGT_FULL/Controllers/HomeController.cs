using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using CapaEntidad;
using CapaNegocio;


namespace SIGT_FULL.Controllers
{


    public class HomeController : Controller
    {


        //usar estas propiedades para crear referencias a otras capas
        Seguridad seguridad = new Seguridad();
        CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();


        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Pantalla de login
        /// mostrara el formulario web para acceso
        /// al usuarios
        /// </summary>
        /// <param name="IP">IP DETECTADA</param>
        /// <returns></returns>
        public ActionResult Login(string IP)
        {

            try{
                //IP = "192.168.1.95"; // IP de prueba
                //crear ViewBag IP que será usado en el formulario
                ViewBag.IP = IP;

                //Obtener la informacion del usuario a traves de su IP
                var Usuario = NUsuario.DetalleUsuarioIP(IP);
                //validar si es una estacion o monitor (0=monitor, 1=estacion)
                if (Usuario.Tipo.IdTipo == 0){
                    return RedirectToAction("PantallaTurno", "Monitor", new { IP = IP });
                }

                //validar si es el expendedor de tickets (kiosko)
                if (Usuario.Tipo.IdTipo == 2){
                    return RedirectToAction("Index", "Expendedor", new { IP = IP });
                }

                //si no es un monitor, mostrar la vista del formulario
                return View();
            }
            catch (Exception){
                return View();
            }
        }


        /// <summary>
        /// Metodo POST para login
        /// se validara el acceso del usuario, si tiene permitido acceder a la cuenta
        /// </summary>
        /// <param name="usuario">recibe la entidad usuario con las propiedades cedula y contraseña</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult Login(CapaEntidad.Usuario usuario)
        {

            try {
                
                //verificar la existencia de la cuenta de usuario
                //si existe resultado(existe y que este activo) crear las sesiones
                if (seguridad.CrearSesiones(usuario.Cuenta, usuario.Contrasena, usuario.IP) == true) {
                        return Json(usuario, JsonRequestBehavior.AllowGet);
                }

                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }


        /// <summary>
        /// Metodo get para Logout
        /// </summary>
        /// <returns>retornara a al login</returns>
        public ActionResult Logout()
        {
            string usuario = (Session["User"] == null) ? String.Empty : Session["User"].ToString();
            string ip = (Session["IP"] == null) ? String.Empty : Session["IP"].ToString();

            //cerrrar sesion
            seguridad.DestruirSession(usuario, ip);
            return RedirectToAction("Login", "Home", new { IP = ip });
        }

    }
}