using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

namespace SIGT_FULL.Hubs
{
    /// <summary>
    /// Clase hub que permite crear alerta en tiempo real
    /// 1- recibir el mensaje
    /// 2- guardar el mensaje en bd
    /// 3- agregar a la lista los mensajes enviados
    /// 4- enviar la lista
    /// </summary>
    public class AlertasHub : Hub
    {

        //comunicacion a otras capas
        private CapaNegocio.Alertas NNotificaciones = new CapaNegocio.Alertas();

        //notificaciones
        public static CapaEntidad.Alertas Notificaciones = new CapaEntidad.Alertas();
        public static List<CapaEntidad.Alertas> ListaNotificaciones = new List<CapaEntidad.Alertas>();
        

        /// <summary>
        /// notificar desde la estación
        /// al responsable sobre la sugerencia de apagado
        /// de la estación
        /// </summary>
        /// <param name="Estacion">Objeto que contiene la información de la estacion conectadas</param>
        public void Notificar(CapaEntidad.EstacionTrabajo Estacion)
        {
            //guardar notifaciones, comunicarse a la capa de negocio de alertas
            bool flag = NNotificaciones.RegistrarNotificaciones(Estacion.CodEstacion, Estacion.Cuenta, Convert.ToInt32(Estacion.CodTicket), Estacion.Ticket);

            //si se inserta correctamente, agregar a la lista
            if (flag == true) { 
                //obtener la lista de notificaciones para actualizar
                ListaNotificaciones = new List<CapaEntidad.Alertas>();
                ListaNotificaciones = NNotificaciones.ListaNotificaciones();
            }

            //agregar a la lista la alerta
            Clients.All.ListaMensajes(ListaNotificaciones);
        }

        /// <summary>
        /// Enviar la lista de mensajes existentes
        /// en la bd para para visualización al responsable de area
        /// </summary>
        public void Cargar()
        {
            ListaNotificaciones = new List<CapaEntidad.Alertas>();
            ListaNotificaciones = NNotificaciones.ListaNotificaciones();

            //agregar a la lista la alerta
            Clients.All.ListaMensajes(ListaNotificaciones);
        }
    }
}