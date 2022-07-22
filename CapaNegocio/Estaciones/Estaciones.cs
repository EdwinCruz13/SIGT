using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using CapaEntidad;
using CapaDatos;

namespace CapaNegocio
{
    public class Estaciones
    {

        //Crear la referencia a la capa de negocio con un objeto
        CapaDatos.SQLContext Access = new CapaDatos.SQLContext();

        //referencia a las demas capas de negocio
        CapaNegocio.Turno NTurno = new CapaNegocio.Turno();
        //referencia a las demas capas de negocio
        CapaNegocio.Zona NZona = new CapaNegocio.Zona();
        //referencia a las demas capas de negocio
        CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();




        /// <summary>
        /// Metodo que permite activar la estacion de trabajo activa
        /// USADO EN EL HUB
        /// </summary>
        /// <returns>si la accion fue realizada</returns>
        public CapaEntidad.EstacionTrabajo ControlEstacion(string user, string ip, int estado)
        {
            CapaEntidad.EstacionTrabajo Estacion = new CapaEntidad.EstacionTrabajo();
            DataTable dtEstacion = new DataTable();

            try
            {
                dtEstacion = Access.ObtenerDatos("EXEC spr_EstacionActivar '" + user + "','" + ip + "'," + estado).Tables[0];
                if (dtEstacion.Rows.Count > 0)
                {
                    for (int i = 0; i < dtEstacion.Rows.Count; i++)
                    {
                        Estacion.nAsignacion = Convert.ToInt32(dtEstacion.Rows[i]["nAsignacion"]);
                        Estacion.Cuenta = dtEstacion.Rows[i]["Usuario"].ToString();
                        Estacion.CodTicket = dtEstacion.Rows[i]["CodTicket"].ToString();
                        Estacion.Ticket = dtEstacion.Rows[i]["Ticket"].ToString().Replace("A", "A-").Replace("B", "B-");
                        Estacion.Turno = NTurno.DetalleTicket(dtEstacion.Rows[i]["CodTicket"].ToString());
                        Estacion.Usuario = NUsuario.DetalleUsuario(dtEstacion.Rows[i]["Usuario"].ToString());
                        Estacion.Usuario.IP = ip; //Ip donde se conecta a la estacion
                        Estacion.Estado = Convert.ToInt32(dtEstacion.Rows[i]["Estado"]);
                        Estacion.FechaAsignacion = dtEstacion.Rows[i]["FechaAsignación"].ToString();
                        Estacion.NAtendidos = Convert.ToInt32(dtEstacion.Rows[i]["nAtendidos"]);
                        Estacion.NombreEstacion = dtEstacion.Rows[i]["NombreEstacion"].ToString();
                        Estacion.IpEstacion = dtEstacion.Rows[i]["IpEstacion"].ToString();
                        Estacion.CodEstacion = Convert.ToInt32(dtEstacion.Rows[i]["IdEstacion"]);

                        Estacion.Institucion = dtEstacion.Rows[i]["Institucion"].ToString();

                        //tipo de asignacion {1 manual, 0 automatica}
                        Estacion.IdAsignacion = Convert.ToInt32(dtEstacion.Rows[i]["IdTipoAsignacion"]);
                        Estacion.TipoAsignacion = dtEstacion.Rows[i]["Descripcion"].ToString();

                    }

                    return Estacion;
                }

                return null;
            }
            catch (Exception)
            {

                return null;
            }
        }

        //Finalizada la atencion al cliente, enviar las observaciones a la estación
        //retornara true o false si completa la solicitud
        public bool ActualizarAtencion(string IdTicket, string Ticket, string tiempo, string observaciones = "")
        {
            bool flag = false;
            //variables que se utilizaran ejecucion del procedimiento almacenado de inserccion de tramite
            IList<object> parametros;
            int varSalida = 0;

            try{
                parametros = (IList<object>)Access.EjecutarProcedimiento("spr_TurnoAtencion_Actualizar", varSalida, Convert.ToInt32(IdTicket), Ticket, tiempo, observaciones);
                varSalida = Convert.ToInt32(parametros[0]);

                //retornar el resultado del procedimiento
                flag = (varSalida == 1) ? true : false;
                return flag;
            }
            catch (Exception){

                return flag;
            }
        }


        
        /// <summary>
        /// Lista de estaciones Activa
        /// CONTROLADOR Atencion
        /// </summary>
        /// <returns>sDevolvera una lista con las estaciones activas</returns>
        public List<CapaEntidad.Estacion> ListaEstacionesActiva(string id = null)
        {
            //crear la lista
            List<CapaEntidad.Estacion> lista = new List<Estacion>();
            DataTable dtLista = new DataTable();

            //crear la consulta
            string strSQL = (id != null) ? "EXEC spr_EstacionesActiva_Lista " + id : "EXEC spr_EstacionesActiva_Lista";

            try
            {
                //acceder a la capa de datos y almacenar los registro en los datatables
                dtLista = Access.ObtenerDatos(strSQL).Tables[0];
                if (dtLista.Rows.Count > 0)
                {
                    //agregar los elementos a la lista
                    for (int i = 0; i < dtLista.Rows.Count; i++)
                    {
                        lista.Add(new Estacion
                        {
                            IdEstacion = Convert.ToInt32(dtLista.Rows[i]["IdEstacion"]),
                            Estado = dtLista.Rows[i]["Estado"].ToString(),
                            NombreEstacion = dtLista.Rows[i]["NombreEstacion"].ToString(),
                            IpLocal = dtLista.Rows[i]["IP_Local"].ToString(),
                            Area = NZona.DetalleZona(dtLista.Rows[i]["IdZona"].ToString()),
                            UsuarioP = dtLista.Rows[i]["Usuario"].ToString()
                        });
                    }
                }
            }
            catch (Exception){
                return null;
            }

            //retornar lista
            return lista;
        }


        /// <summary>
        /// Lista de estaciones Activa por IP
        /// HUB UsuarioHub
        /// </summary>
        /// <returns>sDevolvera una lista con las estaciones activas</returns>
        public CapaEntidad.Estacion ListaEstacionesActiva_IP(string ip = null)
        {
            //crear la lista
            CapaEntidad.Estacion elemento = new Estacion();
            DataTable dtLista = new DataTable();

            //crear la consulta
            string strSQL = (ip != null) ? "EXEC spr_EstacionesActiva_ListaIP '" + ip + "'": "EXEC spr_EstacionesActiva_ListaIP";

            try
            {
                //acceder a la capa de datos y almacenar los registro en los datatables
                dtLista = Access.ObtenerDatos(strSQL).Tables[0];
                if (dtLista.Rows.Count > 0)
                {
                    //agregar los elementos a la lista
                    for (int i = 0; i < dtLista.Rows.Count; i++)
                    {
                        elemento.IdEstacion = Convert.ToInt32(dtLista.Rows[i]["IdEstacion"]);
                        elemento.Estado = dtLista.Rows[i]["Estado"].ToString();
                        elemento.NombreEstacion = dtLista.Rows[i]["NombreEstacion"].ToString();
                        elemento.IpLocal = dtLista.Rows[i]["IP_Local"].ToString();
                        elemento.Area = NZona.DetalleZona(dtLista.Rows[i]["IdZona"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            //retornar lista
            return elemento;
        }

        
        /// <summary>
        /// Lista de estaciones Activa
        /// CONTROLADOR Atencion
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public CapaEntidad.EstacionTrabajo DetalleEstacionesActiva(string user, string ip = null)
        {
            //crear la lista
            CapaEntidad.EstacionTrabajo info = new CapaEntidad.EstacionTrabajo();
            DataTable dtLista = new DataTable();

            //crear la consulta
            //to-do hacer procedimiento almacenado
            //string strSQL = "SELECT * FROM tblTurno_Asignacion asig INNER JOIN tblEstacion estacion ON estacion.IdEstacion = asig.IdEstacion INNER JOIN tblTurno_EstacionTipoAsignacion tipo ON asig.IdTipoAsignacion = tipo.IdAsignacion WHERE Usuario = '" + user + "' OR IpEstacion = '" + ip + "' ORDER BY asig.FechaAsignación DESC";
            string strSQL = "SELECT asig.*, estacion.*, tipo.*,  institucion.Siglas Institucion FROM tblTurno_Asignacion asig INNER JOIN tblEstacion estacion ON estacion.IdEstacion = asig.IdEstacion INNER JOIN tblTurno_EstacionTipoAsignacion tipo ON asig.IdTipoAsignacion = tipo.IdAsignacion LEFT JOIN InversionesNet.dbo.tblClientes cliente ON asig.IdCliente = cliente.Id_Cliente LEFT JOIN InversionesNet.dbo.tblClientes_Instituciones institucion ON cliente.Id_Institucion = institucion.Id_Institucion WHERE Usuario = '" + user + "' ORDER BY asig.FechaAsignación DESC";


            try{
                //acceder a la capa de datos y almacenar los registro en los datatables
                dtLista = Access.ObtenerDatos(strSQL).Tables[0];
                //asumiendo que existe en tblTurnoAsignacion, sino existe crearlo en estado -1
                if (dtLista.Rows.Count > 0)
                {
                    //agregar los elementos a la lista
                    for (int i = 0; i < dtLista.Rows.Count; i++)
                    {

                        info.nAsignacion = Convert.ToInt32(dtLista.Rows[i]["nAsignacion"]);
                        info.Cuenta = dtLista.Rows[i]["Usuario"].ToString();
                        info.Usuario = NUsuario.DetalleUsuario(dtLista.Rows[i]["Usuario"].ToString());
                        info.Estado = Convert.ToInt32(dtLista.Rows[i]["Estado"]);
                        info.IpEstacion = dtLista.Rows[i]["IpEstacion"].ToString();
                        info.CodEstacion = Convert.ToInt32(dtLista.Rows[i]["IdEstacion"]);
                        info.NombreEstacion = dtLista.Rows[i]["NombreEstacion"].ToString();
                        info.FechaAsignacion = Convert.ToDateTime(dtLista.Rows[i]["FechaAsignación"]).ToString();


                        info.Ticket = (dtLista.Rows[i]["Ticket"] == null) ? null : dtLista.Rows[i]["Ticket"].ToString();
                        info.CodTicket = (dtLista.Rows[i]["CodTicket"] == null || dtLista.Rows[i]["CodTicket"].ToString() == "") ? null : dtLista.Rows[i]["CodTicket"].ToString();

                        info.Institucion = (dtLista.Rows[i]["Institucion"] == null) ? "Sin referencia" : dtLista.Rows[i]["Institucion"].ToString();

                        info.Turno = (dtLista.Rows[i]["CodTicket"] == null) ? null : NTurno.DetalleTicket(dtLista.Rows[i]["CodTicket"].ToString());


                        //tipo de asignacion {1 manual, 0 automatica}
                        info.IdAsignacion = Convert.ToInt32(dtLista.Rows[i]["IdTipoAsignacion"]);
                        info.TipoAsignacion = dtLista.Rows[i]["Descripcion"].ToString();
                        
                        
                    }
                }

            }
            catch (Exception ex){
                return null;
            }

            //retornar lista
            return info;
        }



        /// <summary>
        /// Lista de estaciones Activa
        /// CONTROLADOR Atencion
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<CapaEntidad.EstacionTrabajo> DetalleEstacionesActiva()
        {
            //crear la lista
            List<CapaEntidad.EstacionTrabajo> info = new List<CapaEntidad.EstacionTrabajo>();
            DataTable dtLista = new DataTable();

            //crear la consulta
            string strSQL = "SELECT * FROM tblTurno_Asignacion asig INNER JOIN tblEstacion estacion ON estacion.IdEstacion = asig.IdEstacion INNER JOIN tblTurno_EstacionTipoAsignacion tipo ON asig.IdTipoAsignacion = tipo.IdAsignacion ORDER BY asig.FechaAsignación DESC";


            try{
                //acceder a la capa de datos y almacenar los registro en los datatables
                dtLista = Access.ObtenerDatos(strSQL).Tables[0];
                //asumiendo que existe en tblTurnoAsignacion, sino existe crearlo en estado -1
                if (dtLista.Rows.Count > 0) {
                    //agregar los elementos a la lista
                    for (int i = 0; i < dtLista.Rows.Count; i++){
                        info.Add(new EstacionTrabajo {
                            nAsignacion = Convert.ToInt32(dtLista.Rows[i]["nAsignacion"]),
                            Cuenta = dtLista.Rows[i]["Usuario"].ToString(),
                            Usuario = NUsuario.DetalleUsuario(dtLista.Rows[i]["Usuario"].ToString()),
                            Estado = Convert.ToInt32(dtLista.Rows[i]["Estado"]),
                            IpEstacion = dtLista.Rows[i]["IpEstacion"].ToString(),
                            CodEstacion = Convert.ToInt32(dtLista.Rows[i]["IdEstacion"]),
                            NombreEstacion = dtLista.Rows[i]["NombreEstacion"].ToString(),
                            FechaAsignacion = Convert.ToDateTime(dtLista.Rows[i]["FechaAsignación"]).ToString(),


                            Ticket = (dtLista.Rows[i]["Ticket"] == null) ? null : dtLista.Rows[i]["Ticket"].ToString(),
                            CodTicket = (dtLista.Rows[i]["CodTicket"] == null || dtLista.Rows[i]["CodTicket"].ToString() == "") ? null : dtLista.Rows[i]["CodTicket"].ToString(),
                            Turno = (dtLista.Rows[i]["CodTicket"] == null) ? null : NTurno.DetalleTicket(dtLista.Rows[i]["CodTicket"].ToString()),


                            //tipo de asignacion {1 manual, 0 automatica}
                            IdAsignacion = Convert.ToInt32(dtLista.Rows[i]["IdTipoAsignacion"]),
                            TipoAsignacion = dtLista.Rows[i]["Descripcion"].ToString()

                        });
                    }
                }

            }
            catch (Exception)
            {
                return null;
            }

            //retornar lista
            return info;
        }



        /// <summary>
        /// Crear estaciones y asignar los motivos
        /// asignado a atender
        /// </summary>
        /// <param name="Obj">entidad con la informacion de la estacion</param>
        /// <param name="motivo">lista de tipos de visitas a atender</param>
        /// <returns>true o false si se ejecutó correctamente el proceso</returns>
        public bool CrearEstacion(CapaEntidad.EstacionMotivos Obj)
        {
            //Crear lista para insertar la cadena de consulta que se ejecutara en transaccion
            List<String> Cadena = new List<String>();

            //colecciones genericas de parametros devueltos
            IList<object> collection;
            int Salida = 0; //valor devuelto del procedimiento almacenado
            try{
                
                ///////guardar la estacion y obtener el IdEstacion Insertado///////
                collection = (IList<object>)Access.EjecutarProcedimiento("spr_EstacionCrear", Salida, Obj.Estacion.IpLocal, Obj.Estacion.Area.IdZona);
                //obtener el valor de salida
                Salida = Convert.ToInt32(collection[0]); //devolvera la estacion creada
                //////////////////////////////////////////////////////////////////

                //Insertar la cadena para insertar en tblEstacion_MotivoAsignado
                //recorrer la lista
                for (int i = 0; i < Obj.Motivos.Count(); i++)
                {
                    Cadena.Add("EXEC spr_EstacionAsignar_TurnoDefinidos '" + Salida + "','" + Obj.Estacion.IpLocal + "','" + Obj.Motivos[i].IdMotivo + "','" + Obj.Motivos[i].Zona.IdZona + "','" + Obj.Motivos[i].Asignado + "'");
                }

                //ejecutar la transaccion
                return Access.EjecutarTransaccion(Cadena);
            }
            catch (Exception){
                //en el caso de error eliminar la estación
                Access.EjecutarProcedimiento("spr_EstacionEliminar", Salida);
                return false;
            }
        }

        /// <summary>
        /// funcion que permite retirar al usuario de la estacion
        /// cuando se deslogea de la estación
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool LimpiarEstacion_Trabajo(string ip)
        {
            //colecciones genericas de parametros devueltos
            IList<object> collection;
            int Salida = 0; //valor devuelto del procedimiento almacenado

            try{
                collection = (IList<object>) Access.EjecutarProcedimiento("spr_EstacionLimpiar_Sesion", Salida, ip);
                //obtener el valor de salida
                Salida = Convert.ToInt32(collection[0]); //devolvera la estacion creada
            }
            catch (Exception){
                return false; 
            }

            return true;
        }


        /// <summary>
        /// Metodo que permite devolver la lista de estaciones existentes
        /// CONTROLADOR Monitor
        /// </summary>
        /// <returns>si la accion fue realizada</returns>
        public List<CapaEntidad.Estacion> ListaEstaciones(string id = null)
        {
            //crear la lista
            List<CapaEntidad.Estacion> lista = new List<Estacion>();
            DataTable dtLista = new DataTable();

            //crear la consulta
            string strSQL = (id != null) ? "EXEC spr_EstacionesLista " + id : "EXEC spr_EstacionesLista";

            try
            {
                //acceder a la capa de datos y almacenar los registro en los datatables
                dtLista = Access.ObtenerDatos(strSQL).Tables[0];
                if (dtLista.Rows.Count > 0)
                {
                    //agregar los elementos a la lista
                    for (int i = 0; i < dtLista.Rows.Count; i++)
                    {
                        lista.Add(new Estacion
                        {
                            IdEstacion = Convert.ToInt32(dtLista.Rows[i]["IdEstacion"]),
                            Estado = dtLista.Rows[i]["Estado"].ToString(),
                            NombreEstacion = dtLista.Rows[i]["NombreEstacion"].ToString(),
                            IpLocal = dtLista.Rows[i]["IP_Local"].ToString(),
                            Area = NZona.DetalleZona(dtLista.Rows[i]["IdZona"].ToString())

                        });
                    }
                }
            }
            catch (Exception){
                return null;
            }

            //retornar lista
            return lista;
        }




    }
}
