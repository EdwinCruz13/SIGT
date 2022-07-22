using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaDatos;



namespace CapaNegocio
{

    
    public class Reporte
    {
        private SQLContext DBContext = new SQLContext();




        /// <summary>
        /// Return the created tickets according of the deparment
        /// </summary>
        /// <param name="day">recently day</param>
        /// <param name="month">recently month</param>
        /// <param name="year">recently year</param>
        /// <returns></returns>
        public CapaEntidad.PivotTicket_Areas TicketPorArea(int day, int month, int year)
        {
            CapaEntidad.PivotTicket_Areas pivot = new CapaEntidad.PivotTicket_Areas();
            String str = " EXEC spr_TicketArea_Pivot " + day + ","+ month + "," + year;
            DataTable dt = new DataTable();
            try{
                dt = new SQLContext().ObtenerDatos(str).Tables[0];

                if(dt.Rows.Count > 0){
                    pivot = dt.AsEnumerable().Select(row => new CapaEntidad.PivotTicket_Areas() { 
                        Prestamos = Convert.ToInt32(row["Prestamos"]),
                        Recuperaciones = Convert.ToInt32(row["Recuperaciones"]),
                        Total = Convert.ToInt32(row["Total"]),
                    }).ToList().FirstOrDefault();
                }
                else{
                    pivot.Prestamos = 0;
                    pivot.Recuperaciones = 0;
                    pivot.Total = 0;
                }

                return pivot;
            }
            catch (Exception ex){
                return null;
            }

        }

        /// <summary>
        /// return the pivot with created tickets according the atention
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public CapaEntidad.PivotTicket_Atencion TicketPorAtencion(int day, int month, int year)
        {
            CapaEntidad.PivotTicket_Atencion pivot = new CapaEntidad.PivotTicket_Atencion();
            String str = " EXEC spr_TicketAtencion_Pivot " + day + "," + month + "," + year;
            DataTable dt = new DataTable();
            try
            {
                dt = new SQLContext().ObtenerDatos(str).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    pivot = dt.AsEnumerable().Select(row => new CapaEntidad.PivotTicket_Atencion()
                    {
                        Anulados = Convert.ToInt32(row["Anulados"]),
                        Pendientes = Convert.ToInt32(row["Pendientes"]),
                        Asignados = Convert.ToInt32(row["Asignados"]),
                        Procesando = Convert.ToInt32(row["Procesando"]),
                        Atendidos = Convert.ToInt32(row["Atendidos"]),
                        Total = Convert.ToInt32(row["Total"]),
                    }).ToList().FirstOrDefault();
                }
                else
                {
                    pivot.Anulados = 0;
                    pivot.Pendientes = 0;
                    pivot.Asignados = 0;
                    pivot.Procesando = 0;
                    pivot.Atendidos = 0;
                    pivot.Total = 0;
                }

                return pivot;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// return tickets by user
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<CapaEntidad.PivotTicket_AtencionUsuario> TicketPorAtencion_Usuario(int day, int month, int year)
        {
            List <CapaEntidad.PivotTicket_AtencionUsuario> pivot = new List<CapaEntidad.PivotTicket_AtencionUsuario>();
            String str = "EXEC spr_TicketAtencionUsuario_Pivot " + day + "," + month + "," + year;
            DataTable dt = new DataTable();
            try
            {
                dt = new SQLContext().ObtenerDatos(str).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    pivot = dt.AsEnumerable().Select(row => new CapaEntidad.PivotTicket_AtencionUsuario()
                    {
                        Usuario = row["Usuario"].ToString(),
                        Anulados = Convert.ToInt32(row["Anulados"]),
                        Pendientes = Convert.ToInt32(row["Pendientes"]),
                        Asignados = Convert.ToInt32(row["Asignados"]),
                        Procesando = Convert.ToInt32(row["Procesando"]),
                        Atendidos = Convert.ToInt32(row["Atendidos"]),
                        Total = Convert.ToInt32(row["Total"]),
                    }).ToList();
                }
                else{
                    pivot = new List<CapaEntidad.PivotTicket_AtencionUsuario>();
                    pivot.Add(new CapaEntidad.PivotTicket_AtencionUsuario {
                        Usuario = "",
                        Anulados = 0,
                        Pendientes = 0,
                        Asignados = 0,
                        Procesando = 0,
                        Atendidos = 0,
                        Total = 0
                    });
                    
                }

                return pivot;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        /// <summary>
        /// return list of ticket according to the user and hours
        /// </summary>
        /// <param name="day"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<CapaEntidad.PivoteTicket_UsuarioHora> TicketPorAtencion_UsuarioHora(int day, int month, int year, string user)
        {
            List<CapaEntidad.PivoteTicket_UsuarioHora> pivot = new List<CapaEntidad.PivoteTicket_UsuarioHora>();
            String str = "EXEC spr_TicketAtencionUsuarioHora_Pivot " + day + "," + month + "," + year;
            DataTable dt = new DataTable();
            try
            {
                dt = new SQLContext().ObtenerDatos(str).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    pivot = dt.AsEnumerable().Select(row => new CapaEntidad.PivoteTicket_UsuarioHora()
                    {
                        Usuario = row["Usuario"].ToString(),
                        Area = row["Area"].ToString(),
                        _8 = Convert.ToInt32(row["8"]),
                        _9 = Convert.ToInt32(row["9"]),
                        _10 = Convert.ToInt32(row["10"]),
                        _11 = Convert.ToInt32(row["11"]),
                        _12 = Convert.ToInt32(row["12"]),
                        _13 = Convert.ToInt32(row["13"]),
                        _14 = Convert.ToInt32(row["14"]),
                        _15 = Convert.ToInt32(row["15"]),
                        _16 = Convert.ToInt32(row["16"]),
                        _17 = Convert.ToInt32(row["17"]),
                        Total = Convert.ToInt32(row["Total"]),
                    }).ToList();
                }
                else
                {
                    pivot = new List<CapaEntidad.PivoteTicket_UsuarioHora>();
                    pivot.Add(new CapaEntidad.PivoteTicket_UsuarioHora
                    {
                        Usuario = "",
                        Area = "",
                        _8 = 0,
                        _9 = 0,
                        _10 = 0,
                        _11 = 0,
                        _12 = 0,
                        _13 = 0,
                        _14 = 0,
                        _15 = 0,
                        _16 = 0,
                        _17 = 0,
                        Total = 0
                    });

                }


                return pivot.Where(x=> x.Usuario == user).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public List<CapaEntidad.PivoteTicket_UsuarioHora> TicketPorAtencion_AreaHora(int day, int month, int year)
        {
            List<CapaEntidad.PivoteTicket_UsuarioHora> pivot = new List<CapaEntidad.PivoteTicket_UsuarioHora>();
            String str = "EXEC spr_TicketAtencionAreaHora_Pivot " + day + "," + month + "," + year;
            DataTable dt = new DataTable();
            try
            {
                dt = new SQLContext().ObtenerDatos(str).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    pivot = dt.AsEnumerable().Select(row => new CapaEntidad.PivoteTicket_UsuarioHora()
                    {
                        Area = row["Area"].ToString(),
                        _8 = Convert.ToInt32(row["8"]),
                        _9 = Convert.ToInt32(row["9"]),
                        _10 = Convert.ToInt32(row["10"]),
                        _11 = Convert.ToInt32(row["11"]),
                        _12 = Convert.ToInt32(row["12"]),
                        _13 = Convert.ToInt32(row["13"]),
                        _14 = Convert.ToInt32(row["14"]),
                        _15 = Convert.ToInt32(row["15"]),
                        _16 = Convert.ToInt32(row["16"]),
                        _17 = Convert.ToInt32(row["17"]),
                        Total = Convert.ToInt32(row["Total"]),
                    }).ToList();
                }
                else
                {
                    pivot = new List<CapaEntidad.PivoteTicket_UsuarioHora>();
                    pivot.Add(new CapaEntidad.PivoteTicket_UsuarioHora
                    {
                        Usuario = "",
                        Area = "",
                        _8 = 0,
                        _9 = 0,
                        _10 = 0,
                        _11 = 0,
                        _12 = 0,
                        _13 = 0,
                        _14 = 0,
                        _15 = 0,
                        _16 = 0,
                        _17 = 0,
                        Total = 0
                    });

                }


                return pivot;
            }
            catch (Exception ex)
            {
                return null;
            }

        }



        /// <summary>
        /// Obtener el rendimiento de tiempo
        /// </summary>
        /// <param name="busqueda">tipo de filtro Area/Usuario </param>
        /// <param name="id">Usuario o area a buscar</param>
        /// <param name="fi">fecha de inicio de medición de rendimiento</param>
        /// <param name="ff">fecha de fin de medición de rendimiento</param>
        /// <returns>List<CapaEntidad.UsuarioRendimiento></returns>
        /// 
        public List<CapaEntidad.UsuarioRendimiento> Rendimiento(string filtro, string id, string fi, string ff)
        {
            List<CapaEntidad.UsuarioRendimiento> rendimiento = new List<CapaEntidad.UsuarioRendimiento>();
            try {

                string str = (filtro == "Area") ? "EXEC spr_Reporte_RendimientoArea_Fecha '" + id + "', '" + fi  + "', '" + ff + "'" : "EXEC spr_Reporte_RendimientoFecha '" + id + "', '" + fi + "', '" + ff + "'";

                DataTable dt = new DataTable();
                dt = new SQLContext().ObtenerDatos(str).Tables[0];
                if(dt.Rows.Count > 0) {

                    for (int i = 0; i < dt.Rows.Count; i++){
                        rendimiento.Add(new CapaEntidad.UsuarioRendimiento {
                            Id = dt.Rows[i]["Contador"].ToString(),
                            Usuario = dt.Rows[i]["Usuario"].ToString(),
                            Fecha = Convert.ToDateTime(dt.Rows[i]["Fecha"]),
                            NombreDia = dt.Rows[i]["NombreDia"].ToString(),
                            NumTickets = Convert.ToInt32(dt.Rows[i]["TicketsAtendidos"]),
                            Tiempo = Convert.ToDouble(dt.Rows[i]["TotalTiempo"]),
                            Promedio = Convert.ToDouble(dt.Rows[i]["Promedio"]),
                            TiempoRequerido = Convert.ToDouble(dt.Rows[i]["TiempoRequerido"]),
                            Rendimiento = Convert.ToDouble(dt.Rows[i]["Rendimiento"]),

                            Hora8 = Convert.ToDouble(dt.Rows[i]["8-9"]),
                            Hora9 = Convert.ToDouble(dt.Rows[i]["9-10"]),
                            Hora10 = Convert.ToDouble(dt.Rows[i]["10-11"]),
                            Hora11 = Convert.ToDouble(dt.Rows[i]["11-12"]),
                            Hora12 = Convert.ToDouble(dt.Rows[i]["12-13"]),
                            Hora13 = Convert.ToDouble(dt.Rows[i]["13-14"]),
                            Hora14 = Convert.ToDouble(dt.Rows[i]["14-15"]),
                            Hora15 = Convert.ToDouble(dt.Rows[i]["15-16"]),
                            Hora16 = Convert.ToDouble(dt.Rows[i]["16-17"]),
                            Hora17 = Convert.ToDouble(dt.Rows[i]["17-18"])
                        });
                    }
                }


                return rendimiento;
            }
            catch (Exception ex){
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CapaEntidad.AreaRendimiento RendimientoArea(string IdArea, string fi, string ff)
        {
            CapaEntidad.AreaRendimiento rendimiento = new CapaEntidad.AreaRendimiento();
            try  {
                rendimiento.area = new CapaNegocio.Zona().DetalleZona(IdArea).Descripcion;
                rendimiento.RendimientoUsuario = new List<List<CapaEntidad.UsuarioRendimiento>>();

                //Obtener la lista de usuarios
                List<CapaEntidad.Usuario> ListaUsuarios = new List<CapaEntidad.Usuario>();
                ListaUsuarios = new CapaNegocio.Usuario().ListarUsuarios().Where(x => x.Area.IdZona == IdArea && x.Estado == 1 && x.Tipo.IdTipo == 1 && (x.Cuenta != "ECRUZ" && x.Cuenta != "BCELEDON" && x.Cuenta != "NLACAYO" && x.Cuenta != "KGUADAMUZ")).ToList();

                //Obtener los usuarios de las areas correspondiente
                if(ListaUsuarios.Count > 0){
                    //recorremos la lista de usuarios
                    for (int i = 0; i < ListaUsuarios.Count; i++){
                        //añadimos el nuevo elemento
                        rendimiento.RendimientoUsuario.Add(this.Rendimiento("Usuario", ListaUsuarios[i].Cuenta, fi, ff));
                    }
                }
            }
            catch (Exception ex) {

                return null;
            }

            return rendimiento;
        }




        /// <summary>
        /// Obtiene la información de los tickets atendidos en un area determinada
        /// </summary>
        /// <param name="idarea">IdZona</param>
        /// <param name="fi">Fecha inicio a buscar</param>
        /// <param name="ff">Fecha Fin a buscar</param>
        /// <returns></returns>
        public List<CapaEntidad.UsuarioAtencionTickets> TotalTickets(string idarea, string fi, string ff)
        {
            //craer la lista para almacenar datos
            List<CapaEntidad.UsuarioAtencionTickets> lista = new List<CapaEntidad.UsuarioAtencionTickets>();

            //Crear datatable
            DataTable dt = new DataTable();
            String strTickets = "EXEC spr_Reporte_TicketsAtendidosFecha '" + idarea + "', '" + fi + "', '" + ff + "'";

            try{
                dt = DBContext.ObtenerDatos(strTickets).Tables[0];
                if(dt.Rows.Count > 0){
                    for (int i = 0; i < dt.Rows.Count; i++){
                        if (idarea == "01" || idarea == "03") {
                            lista.Add(new CapaEntidad.UsuarioAtencionTickets {
                                Usuario = dt.Rows[i]["Usuario"].ToString(),
                                IdArea = idarea,
                                AtencionPrestamos = new CapaEntidad.PivotePrestamo_Tickets {
                                    Area = idarea,
                                    Consulta = Convert.ToInt32(dt.Rows[i]["Consulta"]),
                                    EntregaPrestamoRRHH = Convert.ToInt32(dt.Rows[i]["EntregaPrestamoRRHH"]),
                                    TramitePrestamo = Convert.ToInt32(dt.Rows[i]["TramitePrestamo"]),
                                    Otros = Convert.ToInt32(dt.Rows[i]["Otros"]),
                                    SolicitudReestructuracion = Convert.ToInt32(dt.Rows[i]["SolicitudReestructuracion"]),
                                    TotalAdquirido = Convert.ToInt32(dt.Rows[i]["Totales"]),
                                },
                            });
                        }
                        if (idarea == "02") {
                            lista.Add(new CapaEntidad.UsuarioAtencionTickets {
                                Usuario = dt.Rows[i]["Usuario"].ToString(),
                                IdArea = idarea,
                                AtencionRecuperaciones = new CapaEntidad.PivoteRecuperaciones_Tickets {
                                    Area = idarea,
                                    Cancelacion = Convert.ToInt32(dt.Rows[i]["Cancelacion"]),
                                    TramiteReembolso = Convert.ToInt32(dt.Rows[i]["TramiteReembolso"]),
                                    CancelacionHipoteca = Convert.ToInt32(dt.Rows[i]["CancelacionHipoteca"]),
                                    Liquidacion = Convert.ToInt32(dt.Rows[i]["Liquidacion"]),
                                    Otros = Convert.ToInt32(dt.Rows[i]["Otros"]),
                                    TotalAdquirido = Convert.ToInt32(dt.Rows[i]["Totales"]),
                                }
                            });
                        }

                    }
                }


                return lista;
            }
            catch (Exception) {
                throw;
            }
            
        }


        /// <summary>
        /// Obtiene el tiempo total de atención por tickets atendidos
        /// </summary>
        /// <param name="idarea"></param>
        /// <param name="fi"></param>
        /// <param name="ff"></param>
        /// <returns></returns>
        public List<CapaEntidad.UsuarioAtencionTiempo> TotalTiempo(string idarea, string fi, string ff)
        {
            //craer la lista para almacenar datos
            List<CapaEntidad.UsuarioAtencionTiempo> lista = new List<CapaEntidad.UsuarioAtencionTiempo>();

            //Crear datatable
            DataTable dt = new DataTable();
            String strTickets = "EXEC spr_Reporte_TotalTiempoAtencionFecha '" + idarea + "', '" + fi + "', '" + ff + "'";

            try
            {
                dt = DBContext.ObtenerDatos(strTickets).Tables[1];
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (idarea == "01" || idarea == "03")
                        {
                            lista.Add(new CapaEntidad.UsuarioAtencionTiempo
                            {
                                Usuario = dt.Rows[i]["Usuario"].ToString(),
                                IdArea = idarea,
                                TiempoPrestamos = new CapaEntidad.PivotePrestamo_Tiempo
                                {
                                    Area = idarea,
                                    Consulta = dt.Rows[i]["Consultas"].ToString(),
                                    EntregaPrestamoRRHH = dt.Rows[i]["EntregaPrestamoRRHH"].ToString(),
                                    TramitePrestamo = dt.Rows[i]["TramitePrestamo"].ToString(),
                                    Otros = dt.Rows[i]["Otros"].ToString(),
                                    SolicitudReestructuracion = dt.Rows[i]["SolicitudReestructuracion"].ToString(),
                                    TotalAdquirido = dt.Rows[i]["TotalTiempo"].ToString(),
                                },
                            });
                        }
                        if (idarea == "02")
                        {
                            lista.Add(new CapaEntidad.UsuarioAtencionTiempo
                            {
                                Usuario = dt.Rows[i]["Usuario"].ToString(),
                                IdArea = idarea,
                                TiempoRecuperaciones = new CapaEntidad.PivoteRecuperaciones_Tiempo
                                {
                                    Area = idarea,
                                    Cancelacion = dt.Rows[i]["Cancelacion"].ToString(),
                                    TramiteReembolso = dt.Rows[i]["TramiteReembolso"].ToString(),
                                    CancelacionHipoteca = dt.Rows[i]["CancelacionHipoteca"].ToString(),
                                    Liquidacion = dt.Rows[i]["Liquidacion"].ToString(),
                                    Otros = dt.Rows[i]["Otros"].ToString(),
                                    TotalAdquirido = dt.Rows[i]["TotalTiempo"].ToString(),
                                }
                            });
                        }

                    }
                }


                return lista;
            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }
}
