using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;
using System.Data;
using CapaEntidad;

namespace CapaNegocio
{

    /// <summary>
    /// capa de negocio
    /// con los reportes 
    /// existentes
    /// </summary>
    public class Reportes
    {
        //Crear la referencia a la capa de negocio con un objeto
        CapaDatos.SQLContext Access = new CapaDatos.SQLContext();


        /// <summary>
        /// Obtiene la lista total y su información
        /// </summary>
        /// <param name="IdTicket"></param>
        /// <returns></returns>
        public List<CapaEntidad.Ticket> ListaTicket(string IdTicket = null)
        {

            //crear lista
            List<CapaEntidad.Ticket> ticket = new List<Ticket>();
            //crear datatable
            DataTable dtTicket = new DataTable();
            string stringSQL = "";
            try
            {
                //ejecutar el procedimiento almacenado
                stringSQL = "EXEC spr_TicketLista";

                //obtener los datos del ticket
                dtTicket = Access.ObtenerDatos(stringSQL).Tables[0];
                if (dtTicket.Rows.Count > 0) {
                    ticket = dtTicket.AsEnumerable().Select(row => new Ticket {
                        Mov = row["IdSolicitud"].ToString(),
                        CodTicket = row["Ticket"].ToString(),

                        Cliente = new CapaEntidad.Cliente
                        {
                            IdCliente = row["IdCliente"].ToString()
                        },

                        Motivo = new Motivo
                        {
                            IdMotivo = Convert.ToInt32(row["IdMotivo"]),
                            Zona = new CapaEntidad.Zona
                            {
                                IdZona = row["IdZona"].ToString()
                            },
                            Descripcion = row["Descripcion"].ToString()
                        },

                        Observaciones = row["Observaciones"].ToString(),
                        FechaSolicitud = Convert.ToDateTime(row["FechaSolicitud"]).ToString("yyyy/MM/dd HH:mm:ss"),
                        Estado = Convert.ToInt32(Convert.ToInt32(row["EstadoSolicitud"])),
                        EstadoDesc = row["EstadoDesc"].ToString(),
                        PrioridadDesc = row["PrioridadDesc"].ToString(),

                        //si existe algun usuario que atendió el ticket
                        Usuario = new CapaEntidad.Usuario
                        {
                            Cuenta = row["Usuario"].ToString()
                        },
                        TiempoInicia = row["TiempoInicia"].ToString(),
                        TiempoFinaliza = row["TiempoFinaliza"].ToString(),
                    }).ToList();


                    //recorrer los elementos obtenidos en la consulta
                    //for (int i = 0; i < dtTicket.Rows.Count; i++) {
                    //    ticket.Add(new CapaEntidad.Ticket {
                    //        Mov = dtTicket.Rows[i]["IdSolicitud"].ToString(),
                    //        CodTicket = dtTicket.Rows[i]["Ticket"].ToString(),

                    //        Cliente = new CapaEntidad.Cliente{
                    //            IdCliente = dtTicket.Rows[i]["IdCliente"].ToString()
                    //        },

                    //        Motivo = new Motivo {
                    //            IdMotivo = Convert.ToInt32(dtTicket.Rows[i]["IdMotivo"]),
                    //            Zona = new CapaEntidad.Zona{
                    //                IdZona = dtTicket.Rows[i]["IdZona"].ToString()
                    //            },
                    //            Descripcion = dtTicket.Rows[i]["Descripcion"].ToString()
                    //        },

                    //        Observaciones = dtTicket.Rows[i]["Observaciones"].ToString(),
                    //        FechaSolicitud = Convert.ToDateTime(dtTicket.Rows[i]["FechaSolicitud"]).ToString("yyyy/MM/dd HH:mm:ss"),
                    //        Estado = Convert.ToInt32(Convert.ToInt32(dtTicket.Rows[i]["EstadoSolicitud"])),
                    //        EstadoDesc = dtTicket.Rows[i]["EstadoDesc"].ToString(),
                    //        PrioridadDesc = dtTicket.Rows[i]["PrioridadDesc"].ToString(),

                    //        //si existe algun usuario que atendió el ticket
                    //        Usuario = new CapaEntidad.Usuario{
                    //            Cuenta = dtTicket.Rows[i]["Usuario"].ToString()
                    //        },
                    //        TiempoInicia = dtTicket.Rows[i]["TiempoInicia"].ToString(),
                    //        TiempoFinaliza = dtTicket.Rows[i]["TiempoFinaliza"].ToString(),

                    //    });
                    //}
                }
            }
            catch (Exception ex){
                return null;
            }

            return ticket;


        }


        /// <summary>
        /// reportes de tickets atendidos por un 
        /// usuario en un año en especifico
        /// </summary>
        /// <param name="usuario">usuarios a buscar</param>
        /// <param name="anno">en un año en especifico</param>
        /// <returns>CapaEntidad.TicketsMensual con la informacion de tickets atendidos mensualmentes</returns>
        public CapaEntidad.TicketsMensual ReporteTickets_Mensual(int anno)
        {
            CapaEntidad.TicketsMensual reporte = new CapaEntidad.TicketsMensual();
            DataTable dt = new DataTable();
            string strSQL = "";

            try{
                strSQL = "EXEC spr_Reporte_TicketsMensuales " + anno;
                dt = Access.ObtenerDatos(strSQL).Tables[0];
                if(dt.Rows.Count > 0){
                    for (int i = 0; i < dt.Rows.Count; i++){
                        reporte.Usuario = (!dt.Columns.Contains("Usuario")) ? "" : dt.Rows[i]["Usuario"].ToString();
                        reporte.Total = Convert.ToInt32(dt.Rows[i]["Totales"]);

                        reporte.Anno = Convert.ToInt32(dt.Rows[i]["Anno"]);
                        reporte.Enero = Convert.ToInt32(dt.Rows[i]["Enero"]);
                        reporte.Febrero = Convert.ToInt32(dt.Rows[i]["Febrero"]);
                        reporte.Marzo = Convert.ToInt32(dt.Rows[i]["Marzo"]);
                        reporte.Abril = Convert.ToInt32(dt.Rows[i]["Abril"]);
                        reporte.Mayo = Convert.ToInt32(dt.Rows[i]["Mayo"]);
                        reporte.Junio = Convert.ToInt32(dt.Rows[i]["Junio"]);
                        reporte.Julio = Convert.ToInt32(dt.Rows[i]["Julio"]);
                        reporte.Agosto = Convert.ToInt32(dt.Rows[i]["Agosto"]);
                        reporte.Septiembre = Convert.ToInt32(dt.Rows[i]["Septiembre"]);
                        reporte.Octubre = Convert.ToInt32(dt.Rows[i]["Octubre"]);
                        reporte.Noviembre = Convert.ToInt32(dt.Rows[i]["Noviembre"]);
                        reporte.Diciembre = Convert.ToInt32(dt.Rows[i]["Diciembre"]);
                    }
                }
            }
            catch (Exception){
                return null;
            }

            return reporte;
        }

        /// <summary>
        /// reportes de tickets atendidos por usuario en un año en especifico
        /// </summary>
        /// <param name="anno"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public CapaEntidad.TicketsMensual ReporteTickets_MensualUsuario(int anno, string usuario)
        {
            CapaEntidad.TicketsMensual reporte = new CapaEntidad.TicketsMensual();
            DataTable dt = new DataTable();
            string strSQL = "";

            try
            {
                strSQL = "EXEC spr_Reporte_TicketsAtendidos_Mensual_Usuario '" + usuario + "', " + anno;
                dt = Access.ObtenerDatos(strSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        reporte.Usuario = (!dt.Columns.Contains("Usuario")) ? "" : dt.Rows[i]["Usuario"].ToString();
                        reporte.Total = Convert.ToInt32(dt.Rows[i]["Totales"]);

                        reporte.Anno = Convert.ToInt32(dt.Rows[i]["Anno"]);
                        reporte.Enero = Convert.ToInt32(dt.Rows[i]["Enero"]);
                        reporte.Febrero = Convert.ToInt32(dt.Rows[i]["Febrero"]);
                        reporte.Marzo = Convert.ToInt32(dt.Rows[i]["Marzo"]);
                        reporte.Abril = Convert.ToInt32(dt.Rows[i]["Abril"]);
                        reporte.Mayo = Convert.ToInt32(dt.Rows[i]["Mayo"]);
                        reporte.Junio = Convert.ToInt32(dt.Rows[i]["Junio"]);
                        reporte.Julio = Convert.ToInt32(dt.Rows[i]["Julio"]);
                        reporte.Agosto = Convert.ToInt32(dt.Rows[i]["Agosto"]);
                        reporte.Septiembre = Convert.ToInt32(dt.Rows[i]["Septiembre"]);
                        reporte.Octubre = Convert.ToInt32(dt.Rows[i]["Octubre"]);
                        reporte.Noviembre = Convert.ToInt32(dt.Rows[i]["Noviembre"]);
                        reporte.Diciembre = Convert.ToInt32(dt.Rows[i]["Diciembre"]);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return reporte;
        }


        /// <summary>
        /// reportes de tickets atendidos por un 
        /// areas en un año en especifico
        /// </summary>
        /// <param name="anno">año que se especifica en el reporte</param>
        /// <param name="area">segun el area</param>
        /// <returns></returns>
        public CapaEntidad.TicketsMensual ReporteTickets_MensualArea(int anno, string area)
        {
            CapaEntidad.TicketsMensual reporte = new CapaEntidad.TicketsMensual();
            DataTable dt = new DataTable();
            string strSQL = "";

            try
            {
                strSQL = "EXEC spr_Reporte_TicketsMensualesArea '" + anno + "', '" + area + "'";
                dt = Access.ObtenerDatos(strSQL).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        reporte.Usuario = (!dt.Columns.Contains("Usuario")) ? "" : dt.Rows[i]["Usuario"].ToString();
                        reporte.Total = Convert.ToInt32(dt.Rows[i]["Totales"]);

                        reporte.Anno = Convert.ToInt32(dt.Rows[i]["Anno"]);
                        reporte.Enero = Convert.ToInt32(dt.Rows[i]["Enero"]);
                        reporte.Febrero = Convert.ToInt32(dt.Rows[i]["Febrero"]);
                        reporte.Marzo = Convert.ToInt32(dt.Rows[i]["Marzo"]);
                        reporte.Abril = Convert.ToInt32(dt.Rows[i]["Abril"]);
                        reporte.Mayo = Convert.ToInt32(dt.Rows[i]["Mayo"]);
                        reporte.Junio = Convert.ToInt32(dt.Rows[i]["Junio"]);
                        reporte.Julio = Convert.ToInt32(dt.Rows[i]["Julio"]);
                        reporte.Agosto = Convert.ToInt32(dt.Rows[i]["Agosto"]);
                        reporte.Septiembre = Convert.ToInt32(dt.Rows[i]["Septiembre"]);
                        reporte.Octubre = Convert.ToInt32(dt.Rows[i]["Octubre"]);
                        reporte.Noviembre = Convert.ToInt32(dt.Rows[i]["Noviembre"]);
                        reporte.Diciembre = Convert.ToInt32(dt.Rows[i]["Diciembre"]);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return reporte;
        }

        /// <summary>
        /// obtiene el total de tickest en tiempo de atención agrupadas en intervalos de tiempos
        /// </summary>
        /// <param name="idarea"></param>
        /// <param name="fi"></param>
        /// <param name="ff"></param>
        /// <returns></returns>
        public TiempoAtencion_Area TiempoAtencion_Area(string idarea, string fi, string ff)
        {
            CapaEntidad.TiempoAtencion_Area tickets = new TiempoAtencion_Area();
            try {

                //obtener la lista
                List<Ticket> lista = new List<Ticket>();
                double seconds = 0, minutes = 0, hour = 0, tiempo = 0;

                //obtener la zona
                lista = this.ListaTicket().Where(x => x.Motivo.Zona.IdZona == idarea && (x.Estado == 3 || x.Estado == 4) &&
                                                Convert.ToDateTime(x.FechaSolicitud) >= Convert.ToDateTime(fi + " 00:00:00") &&
                                                Convert.ToDateTime(x.FechaSolicitud) <= Convert.ToDateTime(ff + " 23:59:59")).ToList();

                //crear los contadores
                tickets.Area = (lista.Count > 0) ? lista[0].Motivo.Zona.Descripcion : "No hay datos";
                tickets.IdArea = (lista.Count > 0) ? lista[0].Motivo.Zona.IdZona : "No hay datos";
                tickets.TotalTickets = lista.Count();
                tickets.min0_10 = 0;
                tickets.min10_30 = 0;
                tickets.min30_60 = 0;
                tickets.min60_max = 0;
                

                for (int i = 0; i < lista.Count; i++) {
                    var result = (Convert.ToDateTime(lista[i].TiempoFinaliza) - Convert.ToDateTime(lista[i].TiempoInicia));
                    seconds = result.Seconds;
                    minutes = result.Minutes;
                    hour = result.Hours;
                    tiempo = (seconds  / 60) + minutes + (hour * 60);


                    tickets.min0_10 += (tiempo <= 10) ? 1 : 0;
                    tickets.min10_30 += (tiempo > 10 && tiempo <= 30) ? 1 : 0;
                    tickets.min30_60 += (tiempo > 30 && tiempo <= 60) ? 1 : 0;
                    tickets.min60_max += (tiempo > 60) ? 1 : 0;
                }



                return tickets;
            }
            catch (Exception) {

                return null;
            }
        }


        /// <summary>
        /// tiempo distribuido por usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="fi"></param>
        /// <param name="ff"></param>
        /// <returns></returns>
        public TiempoAtencion_Usuario TiempoAtencion_Usuario(string usuario, string fi, string ff)
        {
            CapaEntidad.TiempoAtencion_Usuario tickets = new TiempoAtencion_Usuario();
            try
            {

                //obtener la lista
                List<Ticket> lista = new List<Ticket>();
                double seconds = 0, minutes = 0, hour = 0, tiempo = 0;

                //obtener la zona
                lista = this.ListaTicket().Where(x => x.Usuario.Cuenta == usuario && (x.Estado == 3 || x.Estado == 4) &&
                                                (Convert.ToDateTime(x.FechaSolicitud) > Convert.ToDateTime(fi + " 00:00:00") &&
                                                Convert.ToDateTime(x.FechaSolicitud) < Convert.ToDateTime(ff + " 23:59:59"))).ToList();

                //crear los contadores
                tickets.Area = (lista.Count > 0) ? lista[0].Motivo.Zona.Descripcion : "No hay datos";
                tickets.cuenta = (lista.Count > 0) ? lista[0].Usuario.Cuenta : "No hay datos";
                tickets.TotalTickets = lista.Count();
                tickets.min0_10 = 0;
                tickets.min10_30 = 0;
                tickets.min30_60 = 0;
                tickets.min60_max = 0;


                for (int i = 0; i < lista.Count; i++)
                {
                    var result = (Convert.ToDateTime(lista[i].TiempoFinaliza) - Convert.ToDateTime(lista[i].TiempoInicia));
                    seconds = result.Seconds;
                    minutes = result.Minutes;
                    hour = result.Hours;
                    tiempo = (seconds / 60) + minutes + (hour * 60);


                    tickets.min0_10 += (tiempo <= 10) ? 1 : 0;
                    tickets.min10_30 += (tiempo > 10 && tiempo <= 30) ? 1 : 0;
                    tickets.min30_60 += (tiempo > 30 && tiempo <= 60) ? 1 : 0;
                    tickets.min60_max += (tiempo > 60) ? 1 : 0;
                }



                return tickets;
            }
            catch (Exception)
            {

                return null;
            }
        }





    }
}
