using CapaEntidad;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIGT_FULL.Controllers
{
    public class ExpendedorController : Controller
    {

        //sesiones activas
        private System.Web.HttpContext Context = System.Web.HttpContext.Current;

        //Referencia a la capa de negocio de Turnos
        CapaNegocio.Turno NTurno = new CapaNegocio.Turno();


  

        // GET: Expendedor
        /// <summary>
        /// vista que muestra el expendedor de ticket
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public ActionResult Index(string IP)
        {

            try {
                //validar que contenga la ip
                if (IP == null)
                    return RedirectToAction("Index", "Home");

                //obtener la IP y almacenarlo en ViewBag
                ViewBag.IP = IP;
                return View();
            }
            catch (Exception) {
                return RedirectToAction("Index", "Home");
            }
        }


        /// <summary>
        /// seleccion de visita, recibira
        /// de parametros la cedula de la persona
        /// </summary>
        /// <returns></returns>
        public ActionResult SeleccionVisita(string cedula, string idcliente, string cliente, int tipo)
        {

            try{
                //guardar variables en viewbag
                ViewBag.Cedula = cedula;
                ViewBag.IdCliente = idcliente;
                ViewBag.Cliente = cliente;
                ViewBag.Tipo = tipo;

                //retornar la vista
                return View();
            }
            catch (Exception){
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// Visualizar reporte
        /// </summary>
        /// <param name="Ticket">Cod de ticket a buscars</param>
        /// <returns></returns>
        public ActionResult VisualizarTicket(string Ticket)
        {

            try {
                ViewBag.CodTicket = Ticket;
                return View();
            }
            catch (Exception){
                return RedirectToAction("Index", "Home");
            }
        }

        /// <summary>
        /// devolverá la vista parcial con el ticket
        /// </summary>
        /// <param name="Ticket"></param>
        /// <returns></returns>
        public ActionResult _Ticket(string Ticket)
        {
            ViewBag.CodTicket = Ticket;
            //Obtener la informacion del reporte y cargar el reporte
            var detalle = NTurno.DetalleTicket(Ticket);
            var report = new CapaReportes.Ticket(detalle);


            //Buscar ticket y cargar el reporte
            return PartialView("_Ticket", report);
        }

        /// <summary>
        /// exportar documentos
        /// </summary>
        /// <returns></returns>
        public ActionResult ReportPartialExport(string Ticket)
        {
            var report = new CapaReportes.Ticket(NTurno.DetalleTicket(Ticket));
            return DocumentViewerExtension.ExportTo(report, Request);
        }


        public ActionResult _VisualizarTicket(string Turno)
        {
            //obtener la informacion del ticket
            CapaEntidad.Ticket ticket = new CapaEntidad.Ticket();
            ticket = NTurno.DetalleTicket(Turno);

            //crear reporte
            var report = new CapaReportes.Ticket(ticket);
            //visualizar el reporte en la vista
            return PartialView(report);

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
                ticketGenerado = NTurno.GenerarTicket_kiosko(solicitud);

                //Si se genera el ticket, devolver los datos a la vista y cargar la pagina del reporte
                if (ticketGenerado != null && ticketGenerado.CodTicket != null){
                    //imprimir ticket, devolver resultado
                    return Json(NTurno.Impresion(ticketGenerado), JsonRequestBehavior.AllowGet);
                }
                else{
                    //devolver error
                    return Json("NO SE HA PODIDO REGISTRAR EL TICKET, POR FAVOR, CONSULTE A INFORMÁTICA, ERROR EN LA GENERACIÓN DE TICKET: => ", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex) {
                return Json("NO SE HA PODIDO REGISTRAR EL TICKET, POR FAVOR, CONSULTE A INFORMÁTICA, ERROR: => " + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        ///// aqui funciona

        ///// <summary>
        ///// visualización de la ticket
        ///// </summary>
        ///// <param name="Turno"></param>
        ///// <param name="Print"></param>
        ///// <returns></returns>
        //public ActionResult VisualizarTicket(string Turno, bool Print = true)
        //{
        //    try{
        //        //obtener la informacion del ticket
        //        CapaEntidad.Ticket ticket = new CapaEntidad.Ticket();
        //        ticket = NTurno.DetalleTicket(Turno);
        //        var report = new CapaReportes.Ticket(ticket);


        //        //XtraReport report = new XtraReport();
        //        using (System.IO.MemoryStream ms = new System.IO.MemoryStream()){
        //            report.ExportToPdf(ms, new PdfExportOptions() { ShowPrintDialogOnOpen = true });
        //            WriteDocumentToResponse(ms.ToArray(), "pdf", true, "Report.pdf");
        //        }

        //        //visualizar el reporte en la vista
        //        return View(report);
        //    }
        //    catch (Exception){
        //        return RedirectToAction("Index", "Home", new { usuario = Context.Session["User"].ToString() });
        //    }

        //}


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