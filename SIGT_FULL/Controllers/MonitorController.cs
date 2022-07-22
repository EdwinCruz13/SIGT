using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SIGT.Controllers
{
    public class MonitorController : Controller
    {

        private CapaNegocio.Utilidades NUtilidades = new CapaNegocio.Utilidades();
        //referencia a controlador seguridad
        private CapaNegocio.Seguridad seguridad = new CapaNegocio.Seguridad();
        //referencia a la capa de negocio, clase usuario
        private CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();
        //referencia a la capa de negocio, clase usuario
        private CapaNegocio.Estaciones NEstaciones = new CapaNegocio.Estaciones();

        //pagina principal, en la siguiente vista capturará la IP
        //del televisor publicitario
        //y la enviara a "PantallaTurno" para verificar el área proveniente
        public ActionResult Index()
        {
           return View();
        }


        //Metodo para generar la pantalla de publicidad y de turno
        public ActionResult PantallaTurno(string IP)
        {

            //validar que contenga la ip
            if (IP == null)
                return RedirectToAction("Index", "Home");


            //compartir infomacion a la vista
            ViewBag.Ticket = "------";
            ViewBag.Estacion = "------";

            //Obtener la informacion del televisor para detectar la zona
            //segun la IP
            var user = new CapaEntidad.Usuario();
            user = NUsuario.DetalleUsuarioIP(IP);

            var TA_User = new List<CapaEntidad.EstacionTrabajo>();
            TA_User = NEstaciones.DetalleEstacionesActiva();
            if(TA_User.Count > 0){

                //linq de dos maneras, por si queres aprender
                // 1 clausula where
                var ticket = TA_User.Where(x => x.Turno.Motivo != null && x.Turno.Motivo.Zona.IdZona == user.Area.IdZona).FirstOrDefault();
                ViewBag.Ticket = (ticket != null) ? ticket.Ticket :  "------";

                // 2 clausula sql
                var estacion = (from T in TA_User where T.Turno.Motivo != null && T.Turno.Motivo.Zona.IdZona == user.Area.IdZona select T).FirstOrDefault();
                ViewBag.Estacion = (estacion != null) ? estacion.NombreEstacion : "------"; ;
            }


            ViewBag.CodArea = user.Area.IdZona; //compartir infomacion a la vista
            ViewBag.NombreArea = user.Area.Descripcion.ToUpper(); //compartir infomacion a la vista
           

            //retornar el "Modelo" de listas de estaciones y retornarlos a la vista
            return View(NEstaciones.ListaEstaciones());
        }

       























        //cargar vista parciales el reproductor de videos
        public ActionResult _VideoPlayer()
        {
            return PartialView();
        }

        public ActionResult Test()
        {
            return View();
        }


        //Obtener la lista de reproduccion, se hara llamado via ajax en la interfaz Publicidad
        public JsonResult PlayList()
        {
            List<CapaEntidad.Videos> videos = new List<CapaEntidad.Videos>();
            videos = NUtilidades.ListaVideos();
            return Json(videos, JsonRequestBehavior.AllowGet);
        }

	}
}