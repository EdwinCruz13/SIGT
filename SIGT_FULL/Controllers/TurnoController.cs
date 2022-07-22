using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CapaEntidad;
using CapaNegocio;
using DevExpress.XtraPrinting;

namespace SIGT.Controllers
{
    public class TurnoController : Controller
    {
        //Referencia a la capa de negocio de instituciones
        CapaNegocio.Instituciones NInstitucion = new CapaNegocio.Instituciones();

        //Referencia a la capa de negocio de Turnos
        CapaNegocio.Turno NTurno = new CapaNegocio.Turno();

        //Referencia a la capa de negocio de clientes
        CapaNegocio.Cliente NCliente = new CapaNegocio.Cliente();

        //Crear la referencia a la capa de datos con un objeto
        CapaNegocio.Zona NZona = new CapaNegocio.Zona();

        //referencia a controlador seguridad
        CapaNegocio.Seguridad seguridad = new CapaNegocio.Seguridad();

        //sesiones activas
        private System.Web.HttpContext Context = System.Web.HttpContext.Current;


        // <summary>
        /// Metodo get para Modulo de ticket
        /// </summary>
        /// <returns>ActionResult</returns>
        /// recibe el usuario logeado
        /// visualizará los ticket generados en un rango de fecha
        /// por default se generarán los ticket del dia
        public ActionResult ListaTicket(string usuario)
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
                if ((int)Context.Session["TypeUser"] == 0)
                {
                    return RedirectToAction("Index", "Monitor");
                }

                //obtener la fecha actual
                string FechaInicio = String.Format("{0:yyyy}", DateTime.Now) + "-" + String.Format("{0:MM}", DateTime.Now) + "-" + String.Format("{0:dd}", DateTime.Now) + " " +
                     "00" + ":" + "00" + ":" + "00";
                string FechaFin = String.Format("{0:yyyy}", DateTime.Now) + "-" + String.Format("{0:MM}", DateTime.Now) + "-" + String.Format("{0:dd}", DateTime.Now) + " " +
                     "23" + ":" + "59" + ":" + "59";

                //Obtener la lista de ticket del dia y devolverlo a la vista
                return View(NTurno.TicketFecha(FechaInicio, FechaFin, 1));
            }
            catch (Exception){
                return RedirectToAction("Logout", "Home");
            }
        }


        // <summary>
        /// Metodo get para buscar las ticket segun el filtro
        /// </summary>
        /// <returns>ActionResult</returns>
        /// recibe el usuario logeado
        /// visualizará los ticket generados en un rango de fecha
        /// por default se generarán los ticket del dia por si no recibe valores
        public JsonResult BuscarTicket(int Estado, string fechaInicio = null, string fechaFin = null)
        {

            try{
                if(fechaInicio == null || fechaFin == null){
                    fechaInicio = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
                    fechaFin = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
                }



                //Obtener la lista de ticket del dia y devolverlo a la vista
                return Json(NTurno.TicketFecha(fechaInicio, fechaFin, Estado), JsonRequestBehavior.AllowGet);
            }
            catch (Exception){
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            
        }

        // <summary>
        /// Metodo get para buscar las actualizar ticket en la prioridad o estado
        /// </summary>
        /// <returns>ActionResult</returns>
        /// recibe de entrada el ID ticket, CodTicket, Usuario realiza
        public JsonResult ActualizarTicket(int ID, string CodTicket, int Valor, string Usuario)
        {

            try{
                //Obtener la lista de ticket del dia y devolverlo a la vista
                return Json(NTurno.ModificarTicket(ID, CodTicket, Valor, Usuario), JsonRequestBehavior.AllowGet);
            }
            catch (Exception){
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }




        /// <summary>
        /// Metodo get para GenerarTicket
        /// devolvera la lista
        /// </summary>
        /// <returns>ActionResult</returns>
        /// recibe el usuario logeado
        /// devolvera la lista de clientes
        public ActionResult RegistrarTicket(string usuario)
        {


            //Si no existe la sesion
            if ((string)Context.Session["User"] == null) {
               return RedirectToAction("Logout", "Home");
            }
                
            //En este punto, rediccionara al controlador administrador o al controlador usuario(cliente)
            if (seguridad.VerificarSesion("User", usuario) == false){
                return RedirectToAction("Logout", "Home");
            }

            //En este punto, rediccionara al controlador administrador o al controlador usuario(cliente)
            if ((int)Context.Session["TypeUser"] == 0)
            {
                return RedirectToAction("Index", "Monitor");
            }

            //Retornar Lista
            return View();
        }

        /// <summary>
        /// Metodo get para Registrar la solicitud
        /// realizará el registro del ticket y cliente
        /// </summary>
        /// <returns>Jsonresult</returns>
        public JsonResult _RegistrarTicket(Ticket solicitud)
        {
            try{
                CapaEntidad.Ticket ticketGenerado = new CapaEntidad.Ticket();
                ticketGenerado = NTurno.GenerarTicket(solicitud);

                //Si se genera el ticket, devolver los datos a la vista y cargar la pagina del reporte
                if(ticketGenerado != null && ticketGenerado.CodTicket != null){
                    return Json(ticketGenerado, JsonRequestBehavior.AllowGet);
                }
                    

                //devolver error
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            catch (Exception){
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }





        // <summary>
        /// Metodo get para la visualizacion del ticket
        /// </summary>
        /// <returns>Jsonresult</returns>
        public ActionResult VisualizarTicket(string Turno)
        {
            try{
                //obtener la informacion del ticket
                CapaEntidad.Ticket ticket = new CapaEntidad.Ticket();
                ticket = NTurno.DetalleTicket(Turno);

                //imprimir ticket
                NTurno.Impresion(ticket);

                //redireccionar a la lista de ticjets
                return RedirectToAction("ListaTicket", "Turno", new { usuario = Context.Session["User"].ToString() });


                ////crear reporte
                //var report = new CapaReportes.Ticket(ticket);

                //PrintingSystemBase printingSystem1 = report.PrintingSystem;
                //ExportOptions options = printingSystem1.ExportOptions;

                //// Set Print Preview options.
                //options.PrintPreview.ActionAfterExport = ActionAfterExport.AskUser;
                //options.PrintPreview.DefaultDirectory = "C:\\Temp";
                //options.PrintPreview.DefaultFileName = "Report";
                //options.PrintPreview.SaveMode = SaveMode.UsingDefaultPath;
                //options.PrintPreview.ShowOptionsBeforeExport = false;


                ////XtraReport report = new XtraReport();
                //using (System.IO.MemoryStream ms = new System.IO.MemoryStream()){
                //    report.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
                //    WriteDocumentToResponse(ms.ToArray(), "pdf", true, "Report.pdf");
                //}

                ////visualizar el reporte en la vista
                //return View(report);
            }
            catch (Exception){
                return RedirectToAction("Index", "Home", new { usuario = Context.Session["User"].ToString() });
            }
            
        }


        /// <summary>
        /// Metodo get para Clientes y la lista de cedulas que se encuentran en inversiones
        /// </summary>
        /// <returns>JsonResult</returns>
        /// recibe de parametros 3 variables opcionales
        /// devolvera la lista de clientes
        public JsonResult CatalogoClientes(string filtro = null, string campo = null, string valor = null)
        {
            try{
                //Obtener la informacion de cliente
                List<CapaEntidad.Cliente> clientes = new List<CapaEntidad.Cliente>();
                clientes = NCliente.ListadoCliente_Inversiones(filtro, campo, valor);
                return Json(clientes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Metodo GET para Lista de ListaBarrio
        /// recibe como parametro el cod del departamento y cod de municipio, devolvera la lista de barrios de un municipio
        /// se usara en el formulario de registro
        /// </summary>
        /// <returns>JsonResult</returns>
        public JsonResult ListaInstitucion()
        {
            try{
                //Referencia al servicio
                List<Institucion> Instituciones = new CapaNegocio.Instituciones().ListaInstitucion();
                return Json(new SelectList(Instituciones, "IdInstitucion", "NombreInstitucion"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }


        /// <summary>
        /// Metodo get para la lista de zonas existentes
        /// </summary>
        /// <returns>JsonResult</returns>
        /// devolvera la lista de clientes
        public JsonResult CatalogoZona()
        {
            try{
                //Obtener la informacion de cliente
                List<CapaEntidad.Zona> zona = new List<CapaEntidad.Zona>();
                zona = NZona.ListarZona();
                return Json(new SelectList(zona, "IdZona", "Descripcion"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception){
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Metodo get Obtener el catalogo de visitas segun la zona
        /// </summary>
        /// <returns>JsonResult</returns>
        public JsonResult CatalogoMotivo(string IdZona)
        {
            try
            {
                //Obtener la informacion de cliente
                List<CapaEntidad.Motivo> motivo = new List<CapaEntidad.Motivo>();
                motivo = NTurno.ListarVisitas(IdZona);
                return Json(new SelectList(motivo, "IdMotivo", "Descripcion"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Metodo get Obtener el catalogo de prioridades existentes
        /// </summary>
        /// <returns>JsonResult</returns>
        public JsonResult CatalogoPrioridad()
        {
            try{
                //Obtener la informacion de cliente
                List<CapaEntidad.Prioridad> prioridad = new List<CapaEntidad.Prioridad>();
                prioridad = NTurno.ListaPrioridad();
                return Json(new SelectList(prioridad, "IdPrioridad", "PrioridadDesc"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }



        /// <summary>
        /// permite configurar la exportación del reporte
        /// </summary>
        /// <param name="documentData">documento en binario</param>
        /// <param name="format">formato</param>
        /// <param name="isInline"></param>
        /// <param name="fileName">nombre el archivo a exportar</param>
        private void WriteDocumentToResponse(byte[] documentData, string format, bool isInline, string fileName)
        {
            string contentType;
            string disposition = (isInline) ? "inline" : "attachment";

            switch (format.ToLower())
            {
                case "xls":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "xlsx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "mht":
                    contentType = "message/rfc822";
                    break;
                case "html":
                    contentType = "text/html";
                    break;
                case "txt":
                case "csv":
                    contentType = "text/plain";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                default:
                    contentType = String.Format("application/{0}", format);
                    break;
            }

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", String.Format("{0}; filename={1}", disposition, fileName));
            Response.BinaryWrite(documentData);
            Response.End();
        }







    }
}