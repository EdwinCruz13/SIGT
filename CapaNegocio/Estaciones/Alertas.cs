using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    /// <summary>
    /// Clase de negocio que sera usada para el control
    /// de notificaciones
    /// </summary>
    public class Alertas
    {
        //Propieadad para el acceso a datos
        CapaDatos.SQLContext Access = new CapaDatos.SQLContext();
        //Propiedad para acceder a la capa de negocio
        CapaNegocio.Estaciones NEstaciones = new CapaNegocio.Estaciones();

        /// <summary>
        /// obtiene la listas de notificaciones existentes del día
        /// ejecutrapa el procedimiento almacenado
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns>List<CapaEntidad.Alertas</returns>
        public List<CapaEntidad.Alertas> ListaNotificaciones(string usuario = null)
        {
            List<CapaEntidad.Alertas> notificaciones = new List<CapaEntidad.Alertas>();
            DataTable dtNotificaciones = new DataTable();
            string strConsulta = (usuario == null || usuario == "") ? "EXEC spr_AlertaLista" : "EXEC spr_AlertaLista '" + usuario + "'";
            try{
                dtNotificaciones = Access.ObtenerDatos(strConsulta).Tables[0];
                if (dtNotificaciones.Rows.Count > 0){
                    for (int i = 0; i < dtNotificaciones.Rows.Count; i++) {
                        notificaciones.Add(new CapaEntidad.Alertas {
                            ID = Convert.ToInt32(dtNotificaciones.Rows[i]["ID"]),
                            Fecha = Convert.ToDateTime(dtNotificaciones.Rows[i]["FechaAlerta"]),
                            Mensage = dtNotificaciones.Rows[i]["Mensaje"].ToString(),
                            TipoAlerta = dtNotificaciones.Rows[i]["TipoAlerta"].ToString(),
                            Consecutivo = Convert.ToInt32(dtNotificaciones.Rows[i]["Consecutivo"]),
                            EstacionTrabajo = NEstaciones.DetalleEstacionesActiva(dtNotificaciones.Rows[i]["Usuario"].ToString()) 
                        });
                    }
                }
            }
            catch (Exception){
                return null;
            }

            return notificaciones;
        }

        /// <summary>
        /// Registra las notificaciones de las estacioones
        /// solicitando la desactivación de la cuenta
        /// </summary>
        /// <param name="IdEstacion">estación quien la solicita</param>
        /// <param name="usuario">usuario quien lo solicita</param>
        /// <param name="IdTicket">ticket guardado antes de solicitar</param>
        /// <param name="Ticket">nombre del ticket guardado antes de solicitar</param>
        /// <returns>bool</returns>
        public bool RegistrarNotificaciones(int IdEstacion, string usuario, int IdTicket = 0, string Ticket = null)
        {
            //crear lista de objeto
            IList<object> Resultado;

            //crear variable para ser usada como variable de salida
            int Salida = 0;
            try{
                //ejecutar procedimiento almacenado para insertar la alerta
                Resultado = (IList<object>)Access.EjecutarProcedimiento("spr_AlertaRegistrar", Salida, IdEstacion, usuario, IdTicket, Ticket.Replace("-", ""));
                //almacenar la variable de salida despues de la ejecucion
                Salida = Convert.ToInt32(Resultado[0]);

                return Convert.ToBoolean(Salida);
            }
            catch (Exception){
               return false;
            }

        }
    }
}
