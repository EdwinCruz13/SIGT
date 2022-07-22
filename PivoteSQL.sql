SELECT Anno,
	   -- Mostrar los pivotes
	   [1] AS Enero,[2] AS Febrero,[3] AS Marzo,[4] AS Abril,[5] AS Mayo, [6] AS Junio,
	   [7] AS Julio, [8] AS Agosto,[9] AS Septiembre,[10] AS Octubre,[11] AS Noviembre,[12] AS Diciembre,
	   -- Mostrar total por año
	   (ISNULL([1],0)+ISNULL([2],0)+ISNULL([3],0)+ISNULL([4],0)+ISNULL([5],0)+ISNULL([6],0)+ISNULL([7],0)+ISNULL([8],0)+ISNULL([9],0)+ISNULL([10],0)+ISNULL([11],0)+ISNULL([12],0)) Totales
FROM  (SELECT solicitud.IdSolicitud, YEAR(FechASolicitud) AS Anno, MONTH(FechASolicitud) AS Mes
	   FROM tblTurno_Solicitud solicitud
	   LEFT JOIN tblTurno_Atencion atencion ON solicitud.IdSolicitud = atencion.IdSolicitud
	   -- WHERE YEAR(FechASolicitud) = '2018'
	   GROUP BY solicitud.IdSolicitud, YEAR(solicitud.FechASolicitud), MONTH(solicitud.FechASolicitud)) AS RecursoTabla
pivot( Count(IdSolicitud) For Mes in([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])) AS Total