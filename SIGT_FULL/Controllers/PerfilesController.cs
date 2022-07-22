using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIGT_FULL.Controllers
{
    public class PerfilesController : Controller
    {

        //Referencia a la capa de negocio
        private CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();
        //referencia a la capa de permisos
        private CapaNegocio.Permisos NPermisos = new CapaNegocio.Permisos();

        //referencia a la capa de negocio seguridad
        private CapaNegocio.Seguridad seguridad = new CapaNegocio.Seguridad();
        
        //sesiones activas
        private System.Web.HttpContext Context = System.Web.HttpContext.Current;

        /// <summary>
        /// Devuelve la vista con los perfiles existentes
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public ActionResult ListaPerfiles(string usuario)
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

                List<CapaEntidad.UsuarioPerfil> perfiles = new List<CapaEntidad.UsuarioPerfil>();
                //todos los perfiles menos el monitor = 0
                perfiles = NPermisos.ListaPerfiles().Where(x => x.IdPerfil != 0).ToList();
                return View(perfiles);

            }
            catch (Exception){
                return RedirectToAction("Logout", "Home");
            }
        }


        /// <summary>
        /// lista de perfiles existenets
        /// devolvera en formato en json
        /// </summary>
        /// <returns></returns>
        public JsonResult CatalogoPerfiles(){
            try{
                //devolvera la lista de perfiles en formato en json
                return Json(NPermisos.ListaPerfiles().Where(x => x.IdPerfil != 0).ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult CatalogoTipo_Usuario()
        {
            try
            {
                //devolvera la lista de perfiles en formato en json
                return Json(NUsuario.ListaTipo_Usuario(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Vista para la edición de perfil
        /// </summary>
        /// <param name="idPerfil">IdPerfil a editar</param>
        /// <returns></returns>
        public ActionResult EditarPerfil(int idPerfil)
        {
            try{
                //En este punto, rediccionara al controlador administrador o al controlador usuario(cliente)
                if (Session["User"] == null){
                    return RedirectToAction("Logout", "Home");
                }

                //devolver la informacion del perfil
                return View(NPermisos.DetallePerfiles(idPerfil));
            }
            catch (Exception){
                return RedirectToAction("ListaPerfiles", "Perfiles", new { usuario = Session["User"].ToString() });
            }
        }



        /// <summary>
        /// Obtiene la inforamcion del perfil
        /// </summary>
        /// <param name="IdPerfil">perfil a evaluar</param>
        /// <returns>devolvera un json para su respectivo mapep</returns>
        public JsonResult DetallePerfil(int IdPerfil)
        {

            try{
                //Obtener la lista de ticket del dia y devolverlo a la vista
                return Json(NPermisos.DetallePerfiles(IdPerfil), JsonRequestBehavior.AllowGet);
            }
            catch (Exception){
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// obtiene los permisos y usuarios asignados a ese perfil
        /// </summary>
        /// <param name="IdPerfil">Perfil a interactuar</param>
        /// <returns></returns>
        public JsonResult PermisosPerfil(int IdPerfil, string filtro)
        {

            try{
                //retornar la lista serializada 
                return Json(NPermisos.ObtenerPermisos(IdPerfil, filtro), JsonRequestBehavior.AllowGet);
            }
            catch (Exception){
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// Vista parcial para la lista de controles existentes
        /// </summary>
        /// <returns></returns>
        public ActionResult _ListaControles()
        {
            return PartialView("_ListaControles", NPermisos.ListaControles());
        }


        /// <summary>
        /// Vista parcial para la lista de usuarios existentes
        /// </summary>
        /// <returns></returns>
        public ActionResult _ListaUsuarios()
        {
            return PartialView("_ListaUsuarios", NUsuario.ListarUsuarios());
        }


	}
}