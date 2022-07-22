SELECT solicitud.IdSolicitud, solicitud.IdMotivo, solicitud.Ticket, solicitud.IdCliente, solicitud.IdMotivo, solicitud.IdZona, solicitud.Siglas,
			    solicitud.Observaciones, solicitud.FechaSolicitud, solicitud.EstadoSolicitud, motivo.IdPrioridad,
				motivo.Descripcion, estado.EstadoDesc,
				prioridad.Prioridad AS 'PrioridadDesc', atendidos.Usuario
FROM tblTurno_Solicitud solicitud
INNER JOIN tblTurno_SolicitudEstado estado ON solicitud.EstadoSolicitud = estado.IdEstado
INNER JOIN tblTurno_Motivo motivo ON solicitud.IdMotivo = motivo.IdMotivo AND solicitud.IdZona = motivo.IdZona AND solicitud.Siglas = motivo.Siglas
INNER JOIN tblTurno_Prioridad prioridad ON motivo.IdPrioridad = prioridad.IdPrioridad
LEFT JOIN tblTurno_Atencion atendidos ON solicitud.IdSolicitud = atendidos.IdSolicitud
WHERE solicitud.EstadoSolicitud <> 0 AND solicitud.FechaSolicitud BETWEEN CONVERT(datetime, '2018-04-05 01:00:37.977',102) AND CONVERT(datetime, '2018-04-30 23:00:37.977',102)
--AND atendidos.Usuario = 'JDAVILA'
ORDER BY solicitud.FechaSolicitud DESC;


SELECT atendidos.Usuario, SUM(DATEDIFF(second, atendidos.TiempoInicia, atendidos.TiempoFinaliza) / 60)
FROM tblTurno_Solicitud solicitud
INNER JOIN tblTurno_SolicitudEstado estado ON solicitud.EstadoSolicitud = estado.IdEstado
INNER JOIN tblTurno_Motivo motivo ON solicitud.IdMotivo = motivo.IdMotivo AND solicitud.IdZona = motivo.IdZona AND solicitud.Siglas = motivo.Siglas
INNER JOIN tblTurno_Prioridad prioridad ON motivo.IdPrioridad = prioridad.IdPrioridad
LEFT JOIN tblTurno_Atencion atendidos ON solicitud.IdSolicitud = atendidos.IdSolicitud
WHERE solicitud.EstadoSolicitud <> 0 AND solicitud.FechaSolicitud BETWEEN CONVERT(datetime, '2018-04-05 01:00:37.977',102) AND CONVERT(datetime, '2018-04-30 23:00:37.977',102)
GROUP BY atendidos.Usuario;
















SELECT * FROM tblTurno_Atencion 
WHERE CONVERT(datetime, FechaAtencion, 102) > CONVERT(datetime, '2018-04-11 01:00:37.977',102) AND FechaAtencion < CONVERT(datetime, '2018-04-11 23:00:37.977',102)




SELECT s.IdSolicitud, (SELECT IdSolicitud FROM tblTurno_Atencion a WHERE s.IdSolicitud = a.IdSolicitud)
FROM tblTurno_Solicitud s
WHERE s.FechaSolicitud BETWEEN CONVERT(datetime, '2018-04-07 01:00:37.977',102) AND CONVERT(datetime, '2018-04-24 23:00:37.977',102)
AND s.EstadoSolicitud <> 0

