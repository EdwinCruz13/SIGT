using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

using CapaEntidad;
using CapaDatos;
using System.Net;
using System.Net.Sockets;

namespace CapaNegocio
{
    public class Turno
    {
        //Crear la referencia a la capa de datos con un objeto
        CapaDatos.SQLContext Access = new CapaDatos.SQLContext();



        //referencia a otras capas de negocio
        CapaNegocio.Cliente NCliente = new CapaNegocio.Cliente();
        CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();

        //Crear la referencia a la capa de datos con un objeto
        CapaNegocio.Zona NZona = new CapaNegocio.Zona();



        /// <summary>
        /// Metodo para generar la ticket es estado de espera
        /// Recibe de parametro los campos enviados desde el formulario GenerarTicket
        /// </summary>
        /// <returns>Ticket Generado</returns>
        public CapaEntidad.Ticket GenerarTicket(Ticket solicitud)
        {
            CapaEntidad.Ticket ticket = new Ticket();
            DataTable dtTicket = new DataTable();
            string strConsulta = "";
            try{

                //crear la consulta
                /*strConsulta = "EXEC spr_TicketRegistrar '" + solicitud.Cliente.Cedula + "', '" + solicitud.Cliente.IdCliente + "', '" + solicitud.Cliente.NombreCompleto.Trim() + "', " + solicitud.Cliente.TipoCliente + ", " + solicitud.Motivo.IdMotivo + ", '" + solicitud.Motivo.Zona.IdZona + "','" + solicitud.Observaciones + "'";*/


                strConsulta = "EXEC spr_TicketRegistrar '" + solicitud.Cliente.Cedula + "', '" + solicitud.Cliente.NombreCompleto.Trim() + "', " + solicitud.Cliente.TipoCliente + ", " + solicitud.Motivo.IdMotivo + ", '" + solicitud.Motivo.Zona.IdZona + "', '" + solicitud.Cliente.IdCliente + "', '" + solicitud.Cliente.NoAsegurado + "', '" + solicitud.Cliente.CodEmpleado + "', '" + solicitud.Cliente.CodFiscal + "', '" + solicitud.Cliente.Pasaporte + "', '" + solicitud.Cliente.Sexo + "', '" + solicitud.Cliente.Id_Institucion + "', '" + solicitud.Cliente.FechaIngreso + "', '" + solicitud.Cliente.Cargo + "', '" + solicitud.Cliente.Ocupacion + "', '" + solicitud.Cliente.Id_TipoPension + "', " + solicitud.Cliente.Estado + ", '" + solicitud.Observaciones + "'";


                dtTicket = Access.ObtenerDatos(strConsulta).Tables[0];
                if (dtTicket.Rows.Count > 0){
                    for (int i = 0; i < dtTicket.Rows.Count; i++){
                        ticket.Mov = dtTicket.Rows[i]["IdSolicitud"].ToString();
                        ticket.CodTicket = dtTicket.Rows[i]["Ticket"].ToString();
                        ticket.Cliente = NCliente.DetalleCliente(dtTicket.Rows[i]["IdCliente"].ToString());
                        ticket.Motivo = new Motivo
                        {
                            IdMotivo = Convert.ToInt32(dtTicket.Rows[i]["IdMotivo"]),
                            Zona = NZona.DetalleZona(dtTicket.Rows[i]["IdZona"].ToString()),
                            Descripcion = dtTicket.Rows[i]["Descripcion"].ToString()
                        };
                        ticket.Observaciones = dtTicket.Rows[i]["Observaciones"].ToString();
                        ticket.FechaSolicitud = Convert.ToDateTime(dtTicket.Rows[i]["FechaSolicitud"]).ToString("yyyy/MM/dd HH:mm:ss");
                        ticket.Estado = Convert.ToInt32(Convert.ToInt32(dtTicket.Rows[i]["EstadoSolicitud"]));
                    }
                }


            }
            catch (Exception ex){
                return null;
            }


            return ticket;
        }


        /// <summary>
        /// Metodo para generar la ticket es estado de espera
        /// Recibe de parametro los campos enviados desde el formulario GenerarTicket
        /// </summary>
        /// <returns>Ticket Generado</returns>
        public CapaEntidad.Ticket GenerarTicket_kiosko(Ticket solicitud)
        {
            CapaEntidad.Ticket ticket = new Ticket();
            DataTable dtTicket = new DataTable();
            string strConsulta = "";
            try
            {

                //crear la consulta
                strConsulta = "EXEC spr_TicketRegistrar_Kiosko '" + solicitud.Cliente.Cedula + "', '" + solicitud.Cliente.IdCliente + "', '" + solicitud.Cliente.NombreCompleto.Trim() + "', " + solicitud.Cliente.TipoCliente + ", " + solicitud.Motivo.IdMotivo + ", '" + solicitud.Motivo.Zona.IdZona + "','" + solicitud.Observaciones + "'";

                dtTicket = Access.ObtenerDatos(strConsulta).Tables[0];
                if (dtTicket.Rows.Count > 0)
                {
                    for (int i = 0; i < dtTicket.Rows.Count; i++)
                    {
                        ticket.Mov = dtTicket.Rows[i]["IdSolicitud"].ToString();
                        ticket.CodTicket = dtTicket.Rows[i]["Ticket"].ToString();
                        ticket.Cliente = NCliente.DetalleCliente(dtTicket.Rows[i]["IdCliente"].ToString());
                        ticket.Motivo = new Motivo
                        {
                            IdMotivo = Convert.ToInt32(dtTicket.Rows[i]["IdMotivo"]),
                            Zona = NZona.DetalleZona(dtTicket.Rows[i]["IdZona"].ToString()),
                            Descripcion = dtTicket.Rows[i]["Descripcion"].ToString()
                        };
                        ticket.Observaciones = dtTicket.Rows[i]["Observaciones"].ToString();
                        ticket.FechaSolicitud = Convert.ToDateTime(dtTicket.Rows[i]["FechaSolicitud"]).ToString("yyyy/MM/dd HH:mm:ss");
                        ticket.Estado = Convert.ToInt32(Convert.ToInt32(dtTicket.Rows[i]["EstadoSolicitud"]));
                    }
                }


            }
            catch (Exception ex)
            {
                return null;
            }


            return ticket;
        }

        /// <summary>
        /// Metodo para observar la lista de ticket existentes 
        /// recibe como parametro opcionales
        /// </summary>
        /// <returns>Objeto de tipo ticket</returns>
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
                if (dtTicket.Rows.Count > 0){
                    //recorrer los elementos obtenidos en la consulta
                    ticket = dtTicket.AsEnumerable().Select(row => new Ticket
                    {
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




                    //for (int i = 0; i < dtTicket.Rows.Count; i++) {
                    //    ticket.Add(new CapaEntidad.Ticket {
                    //        Mov = dtTicket.Rows[i]["IdSolicitud"].ToString(),
                    //        CodTicket = dtTicket.Rows[i]["Ticket"].ToString(),

                    //        Cliente = new CapaEntidad.Cliente {
                    //            IdCliente = dtTicket.Rows[i]["IdCliente"].ToString()
                    //        },

                    //        Motivo = new Motivo{
                    //            IdMotivo = Convert.ToInt32(dtTicket.Rows[i]["IdMotivo"]),
                    //            Zona = NZona.DetalleZona(dtTicket.Rows[i]["IdZona"].ToString()),
                    //            Descripcion = dtTicket.Rows[i]["Descripcion"].ToString()
                    //        },

                    //        Observaciones = dtTicket.Rows[i]["Observaciones"].ToString(),
                    //        FechaSolicitud = Convert.ToDateTime(dtTicket.Rows[i]["FechaSolicitud"]).ToString("yyyy/MM/dd HH:mm:ss"),
                    //        Estado = Convert.ToInt32(Convert.ToInt32(dtTicket.Rows[i]["EstadoSolicitud"])),
                    //        EstadoDesc = dtTicket.Rows[i]["EstadoDesc"].ToString(),
                    //        PrioridadDesc = dtTicket.Rows[i]["PrioridadDesc"].ToString(),

                    //        //si existe algun usuario que atendió el ticket
                    //        Usuario = new CapaEntidad.Usuario {
                    //            Cuenta = dtTicket.Rows[i]["Usuario"].ToString()
                    //        }

                    //    });
                    //}
                }
            }
            catch (Exception){
                return null;
            }

            return ticket;


        }



        /// <summary>
        /// Metodo para observar la lista de ticket existentes del dia de hoy
        /// recibe como parametro opcionales
        /// </summary>
        /// <returns>Objeto de tipo ticket</returns>
        public List<CapaEntidad.Ticket> ListaTicket_Hoy()
        {
            //crear lista
            List<CapaEntidad.Ticket> ticket = new List<CapaEntidad.Ticket>();
            //crear datatable
            DataTable dtTicket = new DataTable();
            string stringSQL = "";

            String fechaInicial = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            String fechaFinal = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:00";
            try
            {
                //ejecutar el procedimiento almacenado
                stringSQL = "EXEC spr_TicketLista_Hoy";

                //obtener los datos del ticket
                dtTicket = Access.ObtenerDatos(stringSQL).Tables[0];
                if (dtTicket.Rows.Count > 0)
                {
                    //recorrer los elementos obtenidos en la consulta
                    for (int i = 0; i < dtTicket.Rows.Count; i++)
                    {
                        //insertar los elementos a la lista
                        ticket.Add(new CapaEntidad.Ticket
                        {
                            Mov = dtTicket.Rows[i]["IdSolicitud"].ToString(),
                            CodTicket = dtTicket.Rows[i]["Ticket"].ToString(),
                            Cliente = NCliente.DetalleCliente(dtTicket.Rows[i]["IdCliente"].ToString()),
                            Motivo = new Motivo
                            {
                                IdMotivo = Convert.ToInt32(dtTicket.Rows[i]["IdMotivo"]),
                                Zona = NZona.DetalleZona(dtTicket.Rows[i]["IdZona"].ToString()),
                                Descripcion = dtTicket.Rows[i]["Descripcion"].ToString()
                            },
                            Observaciones = dtTicket.Rows[i]["Observaciones"].ToString(),
                            FechaSolicitud = Convert.ToDateTime(dtTicket.Rows[i]["FechaSolicitud"]).ToString("yyyy/MM/dd HH:mm:ss"),


                            Estado = Convert.ToInt32(Convert.ToInt32(dtTicket.Rows[i]["EstadoSolicitud"])),
                            EstadoDesc = dtTicket.Rows[i]["EstadoDesc"].ToString(),
                            PrioridadDesc = dtTicket.Rows[i]["PrioridadDesc"].ToString(),

                            //si existe algun usuario que atendió el ticket
                            Usuario = NUsuario.DetalleUsuario(dtTicket.Rows[i]["Usuario"].ToString())

                        });

                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return ticket;
        }




        /// <summary>
        /// Metodo para observar el detalle del ticket
        /// recibe como parametro el ID ticket a visualizar
        /// </summary>
        /// <returns>Objeto de tipo ticket</returns>
        public CapaEntidad.Ticket DetalleTicket(string IdTicket = null)
        {
            CapaEntidad.Ticket ticket = new CapaEntidad.Ticket();
            DataTable dtTicket = new DataTable();
            string stringSQL = "";
            try{
                //ejecutar el procedimiento almacenado
                stringSQL = "EXEC spr_TicketLista '" + IdTicket + "'";

                //obtener los datos del ticket
                dtTicket = Access.ObtenerDatos(stringSQL).Tables[0];
                if (dtTicket.Rows.Count > 0){
                    for (int i = 0; i < dtTicket.Rows.Count; i++){
                        ticket.Mov = dtTicket.Rows[i]["IdSolicitud"].ToString();
                        ticket.CodTicket = dtTicket.Rows[i]["Ticket"].ToString();
                        ticket.Cliente = NCliente.DetalleCliente(dtTicket.Rows[i]["IdCliente"].ToString());
                        ticket.Motivo = new Motivo{
                            IdMotivo = Convert.ToInt32(dtTicket.Rows[i]["IdMotivo"]),
                            Zona = NZona.DetalleZona(dtTicket.Rows[i]["IdZona"].ToString()),
                            Descripcion = dtTicket.Rows[i]["Descripcion"].ToString(),
                            Prioridad = Convert.ToInt32(dtTicket.Rows[i]["IdPrioridad"])
                        };
                        ticket.Observaciones = dtTicket.Rows[i]["Observaciones"].ToString();
                        ticket.FechaSolicitud = Convert.ToDateTime(dtTicket.Rows[i]["FechaSolicitud"]).ToString("yyyy/MM/dd HH:mm:ss");
                        ticket.Estado = Convert.ToInt32(Convert.ToInt32(dtTicket.Rows[i]["EstadoSolicitud"]));
                        ticket.EstadoDesc = dtTicket.Rows[i]["EstadoDesc"].ToString();
                        ticket.PrioridadDesc = dtTicket.Rows[i]["PrioridadDesc"].ToString();

                        //si existe algun usuario que atendió el ticket
                        ticket.Usuario = NUsuario.DetalleUsuario(dtTicket.Rows[i]["Usuario"].ToString());
                    }
                }
            }
            catch (Exception){
                return null;
            }

            return ticket;
        }

        /// <summary>
        /// metodo que permite actualizar la prioridad y estado del ticket
        /// retornará true o false el éxito de la solicitud
        /// </summary>
        /// <returns>boolean</returns>
        public bool ModificarTicket(int ID, string CodTicket, int Valor, string Usuario)
        {
            try{
                //realizar el proceso de modificacion del ticket, enviará el Id y CodTicket a modificar, la accion a ejecutar y el usuario y sistema donde
                //se realizó la accion
                Access.EjecutarProcedimiento("spr_TicketActualizar", ID, CodTicket, Valor, Usuario);
                return true;
            }
            catch (Exception){
                return false;
            }
        }


        /// <summary>
        /// para obtener la lista de ticket de acuerdo al dia
        /// recibe como parametro opcionales
        /// </summary>
        /// <returns>Objeto de tipo ticket</returns>
        public List<CapaEntidad.Ticket> TicketFecha(string Inicio, string Finalizacion, int EstadoSolicitud)
        {
            //crear lista
            List<CapaEntidad.Ticket> ticket = new List<CapaEntidad.Ticket>();
            //crear datatable
            DataTable dtTicket = new DataTable();
            string stringSQL = "";
            try
            {
                //ejecutar el procedimiento almacenado segun los parametros de entrada
                stringSQL = "EXEC spr_TicketLista_RangoFecha '" + Inicio + "','" + Finalizacion + "', '" + EstadoSolicitud + "'";

                //obtener los datos del ticket
                dtTicket = Access.ObtenerDatos(stringSQL).Tables[0];
                if (dtTicket.Rows.Count > 0)
                {
                    //recorrer los elementos obtenidos en la consulta
                    for (int i = 0; i < dtTicket.Rows.Count; i++)
                    {
                        //insertar los elementos a la lista
                        ticket.Add(new CapaEntidad.Ticket
                        {
                            Mov = dtTicket.Rows[i]["IdSolicitud"].ToString(),
                            CodTicket = dtTicket.Rows[i]["Ticket"].ToString(),
                            Cliente = NCliente.DetalleCliente(dtTicket.Rows[i]["IdCliente"].ToString()),
                            Motivo = new Motivo
                            {
                                IdMotivo = Convert.ToInt32(dtTicket.Rows[i]["IdMotivo"]),
                                Zona = NZona.DetalleZona(dtTicket.Rows[i]["IdZona"].ToString()),
                                Descripcion = dtTicket.Rows[i]["Descripcion"].ToString()
                            },
                            Observaciones = dtTicket.Rows[i]["Observaciones"].ToString(),
                            FechaSolicitud = Convert.ToDateTime(dtTicket.Rows[i]["FechaSolicitud"]).ToString("yyyy/MM/dd HH:mm:ss"),
                            Estado = Convert.ToInt32(dtTicket.Rows[i]["EstadoSolicitud"]),
                            EstadoDesc = dtTicket.Rows[i]["EstadoDesc"].ToString(),
                            PrioridadDesc = dtTicket.Rows[i]["PrioridadDesc"].ToString(),

                            //si existe algun usuario que atendió el ticket
                            Usuario = NUsuario.DetalleUsuario(dtTicket.Rows[i]["Usuario"].ToString())
                        });

                    }
                }
            }
            catch (Exception){
                return null;
            }

            return ticket;
        }


        /// <summary>
        /// Metodo que devuelve los tipo de visita(Motivo) por áreas
        /// Recibe de parametro el ID de zona a las que estan relacionadas distinto tipos
        /// de visitas
        /// </summary>
        /// <returns>List</returns>
        public List<CapaEntidad.Motivo> ListarVisitas(string IdZona = null)
        {

            List<CapaEntidad.Motivo> visita = new List<Motivo>();
            DataTable dtVisita = new DataTable();
            try
            {
                dtVisita = Access.ObtenerDatos("spr_MotivoListar_Zonas '" + IdZona + "'").Tables[0];
                if (dtVisita.Rows.Count > 0){
                    for (int i = 0; i < dtVisita.Rows.Count; i++){
                        visita.Add(new CapaEntidad.Motivo{
                            IdMotivo = Convert.ToInt32(dtVisita.Rows[i]["IdMotivo"]),
                            Zona = NZona.DetalleZona(IdZona),
                            Descripcion = dtVisita.Rows[i]["Descripcion"].ToString()
                        });
                    }
                }
                return visita;

            }
            catch (Exception){
                return null;
            }

        }


        /// <summary>
        /// Metodo de ListarPrioridad
        /// </summary>
        /// <returns>Devolverá una lista con las prioridades existentes en bd</returns>
        public List<CapaEntidad.Prioridad> ListaPrioridad()
        {
            //crear lista
            List<CapaEntidad.Prioridad> Lista = new List<CapaEntidad.Prioridad>();
            DataTable dtLista = new DataTable();
            try{
                //ejecutar la consulta
                dtLista = Access.ObtenerDatos("SELECT * FROM tblTurno_Prioridad").Tables[0];
                if (dtLista.Rows.Count > 0){
                    for (int i = 0; i < dtLista.Rows.Count; i++){
                        //añadir los elementos a la lista
                        Lista.Add(new CapaEntidad.Prioridad {
                            IdPrioridad = Convert.ToInt32(dtLista.Rows[i]["IdPrioridad"]),
                            PrioridadDesc = dtLista.Rows[i]["Prioridad"].ToString(),
                            Siglas = dtLista.Rows[i]["Siglas"].ToString()
                        });
                    }
                }
                return Lista;

            }
            catch (Exception){
                return null;
            }

        }

        /// <summary>
        /// impresión automatica sin vista previa
        /// </summary>
        /// <param name="reporte">xtrareport con el formato de reporte</param>
        public String Impresion(Ticket _Ticket)
        {

            try{

                ///crear el reporte
                CapaReportes.Ticket report = new CapaReportes.Ticket(_Ticket);

                //parametros del reporte (sin advertencia de margenes y sin ventana de selección de impresora)
                report.ShowPrintMarginsWarning = false;
                report.ShowPrintStatusDialog = false;


                //obtener la descripcion de la impresora
                DataTable dtImpresion = new DataTable();
                CapaEntidad.Impresora impresora = new Impresora();
                dtImpresion = new CapaDatos.SQLContext().ObtenerDatos("SELECT * FROM tblPrinter WHERE Status = 1").Tables[0];
                if(dtImpresion.Rows.Count > 0){
                    impresora = dtImpresion.AsEnumerable().Select(row => new CapaEntidad.Impresora {
                        Id = Convert.ToInt32(row["IdDevice"]),
                        Descripcion = row["Description"].ToString().Trim(),
                        IP = row["IP"].ToString().Trim(),
                        Status = Convert.ToBoolean(row["Status"]),
                        Mac = row["Mac"].ToString()
                        
                    }).ToList().FirstOrDefault();



                    //impresión por dispositivo activo
                    //imprimir reporte
                    DevExpress.XtraReports.UI.ReportPrintTool rpt = new DevExpress.XtraReports.UI.ReportPrintTool(report);
                    rpt.Print("\\\\" + impresora.IP + "\\" + impresora.Descripcion + "");

                    return "true";

                }

                //por defecto
                //else{
                //    //imprimir reporte
                //    DevExpress.XtraReports.UI.ReportPrintTool rpt = new DevExpress.XtraReports.UI.ReportPrintTool(report);
                //    rpt.Print("\\\\192.168.1.195\\pos-80c (copy 1)");
                //}

                return false.ToString();
                
            }
            catch (Exception ex) {
                return ex.Message;
            }
  
        }

        /// <summary>
        /// obtener la ip
        /// </summary>
        /// <returns></returns>
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }
}
