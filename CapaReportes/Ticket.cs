using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

using CapaEntidad;

namespace CapaReportes
{
    public partial class Ticket : DevExpress.XtraReports.UI.XtraReport
    {
        public Ticket(CapaEntidad.Ticket solicitud)
        {
            InitializeComponent();

            //cambiar el texto de los xrLabel
            this.xrlabelTurno.Text = solicitud.CodTicket;
            this.xrlabelZonaS.Text = solicitud.Motivo.Zona.Descripcion;
            //this.xrlabelCliente.Text = solicitud.Cliente.NombreCompleto;
            this.xrlabelFechaS.Text = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(solicitud.FechaSolicitud));
            //this.xrlabelHoraS.Text = String.Format("{0:HH:mm:ss}", Convert.ToDateTime(solicitud.FechaSolicitud));

            

        }

    }
}
