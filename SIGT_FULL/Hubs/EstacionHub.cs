using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.Linq;


using System.Collections.Generic;
using System;

using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;


using CapaNegocio;


namespace SIGT_FULL.Hubs
{


    /// <summary>
    /// Permite detectar la conexion de un usuario a una estacion
    /// </summary>
    public class EstacionHub : Hub
    {

        //Referencia a la capa de negocio de usuarios y ticket
        CapaNegocio.Usuario NUsuario = new CapaNegocio.Usuario();
        CapaNegocio.Estaciones NEstacion = new CapaNegocio.Estaciones();
        CapaNegocio.Turno NTicket = new CapaNegocio.Turno();


        //lista de usuarios conectados a la estacion
        public static List<CapaEntidad.EstacionTrabajo> UsuarioActivos = new List<CapaEntidad.EstacionTrabajo>();


        //lista de estaciones activas
        public static List<CapaEntidad.EstacionTrabajo> EstacionesActivas = new List<CapaEntidad.EstacionTrabajo>();
        //lista de estaciones inactivas
        public static List<CapaEntidad.EstacionTrabajo> EstacionesInactivas = new List<CapaEntidad.EstacionTrabajo>();
        //Obtener la informacion de la estacion de trabajo actual
        public static CapaEntidad.EstacionTrabajo Estacion = new CapaEntidad.EstacionTrabajo();

        //verificar si existe turnos
        public static bool flagExiste_Turno = false;


        /// <summary>
        /// metodo que permite detectar el usuario activo y activar la estacion
        /// asignando ticket una vez activadas
        /// </summary>
        /// <param name="_user">usuario activo</param>
        /// <param name="_ip">ip de la estacion donde se encuentra</param>
        /// <param name="atender">si la estacion esta atendiendo</param>
        public void Conectar(string _user, string _ip, bool atender = false)
        {

            //para manejar el control de lista
            int flag = 0;

            var state = EstacionesActivas.Where(x => x.Cuenta == _user).FirstOrDefault();

            //detectar si la estación en donde se encuentra el usuario, se encuentra en la interfaz
            //de atención al público
            if (atender != false) {
                //actualizar 
                var x = NEstacion.ControlEstacion(_user, _ip, -1);
                    if (x != null) { 

                    //actualizar listas
                    for (int i = 0; i < UsuarioActivos.Count; i++){
                        if (UsuarioActivos[i].Cuenta == _user){
                            UsuarioActivos.Remove(UsuarioActivos[i]);
                        }
                    }

                    //verificar si ya existe en la lista de usuarios activos, si no existe agregarlo
                    for (int i = 0; i < UsuarioActivos.Count; i++){
                        if (UsuarioActivos[i].Cuenta == _user)
                            flag++;
                    }

                    //si activará la estacion, agregar a la lista de "Activos"
                    if (flag < 1){
                        //asignar un Id de conexion para controlar la conexion y desconexion
                        x.ConnectionID = Context.ConnectionId;
                        //agregar a lista
                        UsuarioActivos.Add(x);
                    }
                }
            }

            //metodos que servira interactuar con el cliente js
            Clients.All.conectados(UsuarioActivos);
        }



        /// <summary>
        /// Metodo que permite activar la estación
        /// el evento es controlado por un responsble de
        /// area desde la interfaz "ListaEstaciones" del
        /// controlador Atencion
        /// </summary>
        /// <param name="_user">usuario a activar</param>
        /// <param name="_ip">ip de la estacion</param>
        public void Activar(string _user, string _ip)
        {
            //Activar la estacion segun el usuario, usuario, ip de la estacion y accion conectar(1)
            Estacion = NEstacion.ControlEstacion(_user, _ip, 1);

            //si se ha realizado la activacion de la estacion enviar el objeto estacion
            if (Estacion != null){
                //Actualizar la lista de estaciones inactivas, quitandolo de dicha lista
                EstacionesInactivas.Remove(EstacionesInactivas.Where(x => x.Cuenta == _user).FirstOrDefault());

                //Actualizar la informacion de la estacion con el nuevo ticket
                //quitar los datos antiguos
                EstacionesActivas.Remove(EstacionesActivas.Where(x => x.Cuenta == _user).FirstOrDefault());

                //agregar nuevamente con la información actualizada
                EstacionesActivas.Add(Estacion);

            }

            //metodos que servira interactuar con el cliente
            Clients.All.FlagTickets(flagExiste_Turno);
            Clients.All.conectadosEstacion(EstacionesActivas);
            Clients.All.desconectadosEstacion(EstacionesInactivas);
            
        }


        /// <summary>
        /// metodo que permite desactivar la estación
        /// quintando el ticket asignado a la estacion involucrada y dejarlo en cola
        /// </summary>
        /// <param name="_user">usuario presente en la estacion para desconectar</param>
        /// <param name="_ip">ip de la estación</param>
        public void Desactivar(string _user, string _ip)
        {
            //Activar la estacion segun el usuario
            Estacion = NEstacion.ControlEstacion(_user, _ip, 0);
            //si se desactivo correctamente
            if (Estacion != null){
                //si desactivará la sesion, eliminar el elemento de la lista segun el usuario a desactivar
                EstacionesActivas.Remove(EstacionesActivas.Where(x => x.Cuenta == _user).FirstOrDefault());

                //si se desactiva la estacion, agregar a la lista de "inacActivos"
                //Actualizar la informacion de la estacion con el nuevo ticket
                //quitar los datos antiguos
                EstacionesInactivas.Remove(EstacionesInactivas.Where(x => x.Cuenta == _user).FirstOrDefault());
                //agregar nuevamente con la información actualizada
                EstacionesInactivas.Add(Estacion);


                
            }

            //metodos que servira interactuar con el cliente
            Clients.All.conectadosEstacion(EstacionesActivas);
            Clients.All.desconectadosEstacion(EstacionesInactivas);
        }


        /// <summary>
        /// Validar cierre de estacion
        /// si la estacion se encuentra atendiendo
        /// no desactivar la estacion
        /// </summary>
        /// <param name="_user"></param>
        /// <param name="_ip"></param>
        public bool ValidarCierre(string _user, string _ip)
        {
            bool Message = false;
            //Obtener la información de la estacion a desactivar
            CapaEntidad.EstacionTrabajo estacion = new CapaEntidad.EstacionTrabajo();
            estacion = NEstacion.DetalleEstacionesActiva(_user);

            //validar que no este atendiendo
            if (estacion != null && estacion.Estado != 3)
                Message = true;

            return Message;

        }

        /// <summary>
        /// Permite detectar si existe turnos disponibles
        /// retornara true o false según el caso
        /// </summary>
        /// <returns></returns>
        public bool ValidarActivacion(string _user, string _ip)
        {

            //Obtener el area del usuario
            var usuario = NUsuario.DetalleUsuario(_user);

            //verificar si existe turnos en espera
            //usar linq para filtra los estado 1 { Where(x => x.Estado == 1).ToList() }, luego usar count para contar y
            //segun la condicion asignar true o false
            try{
                flagExiste_Turno = (NTicket.ListaTicket_Hoy().Where(x => x.Estado == 1 && x.Motivo.Zona.IdZona == usuario.Area.IdZona).ToList().Count() > 0) ? true : false;
            }
            catch (Exception ex){
                flagExiste_Turno = false;
            }

            

            //si no existe turnos, actualizar las listas
            if (flagExiste_Turno == false){
                //si desactivará la sesion, eliminar el elemento de la lista segun el usuario a desactivar
                EstacionesActivas.Remove(EstacionesActivas.Where(x => x.Cuenta == _user).FirstOrDefault());

                //si se desactiva la estacion, agregar a la lista de "inacActivos"
                //Actualizar la informacion de la estacion
                //quitar los datos antiguos
                EstacionesInactivas.Remove(EstacionesInactivas.Where(x => x.Cuenta == _user).FirstOrDefault());
                //agregar nuevamente con la información actualizada
                EstacionesInactivas.Add(Estacion);

                //metodos que servira interactuar con el cliente
                Clients.All.conectadosEstacion(EstacionesActivas);
                Clients.All.desconectadosEstacion(EstacionesInactivas);
            }

           

            return flagExiste_Turno;
        }


        /// <summary>
        /// Iniciar la atención al cliente
        /// recibirá de parametros el usuario, la ip de la estacion, 
        /// _proceso (si la inicia(3) o finaliza(4)) y las observaciones cuando finalice la atencion
        /// </summary>
        /// <param name="_user">usuario que atendera en la estacion</param>
        /// <param name="_ip">estacion donde atenderá</param>
        /// <param name="_proceso">lo que hara</param>
        /// <param name="_tiempo">tiempo ejecutado</param>
        /// <param name="_observaciones">observaciones detectadas</param>
        public void Atencion(string _user, string _ip, int _proceso, string _tiempo = "", string _observaciones = "")
        {
            //el estado -1 solo se aplica en este metodo "ATENCION"
            //-1 servirá para guardar las observaciones
            if (_proceso == 3 || _proceso == 4){
                //Actualizar estado de la estacion la estacion segun el usuario
                //3 inicia la atención: actualiza el estado de la estacion/ticket
                //4 si finaliza la atención: actualizar el estado de la estación/ticket
                Estacion = NEstacion.ControlEstacion(_user, _ip, _proceso);
            }

            //si inicializa la atención: actualizar el estado de la estacion y ticket
            else{
                bool flag = NEstacion.ActualizarAtencion(Estacion.CodTicket, Estacion.Ticket, _tiempo, _observaciones);
            }

            //Actualizar el registros de la estacions
            if (Estacion != null){
                //quitar de la lista al ticket asignado a la estacion para actualizar
                EstacionesActivas.Remove(EstacionesActivas.Where(x => x.Cuenta == _user).FirstOrDefault());
                //agregar nuevamente con la información actualizada
                EstacionesActivas.Add(Estacion);

                //metodos que servira interactuar con el cliente
                //Clients.All.conectadosEstacion(EstacionesActivas);
                //Clients.All.desconectadosEstacion(EstacionesInactivas);

            }
            
        }


        /// <summary>
        /// metodo que permite cancelar el turno debido a la ausencia del cliente
        /// el ticket quedará anulado
        /// </summary>
        /// <param name="idTicket">IdTicket a desactivar</param>
        /// <param name="ticket">ticket generado</param>
        /// <param name="_user">usuario que cancerlará</param>
        /// <param name="_ip">estacion donde se encuentra atendiendo</param>
        public void Cancelar(string idTicket, string ticket, string _user, string _ip)
        {
            //cancela la ticket, enviar 0 para cancelar turno            
            bool eTicket = NTicket.ModificarTicket(Convert.ToInt32(idTicket), ticket, 0, _user);
        }

        /// <summary>
        /// Cuando no haya ticket que atender
        /// </summary>
        /// <param name="_user"></param>
        /// <param name="_ip"></param>
        public void FinTrabajo(string _user, string _ip)
        {
            //Activar la estacion segun el usuario
            Estacion = NEstacion.ControlEstacion(_user, _ip, -1);
            //si se desactivo correctamente
            if (Estacion != null)
            {
                //si se desactiva la estacion, agregar a la lista de "inacActivos"
                EstacionesInactivas.Add(Estacion);

                //si desactivará la sesion, eliminar el elemento de la lista segun el usuario a desactivar
                for (int i = 0; i < EstacionesActivas.Count; i++)
                {
                    if (EstacionesActivas[i].Cuenta == _user)
                    {
                        EstacionesActivas.Remove(EstacionesActivas[i]);
                    }
                }
            }

            //metodos que servira interactuar con el cliente
            Clients.All.conectadosEstacion(EstacionesActivas);
            Clients.All.desconectadosEstacion(EstacionesInactivas);          
            
        }


        /// <summary>
        /// metodo que permite reasignar otro ticket
        /// </summary>
        /// <param name="_user">usuario a quien le asignaraá la ticket</param>
        /// <param name="_ip">ip de la estacion donde asignará la ticket</param>
        public void Reasignar(string _user, string _ip)
        {

            //reasignar otro tocket
            Estacion = NEstacion.ControlEstacion(_user, _ip, 1);
            ////quitar de la lista al ticket asignado a la estacion para actualizar
            ////el estado del ticket en estaciones activass
            //for (int i = 0; i < EstacionesActivas.Count; i++){
            //    if (EstacionesActivas[i].Cuenta == _user){
            //        EstacionesActivas.Remove(EstacionesActivas[i]);
            //    }
            //}

            if (Estacion != null) {
                //una vez actualizado la informacion de la estacion, actualizar la lista
                EstacionesActivas.Remove(EstacionesActivas.Where(x => x.Cuenta == _user).FirstOrDefault());
                //agregar nuevamente con la información actualizada
                EstacionesActivas.Add(Estacion);
            }

            //propiedad que servira interactuar con el cliente
            Clients.All.conectadosEstacion(EstacionesActivas);
            Clients.All.desconectadosEstacion(EstacionesInactivas);
        }



        /// <summary>
        /// obtiene la lista de usuarios conectados
        /// </summary>
        public List<CapaEntidad.EstacionTrabajo> ObtenerListado()
        {
            //metodos que servira interactuar con el cliente
            return EstacionesActivas;
        }

        /// <summary>
        /// permite detectar la desconexion de usuarios en las estaciones
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            //Obtener la informacion de un usuario de la lista
            var item = UsuarioActivos.FirstOrDefault(x => x.ConnectionID == Context.ConnectionId);

            //detectar usuario
            if (item != null){
                //removerlo de la lista
                UsuarioActivos.Remove(item);
                //crear IdConexion
                var id = Context.ConnectionId;


                //quitarlo de la lista
                //bool saq = NEstacion.LimpiarEstacion_Trabajo(item.IpEstacion);
            }

            //metodos que servira interactuar con el cliente
            Clients.All.conectados(UsuarioActivos);
            return base.OnDisconnected(stopCalled);
        }

       
    }
}