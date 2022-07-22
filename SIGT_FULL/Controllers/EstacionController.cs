using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIGT_FULL.Controllers
{
    public class EstacionController : Controller
    {

        //sesiones activas
        private System.Web.HttpContext Context = System.Web.HttpContext.Current;
        //referencia a la capa de negocio seguridad
        CapaNegocio.Seguridad seguridad = new CapaNegocio.Seguridad();
        CapaNegocio.Estaciones NEstaciones = new CapaNegocio.Estaciones();
        
        /// <summary>
        /// Lista de estaciones existentes
        /// </summary>
        /// <returns></returns>
        public ActionResult ListaEstacion(string usuario)
        {
            try{
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


                //Obtener la lista de estaciones existentes y devolver la lista  a la vista
                List<CapaEntidad.Estacion> Lista = new List<CapaEntidad.Estacion>();
                Lista = NEstaciones.ListaEstacionesActiva();
                return View(Lista);

            }
            catch (Exception){
                return RedirectToAction("Logout", "Home");
            }
        }


        /// <summary>
        /// Metodo get para Registrar la estacion
        /// y los turnos por defecto a la estacion
        /// </summary>
        /// <returns>Jsonresult</returns>
        public JsonResult RegistrarEstacion(CapaEntidad.EstacionMotivos EstacionMotivo)
        {
            try{
                //Ir a la capa de negocio para ejecutar la transaccion
                return Json(NEstaciones.CrearEstacion(EstacionMotivo), JsonRequestBehavior.AllowGet);
            }
            catch (Exception){
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


	}
}