USE [SIGT_FULL]
GO
/****** Object:  StoredProcedure [dbo].[spr_TicketLista]    Script Date: 08/04/2018 16:31:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Detalla la informacion del ticket generado
-- =============================================
ALTER PROCEDURE [dbo].[spr_TicketLista_HOY]
AS
BEGIN
		SELECT solicitud.IdSolicitud, solicitud.IdMotivo, solicitud.Ticket, solicitud.IdCliente, solicitud.IdMotivo, solicitud.IdZona, solicitud.Siglas,
			    solicitud.Observaciones, solicitud.FechaSolicitud, solicitud.EstadoSolicitud, motivo.IdPrioridad,
				motivo.Descripcion, estado.EstadoDesc,
				prioridad.Prioridad AS 'PrioridadDesc', atendidos.Usuario
		 FROM tblTurno_Solicitud solicitud
		 INNER JOIN tblTurno_SolicitudEstado estado ON solicitud.EstadoSolicitud = estado.IdEstado
		 INNER JOIN tblTurno_Motivo motivo ON solicitud.IdMotivo = motivo.IdMotivo AND solicitud.IdZona = motivo.IdZona AND solicitud.Siglas = motivo.Siglas
		 INNER JOIN tblTurno_Prioridad prioridad ON motivo.IdPrioridad = prioridad.IdPrioridad
		 LEFT JOIN tblTurno_Atencion atendidos ON solicitud.IdSolicitud = atendidos.IdSolicitud
		 WHERE solicitud.FechaSolicitud BETWEEN GETDATE() AND GETDATE()
		 ORDER BY solicitud.FechaSolicitud DESC;
END

 -- EXEC spr_TicketLista_HOY
