using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;


using CapaEntidad;
using CapaNegocio;

namespace SIGT_FULL.Controllers
{
    public class ReporteController : Controller
    {


        /// <summary>
        /// Metodo que retorna la vista principal de Reporte
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            try {
                if (Session["User"] == null)
                    return RedirectToAction("Logout", "Home");

                //Obtener el mes actual y sus dias
                int año = DateTime.Now.Year;
                int mes = (DateTime.Now.Month == 1) ? DateTime.Now.Month: DateTime.Now.Month - 1;
                int UltimoDia = new DateTime(año, mes, 1).AddMonths(1).AddDays(-1).Day;

                string FechaInicio = "01/" + ((mes < 10) ? "0" + mes.ToString() : mes.ToString()) + "/" + año;
                string FechaFin = UltimoDia + "/" + ((mes < 10) ? "0" + mes.ToString() : mes.ToString()) + "/" + año;

                /*string FechaInicio = "01/" + "12" + "/2018";
                string FechaFin = UltimoDia + "/" + "12" + "/2018";*/




                ViewBag.fi = FechaInicio;
                ViewBag.ff = FechaFin;
                ViewBag.usuario = "JDAVILA";


                return View();
            }
            catch (Exception ex) {
                return HttpNotFound();
            }
        }

        /// <summary>
        /// obtiene la lista de usuarios y lo carga en una vista parcial
        /// dibuja el componente de lista de boostrap con media-images
        /// </summary>
        /// <returns>Oficial</returns>
        public ActionResult _ListaUsuarios()
        {
            try {
                List<CapaEntidad.Usuario> lista = new List<CapaEntidad.Usuario>();
                lista = new CapaNegocio.Usuario().ListarUsuarios().Where(x => x.Tipo.IdTipo == 1 && x.Estado == 1 && (x.Cuenta != "ECRUZ" && x.Cuenta != "BCELEDON" && x.Cuenta != "NLACAYO" && x.Cuenta != "KGUADAMUZ")).OrderBy(x => x.Cuenta).ToList();
                
                return PartialView(lista);
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }



        /// <summary>
        /// Vista parcial general que contabiliza los tickets atendidos y su tiempo
        /// de ejecucción
        /// </summary>
        /// <param name="idarea">area a buscar</param>
        /// <param name="fi">fecha de incio a evaluar</param>
        /// <param name="ff">fecha fin a evaluar</param>
        /// <returns>Oficial</returns>
        public ActionResult _GeneralTickets(string idarea, string fi, string ff)
        {

            double Total = 0, Tiempo = 0;



            try {
                //Obtener el total de tickets
                var Generados = (from generado in new CapaNegocio.Reportes().ListaTicket()
                                 where generado.Motivo.Zona.IdZona == idarea &&
                                 Convert.ToDateTime(generado.FechaSolicitud) >= Convert.ToDateTime(fi + " 00:00:00") &&
                                 Convert.ToDateTime(generado.FechaSolicitud) <= Convert.ToDateTime(ff + " 23:59:59")
                                 select generado).ToList();   
                var Anulados = Generados.Where(x => x.Estado == 0).ToList();


                //Obtener los tickets que hayan sido generados a nivel de detalle
                List<CapaEntidad.UsuarioAtencionTickets> atendidos = new List<UsuarioAtencionTickets>();
                atendidos = new CapaNegocio.Reporte().TotalTickets(idarea, fi, ff);
                for (int i = 0; i < atendidos.Count(); i++){
                    Total += (idarea == "01") ? atendidos[i].AtencionPrestamos.TotalAdquirido : atendidos[i].AtencionRecuperaciones.TotalAdquirido;
                }

                //obtener el tiempo a nivel de detealle
                List<CapaEntidad.UsuarioAtencionTiempo> ejecuccion = new List<UsuarioAtencionTiempo>();
                ejecuccion = new CapaNegocio.Reporte().TotalTiempo(idarea, fi, ff);
                for (int i = 0; i < ejecuccion.Count(); i++) {
                    Tiempo += (idarea == "01") ? Convert.ToDouble(ejecuccion[i].TiempoPrestamos.TotalAdquirido) : Convert.ToInt32(ejecuccion[i].TiempoRecuperaciones.TotalAdquirido);
                }


                //Guardar los resultados en VIewbag y mostrarlo a la vista
                ViewBag.Generados = Generados.Count();
                ViewBag.Anulados = ((double) Anulados.Count() / (double) Generados.Count()) * 100;
                ViewBag.Atendidos = Total;
                ViewBag.Tiempo = (double) (Convert.ToDouble(Tiempo) / Convert.ToDouble(3600));


                ViewBag.Icon = (idarea == "01") ? "glyphicon glyphicon-star" : "glyphicon glyphicon-registration-mark";
                ViewBag.Text = (idarea == "01") ? "Área de préstamos" : "Área de recuperaciones";

                return PartialView("_GeneralTickets");
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        public ActionResult _GeneralUsuario(string usuario, string fi, string ff)
        {

            int Total = 0, dias = 0;
            double Tiempo = 0, promedio = 0, rendimiento = 0, TiempoRequerido = 0;
            CapaEntidad.Usuario _user = new CapaEntidad.Usuario();
            _user = new CapaNegocio.Usuario().DetalleUsuario(usuario);


            try
            {
                //Obtener el total de tickets
                var Generados = (from generado in new CapaNegocio.Reportes().ListaTicket()
                                 where generado.Usuario.Cuenta == usuario &&
                                 Convert.ToDateTime(generado.FechaSolicitud) >= Convert.ToDateTime(fi + " 00:00:00") &&
                                 Convert.ToDateTime(generado.FechaSolicitud) <= Convert.ToDateTime(ff + " 23:59:59")
                                 select generado).ToList();

                //Obtener los tickets que hayan sido generados a nivel de detalle
                List<CapaEntidad.UsuarioAtencionTickets> atendidos = new List<UsuarioAtencionTickets>();
                atendidos = new CapaNegocio.Reporte().TotalTickets(_user.Area.IdZona, fi, ff);
                for (int i = 0; i < atendidos.Count; i++) {
                   if(atendidos[i].Usuario == usuario)
                        Total += (_user.Area.IdZona == "01") ? atendidos[i].AtencionPrestamos.TotalAdquirido : atendidos[i].AtencionRecuperaciones.TotalAdquirido;
                }

                //obtener el tiempo promedio, rendimiento y tiempo por la fecha generada
                var tickets = new CapaNegocio.Reporte().RendimientoArea(_user.Area.IdZona, fi, ff);
                for (int i = 0; i < tickets.RendimientoUsuario.Count; i++){
                    for (int j = 0;  j < tickets.RendimientoUsuario[i].Count;  j++) {
                        if (tickets.RendimientoUsuario[i][j].Usuario == usuario){
                            dias = dias + 1;
                            Tiempo = Tiempo + tickets.RendimientoUsuario[i][j].Tiempo;
                            TiempoRequerido = TiempoRequerido + tickets.RendimientoUsuario[i][j].TiempoRequerido;
                            promedio = promedio + tickets.RendimientoUsuario[i][j].Promedio;

                            //rendimiento = rendimiento + tickets.RendimientoUsuario[i][j].Rendimiento;
                        }
                            
                    }
                }
                // rendimiento de todo el mes
                rendimiento = ((TiempoRequerido - Tiempo) / TiempoRequerido) * 100;





                ViewBag.Icon = (_user.Area.IdZona == "01") ? "glyphicon glyphicon-star" : "glyphicon glyphicon-registration-mark";
                ViewBag.Text = "Información general del usuario ";
                ViewBag.TotalTickets = Total;
                ViewBag.Tiempo = Tiempo / 3600;

                ViewBag.promedio = promedio / dias;
                ViewBag.TiempoRequerido = TiempoRequerido / 3600;
                ViewBag.rendimiento = rendimiento;




                return PartialView();
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }





        /// <summary>
        /// Muestra el rendimiento en los tickets atendidos por areas o usuarios
        /// </summary>
        /// <param name="filtro">Tipo de busqueda a realizar: usuario o por area</param>
        /// <param name="IdArea">Id de area a mostrar</param>
        /// <param name="Fi">fecha de inicio a buscar</param>
        /// <param name="Ff">fecha fin a buscar</param>
        /// <returns></returns>
        public ActionResult _Rendimiento(string filtro, string busqueda, string fi, string ff)
        {
            double ROb = 0, MejorRendimiento = 0, TotalTiempo = 0, TiempoRequerido = 0, ValorObtenido;
            int contador = 0, Tickets = 0 ;
            string MejorDia = "";

            try {
                //Obtener la información del area
                CapaEntidad.Zona area = new CapaEntidad.Zona();
                CapaEntidad.Usuario usuario = new CapaEntidad.Usuario();

                area = (filtro == "Area") ? new CapaNegocio.Zona().DetalleZona(busqueda) : null;
                usuario = (filtro == "Usuario") ? new CapaNegocio.Usuario().DetalleUsuario(busqueda) : null;

                //Obtener el rendimiento
                List<CapaEntidad.UsuarioRendimiento> rendimiento = new List<UsuarioRendimiento>();
                rendimiento = new CapaNegocio.Reporte().Rendimiento(filtro, busqueda, fi, ff);

                //obtener el rendimiento general sumar los obtenidos por dias y dividirlo entre la cantidad
                for (int i = 0; i < rendimiento.Count; i++){
                    ROb += rendimiento[i].Rendimiento;
                    TiempoRequerido += rendimiento[i].TiempoRequerido;
                    TotalTiempo += rendimiento[i].Tiempo;

                    MejorRendimiento = (rendimiento[i].Rendimiento >= MejorRendimiento) ? rendimiento[i].Rendimiento : MejorRendimiento;
                    MejorDia = (rendimiento[i].Rendimiento >= MejorRendimiento) ? rendimiento[i].Fecha.ToLongDateString().ToString(): MejorDia;
                    Tickets += Convert.ToInt32(rendimiento[i].NumTickets);
                    contador = Convert.ToInt32(rendimiento[i].Id);
                    
                }


                ROb = ((TiempoRequerido - TotalTiempo) / TiempoRequerido) * 100;
                ValorObtenido = ROb;

                //guardar en viewbag para compartir a la vista
                ViewBag.IdDiv = (filtro == "Area") ? area.Descripcion : usuario.Cuenta;
                ViewBag.Rendimiento = ValorObtenido;
                ViewBag.MejorDia = MejorDia;
                ViewBag.MejorRendimiento = MejorRendimiento;
                ViewBag.Tickets = Tickets;

                ViewBag.User = "Resultados obtenidos por " + busqueda;
                ViewBag.TotalWork = TotalTiempo / 3600;
                ViewBag.Performance = ValorObtenido;
                ViewBag.TimeRequired = TiempoRequerido / 3600; 



                return PartialView();
            }
            catch (Exception){
                return HttpNotFound();
            }
        }







        /// <summary>
        /// tickets generados por todas las areas
        /// </summary>
        /// <param name="fi">fecha de inicio a buscar</param>
        /// <param name="ff">fecha fin a buscar</param>
        /// <returns>Oficial</returns>
        public ActionResult _TicketsGenerados(string fi, string ff)
        {
            try {

                ViewBag.fi = fi;
                ViewBag.ff = ff;
                return PartialView();
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        /// <summary>
        /// grafica lineal para las areas
        /// </summary>
        /// <param name="fi">fecha de inicio a buscar</param>
        /// <param name="ff">fecha fin a buscar</param>
        /// <returns>Oficial</returns>
        public JsonResult _LinealGraph_TicketsAreas(string fi, string ff)
        {
            try {
                CapaEntidad.TickestAreas tickets = new TickestAreas();

                //para obtener la cantidad realizada por dia determinado
                //usar linq agrupando por fecha
                List<Ticket> lista = new List<Ticket>();
                lista = new CapaNegocio.Reportes().ListaTicket();



                tickets.prestamos = (from generado in lista
                                     where generado.Motivo.Zona.IdZona == "01" &&
                                     (generado.Estado == 3 || generado.Estado == 4) &&
                                     Convert.ToDateTime(generado.FechaSolicitud) >= Convert.ToDateTime(fi + " 00:00:00") &&
                                     Convert.ToDateTime(generado.FechaSolicitud) <= Convert.ToDateTime(ff + " 23:59:59")
                                     group generado.Usuario.Cuenta by Convert.ToDateTime(generado.FechaSolicitud).ToShortDateString() into g
                                     orderby Convert.ToDateTime(g.Key) ascending
                                     select new { Fecha = Convert.ToDateTime(g.Key).ToString("yyyy/MM/dd"), Total = g.Count() }).ToList();

                tickets.recuperaciones = (from generado in lista
                                          where generado.Motivo.Zona.IdZona == "02" &&
                                          (generado.Estado == 3 || generado.Estado == 4) &&
                                          Convert.ToDateTime(generado.FechaSolicitud) >= Convert.ToDateTime(fi + " 00:00:00") &&
                                          Convert.ToDateTime(generado.FechaSolicitud) <= Convert.ToDateTime(ff + " 23:59:59")
                                          group generado.Usuario.Cuenta by Convert.ToDateTime(generado.FechaSolicitud).ToShortDateString() into g
                                          orderby Convert.ToDateTime(g.Key) ascending
                                          select new { Fecha = Convert.ToDateTime(g.Key).ToString("yyyy/MM/dd"), Total = g.Count() }).ToList();



                

                return Json(tickets, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// Crea una vista parcial para presentar los tickets atendidos
        /// de un usuario en especifico
        /// </summary>
        /// <param name="usuario">usuario a buscar</param>
        /// <param name="fi">fecha de inicio a buscar</param>
        /// <param name="ff">fecha fin a buscar</param>
        /// <returns>Oficial</returns>
        public ActionResult _TicketsUsuario(string usuario, string fi, string ff)
        {
            try {
                ViewBag.usuario = usuario;
                ViewBag.fi = fi;
                ViewBag.ff = ff;
                return PartialView();
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }


        /// <summary>
        /// metodo que retorna la grafica lineal para un usuario en especifico
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="fi"></param>
        /// <param name="ff"></param>
        /// <returns>Oficial</returns>
        public JsonResult _LinealGraph_TicketsUsuario(string usuario, string fi, string ff)
        {
            try
            {
                //para obtener la cantidad realizada por dia determinado
                //usar linq agrupando por fecha
                var ticket = (from generado in new CapaNegocio.Reportes().ListaTicket()
                              where generado.Usuario.Cuenta == usuario &&
                              (generado.Estado == 3 || generado.Estado == 4) &&
                              Convert.ToDateTime(generado.FechaSolicitud) >= Convert.ToDateTime(fi + " 00:00:00") &&
                              Convert.ToDateTime(generado.FechaSolicitud) <= Convert.ToDateTime(ff + " 23:59:59")
                              group generado.Usuario.Cuenta by Convert.ToDateTime(generado.FechaSolicitud).ToShortDateString() into g
                              orderby Convert.ToDateTime(g.Key) ascending
                              select new { Fecha = Convert.ToDateTime(g.Key).ToString("yyyy/MM/dd"), Total = g.Count() }).ToList();


                return Json(ticket, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

       

        /// <summary>
        /// Obtiene la vista parcial para tiempo de atenció
        /// </summary>
        /// <param name="fi">fecha inical</param>
        /// <param name="ff">fecha final</param>
        /// <returns></returns>
        public ActionResult _TiempoAtencionArea(string idarea, string fi, string ff)
        {
            try{
                ViewBag.idarea = idarea;
                ViewBag.area = new CapaNegocio.Zona().DetalleZona(idarea).Descripcion;
                ViewBag.fi = fi;
                ViewBag.ff = ff;
                return PartialView();
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }


        /// <summary>
        /// devuelve en formato json los tiempos de atencion de las dos areas existentes
        /// </summary>
        /// <param name="area">area a evaluar</param>
        /// <param name="fi"></param>
        /// <param name="ff"></param>
        /// <returns></returns>
        public JsonResult _PieGraph_AtencionArea(string idarea, string fi, string ff)
        {
            try {
                CapaEntidad.TiempoAtencion_Area tickets = new TiempoAtencion_Area();
                tickets = new CapaNegocio.Reportes().TiempoAtencion_Area(idarea, fi, ff);

                return Json(tickets, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }




        /// <summary>
        /// Obtiene la vista parcial para tiempo de atenció
        /// </summary>
        /// <param name="fi">fecha inical</param>
        /// <param name="ff">fecha final</param>
        /// <returns></returns>
        public ActionResult _TiempoAtencionUsuario(string usuario, string fi, string ff)
        {
            try
            {
                ViewBag.usuario = usuario;
                ViewBag.fi = fi;
                ViewBag.ff = ff;
                return PartialView();
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }


        /// <summary>
        /// devuelve en formato json los tiempos de atencion de las dos areas existentes
        /// </summary>
        /// <param name="area">area a evaluar</param>
        /// <param name="fi"></param>
        /// <param name="ff"></param>
        /// <returns></returns>
        public JsonResult _PieGraph_AtencionUsuario(string usuario, string fi, string ff)
        {
            try
            {
                CapaEntidad.TiempoAtencion_Usuario tickets = new TiempoAtencion_Usuario();
                tickets = new CapaNegocio.Reportes().TiempoAtencion_Usuario(usuario, fi, ff);

                return Json(tickets, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// vista parcial que muestra el rendimiento adquirido de los usuarios integrantes de 
        /// un area
        /// </summary>
        /// <param name="area"></param>
        /// <param name="fi"></param>
        /// <param name="ff"></param>
        /// <returns>Listo</returns>
        public ActionResult _RendimientoArea(string idarea, string fi, string ff)
        {
            try
            {
                ViewBag.idarea = idarea;
                ViewBag.area = new CapaNegocio.Zona().DetalleZona(idarea).Descripcion;
                ViewBag.fi = fi;
                ViewBag.ff = ff;
                return PartialView();
            }
            catch (Exception){
                return HttpNotFound();
            }
        }

        public JsonResult _PieGraph_RendimientoArea(string idarea, string fi, string ff)
        {
            try
            {
                CapaEntidad.AreaRendimiento tickets = new AreaRendimiento();
                tickets = new CapaNegocio.Reporte().RendimientoArea(idarea, fi, ff);


                return Json(tickets, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }





















        /// <summary>
        /// Vista Parcial que retorna la información 
        /// del total de tickets por dia
        /// PD: I send the list from the view Index due to that i want to avoid an overload of the list "List<Ticket>"
        /// </summary>
        /// <returns>Vista parcial con la información del usuario</returns>
        public ActionResult _TotalTicket(List<Ticket> ListaTicket)
        {
            try{
                //si no existe sesiones
                if (Session["User"] == null)
                    return RedirectToAction("Logout", "Home");

                //filtrar por tickets del dia
                //var TicketDia = ListaTicket.Where(x => Convert.ToDateTime(x.FechaSolicitud).Year == DateTime.Now.Year && Convert.ToDateTime(x.FechaSolicitud).Day == DateTime.Now.Day && Convert.ToDateTime(x.FechaSolicitud).Month == DateTime.Now.Month).ToList();
                ////filtrar por tickets por áreas
                //var TicketPrestamo = TicketDia.Where(x => x.Motivo.Zona.IdZona == "01" || x.Motivo.Zona.IdZona == "03").ToList();
                //var TicketRecuperaciones = TicketDia.Where(x => x.Motivo.Zona.IdZona == "02").ToList();


                
                //Get the created tickets of today
                CapaEntidad.PivotTicket_Areas pivot = new CapaNegocio.Reporte().TicketPorArea(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);

                //save on viewbag the tickets counts
                ViewBag.TicketDia = pivot.Total;
                ViewBag.Recuperaciones = pivot.Recuperaciones;
                ViewBag.Prestamos = pivot.Prestamos;


                
                




                //devolver vista parcial buscando la ruta en la vista
                return PartialView("~/Views/Reporte/Usuario/_TotalTicket.cshtml");
            }
            catch (Exception ex)
            {
                return HttpNotFound();
            }
        }

        /// <summary>
        /// Vista Parcial que retorna la información 
        /// de los ticket en general atendidos en el dia
        /// PD: I send the list from the view Index due to that i want to avoid an overload of the list "List<Ticket>"
        /// </summary>
        /// <returns>Vista parcial con la información del usuario</returns>
        public ActionResult _TotalTicket_Atendidos(List<Ticket> ListaTicket)
        {
            try
            {
                //si no existe sesiones
                if (Session["User"] == null)
                    return RedirectToAction("Logout", "Home");

                //filtrar por tickets del dia
                //var TicketDia = ListaTicket.Where(x => Convert.ToDateTime(x.FechaSolicitud).Year == DateTime.Now.Year && Convert.ToDateTime(x.FechaSolicitud).Day == DateTime.Now.Day && Convert.ToDateTime(x.FechaSolicitud).Month == DateTime.Now.Month).ToList();

                ////guardar en viewbag el conteo de tickets
                //ViewBag.Total = ListaTicket.Count;
                //ViewBag.Atendidos = TicketDia.Where(x => x.Estado == 4).ToList().Count;
                //ViewBag.Anulado = TicketDia.Where(x => x.Estado == 0).ToList().Count;
                //ViewBag.Espera = TicketDia.Where(x => x.Estado == 1).ToList().Count;
                //ViewBag.Atendiendose = TicketDia.Where(x => x.Estado == 2 || x.Estado == 3).ToList().Count;
                //ViewBag.TicketDia = TicketDia.Count;


                CapaEntidad.PivotTicket_Atencion pivot = new CapaNegocio.Reporte().TicketPorAtencion(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                ViewBag.Anulado = pivot.Anulados;
                ViewBag.Espera = pivot.Pendientes;
                ViewBag.Atendiendose = pivot.Procesando + pivot.Asignados;
                ViewBag.Atendidos = pivot.Atendidos;
                ViewBag.TicketDia = pivot.Total;



                return PartialView("~/Views/Reporte/Usuario/_TotalTicket_Atendidos.cshtml");
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }


        /// <summary>
        /// Vista Parcial que retorna la información 
        /// de los ticket del dia segun el usuario
        /// </summary>
        /// <returns>Vista parcial con la información del usuario</returns>
        public ActionResult _TotalDia_Usuario(List<Ticket> ListaTicket)
        {
            try
            {
                //si no existe sesiones
                if (Session["User"] == null)
                    return RedirectToAction("Logout", "Home");


                ////filtrar por tickets del dia
                //var TicketDia = ListaTicket.Where(x => Convert.ToDateTime(x.FechaSolicitud).Year == DateTime.Now.Year && Convert.ToDateTime(x.FechaSolicitud).Day == DateTime.Now.Day && Convert.ToDateTime(x.FechaSolicitud).Month == DateTime.Now.Month).ToList();
                //var TicketUser = TicketDia.Where(x => x.Usuario.Cuenta == Session["User"].ToString()).ToList();


                ////guardar en viewbag el conteo de tickets
                //ViewBag.PorMi = TicketUser.Count;
                //ViewBag.TicketDia = TicketDia.Count;



                List<CapaEntidad.PivotTicket_AtencionUsuario> pivot = new CapaNegocio.Reporte().TicketPorAtencion_Usuario(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                var list = (from x in pivot where x.Usuario == Session["User"].ToString() select x).ToList();


                ViewBag.PorMi = ((list.Count == 0) ? 0 : list.FirstOrDefault().Total);
                ViewBag.TicketDia = (from x in pivot select x.Total).Sum();


                return PartialView("~/Views/Reporte/Usuario/_TotalDia_Usuario.cshtml");
            }
            catch (Exception)
            {

                return HttpNotFound();
            }
        }


        public ActionResult _TotalTicket_UsuarioHora()
        {
            try{

                //si no existe sesiones
                if (Session["User"] == null)
                    return RedirectToAction("Logout", "Home");

                // crear un objeto indefinido
                List<CapaEntidad.PivoteTicket_UsuarioHora> pivot = new CapaNegocio.Reporte().TicketPorAtencion_UsuarioHora(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, Session["User"].ToString());

                if(pivot.Count > 0){
                    ViewBag.Usuario = pivot[0].Usuario;
                    ViewBag.Area = pivot[0].Area;
                    ViewBag._8 = pivot[0]._8;
                    ViewBag._9 = pivot[0]._9;
                    ViewBag._10 = pivot[0]._10;
                    ViewBag._11 = pivot[0]._11;
                    ViewBag._12 = pivot[0]._12;
                    ViewBag._13 = pivot[0]._13;
                    ViewBag._14 = pivot[0]._14;
                    ViewBag._15 = pivot[0]._15;
                    ViewBag._16 = pivot[0]._16;
                    ViewBag._17 = pivot[0]._17;
                    ViewBag.Total= pivot[0].Total;
                }
                else
                {
                    ViewBag.Usuario = Session["User"].ToString();
                    ViewBag.Usuario = "";
                    ViewBag._8 = 0;
                    ViewBag._9 = 0;
                    ViewBag._10 = 0;
                    ViewBag._11 = 0;
                    ViewBag._12 = 0;
                    ViewBag._13 = 0;
                    ViewBag._14 = 0;
                    ViewBag._15 = 0;
                    ViewBag._16 = 0;
                    ViewBag._17 = 0;
                    ViewBag.Total = 0;
                }


                


                return View("~/Views/Reporte/Usuario/_TotalTicket_UsuarioHora.cshtml");
            }
            catch (Exception)
            {

                return HttpNotFound();
            }
        }






        public ActionResult _TotalTicket_AreaHora()
        {
            try
            {

                //si no existe sesiones
                if (Session["User"] == null)
                    return RedirectToAction("Logout", "Home");

                // crear un objeto indefinido                                               
                List<CapaEntidad.PivoteTicket_UsuarioHora> pivot = new CapaNegocio.Reporte().TicketPorAtencion_AreaHora(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                List<CapaEntidad.PivoteTicket_UsuarioHora> pivotPrestamo = new List<PivoteTicket_UsuarioHora>();
                List<CapaEntidad.PivoteTicket_UsuarioHora> pivotRecuperaciones = new List<PivoteTicket_UsuarioHora>();

                ViewBag.pivotPrestamo = new CapaEntidad.PivoteTicket_UsuarioHora();
                ViewBag.pivotRecuperaciones = new CapaEntidad.PivoteTicket_UsuarioHora();
                if (pivot.Count > 0){
                    ViewBag.pivotPrestamo = (pivot.Where(x => x.Area == "Prestamos").ToList().Count > 0 ? pivot.Where(x => x.Area == "Prestamos").ToList().FirstOrDefault() : new CapaEntidad.PivoteTicket_UsuarioHora());
                    ViewBag.pivotRecuperaciones = (pivot.Where(x => x.Area == "Recuperaciones").ToList().Count > 0 ? pivot.Where(x => x.Area == "Recuperaciones").ToList().FirstOrDefault() : new CapaEntidad.PivoteTicket_UsuarioHora());
                }

                

                return View("~/Views/Reporte/Usuario/_TotalTicket_AreaHora.cshtml");
            }
            catch (Exception)
            {

                return HttpNotFound();
            }
        }














        /// <summary>
        /// muestra la grafica combinada {circular, barra}
        /// que representa la cantidad de tickests atendido por area
        /// por gestion y usuario que han atendidos a los clientes
        /// </summary>
        /// <returns></returns>
        public ActionResult _TicketsAreas()
        {
            try {
                return PartialView("~/Views/Reporte/General/_TicketsAreas.cshtml");
            }
            catch (Exception){

                return HttpNotFound();
            }
        }



 


        /// <summary>
        /// Total de tickets atendidos 
        /// por los usuarios mensualmente
        /// </summary>
        /// <returns></returns>
        public ActionResult _TotalTicket_Mensual()
        {
            try{

                //si no existe sesiones
                if (Session["User"] == null)
                    return RedirectToAction("Logout", "Home");

                // crear un objeto indefinido
                CapaNegocio.Reportes NReportes = new Reportes();
                ViewBag.TotalMensual = NReportes.ReporteTickets_Mensual(DateTime.Now.Year);
                ViewBag.TotalMensual_Usuario = NReportes.ReporteTickets_MensualUsuario(DateTime.Now.Year, Session["User"].ToString());

                return View("~/Views/Reporte/Usuario/_TotalTicket_Mensual.cshtml");
            }
            catch (Exception) {

                return HttpNotFound();
            }
        }

        /// <summary>
        /// Total de tickets atendidos 
        /// por los usuarios mensualmente
        /// </summary>
        /// <returns></returns>
        public ActionResult _TotalTicket_MensualArea()
        {
            try {

                //si no existe sesiones
                if (Session["User"] == null)
                    return RedirectToAction("Logout", "Home");

                // crear un objeto indefinido
                CapaNegocio.Reportes NReportes = new Reportes();
                ViewBag.TotalMensual = NReportes.ReporteTickets_Mensual(DateTime.Now.Year);
                ViewBag.TotalRecuperaciones = NReportes.ReporteTickets_MensualArea(DateTime.Now.Year, "02");
                ViewBag.TotalPrestamo = NReportes.ReporteTickets_MensualArea(DateTime.Now.Year, "01");

                return View("~/Views/Reporte/Usuario/_TotalTicket_MensualArea.cshtml");
            }
            catch (Exception)
            {

                return HttpNotFound();
            }
        }


        /// <summary>
        /// Vista Parcial que retorna la información 
        /// de los ticket del dia segun el usuario
        /// PD: I send the list from the view Index due to that i want to avoid an overload of the list "List<Ticket>"
        /// </summary>
        /// <returns>Vista parcial con la información del usuario</returns>
        public ActionResult _TotalTicket_PromedioVisita(List<Ticket> ListaTicket)
        {
            try
            {
                //si no existe sesiones
                if (Session["User"] == null)
                    return RedirectToAction("Logout", "Home");

                return PartialView("~/Views/Reporte/Usuario/_TotalTicket_PromedioVisita.cshtml");
            }
            catch (Exception)
            {

                return HttpNotFound();
            }
        }


        /// <summary>
        /// obtiene la lista de usuarios existenten en el sistema
        /// retornará un objeto serializado a json
        /// </summary>
        /// <returns></returns>
        public JsonResult ListaUsuario()
        {
            try {
                //obtener la lista de usuarios
                List<CapaEntidad.Usuario> lista = new List<CapaEntidad.Usuario>();
                lista = new CapaNegocio.Usuario().ListarUsuarios().Where(x => x.Tipo.IdTipo == 1 && x.Estado == 1).OrderBy(x => x.Cuenta).ToList();
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}