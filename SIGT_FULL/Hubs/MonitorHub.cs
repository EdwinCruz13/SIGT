using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.Linq;

using CapaNegocio;
using System.Collections.Generic;
using System.Collections;
using CapaEntidad.Turno;

namespace SIGT_FULL.Hubs
{
    //el hub de esta clase, hara referencia al llamado de la estacion al cliente
    //donde se visualizará el resultado en el monitor publicitario
    //instalados en las zonas involucradas
    public class MonitorHub : Hub
    {

        public static List<CapaEntidad.EstacionTrabajo> TicketsRec = new List<CapaEntidad.EstacionTrabajo>();

        //cuando haya un llamado de la estacion, visualizar el resultado en el monitor
        //recibe como parametro el objeto estacion que posee las 
        //las propiedades del cliente, el ticket y el codigo involucrado
        public void Llamar(CapaEntidad.EstacionTrabajo estacion)
        {
           
            Clients.All.turnoTicket(estacion);

        }

    }
}