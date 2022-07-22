USE [SIGT_FULL]
GO
/****** Object:  Trigger [dbo].[tr_AsignarTurno]    Script Date: 08/04/2018 18:19:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
ALTER TRIGGER [dbo].[tr_AsignarTurno] ON [dbo].[tblTurno_Asignacion] AFTER UPDATE
AS 
BEGIN
	-------------------------------------------------------------------------------
	-- Trigger que permite gestionar el estado de las estaciones y ticket
	-- cuando la estacion se activa, selecciona un ticket que se encuentra en espera
	-- para la asignacion, luego cambia en estado 2(ticket asignado a la estacion)
	-- si el cliente se presenta, pasar en estado 3(ticket en atencion)
	-- si no se presenta, anular el ticket
	--------------------------------------------------------------------------------


	-------------------------------------------------------------------------------
	-- declarar variables que representa los campos de la columna [tblTurno_Asignacion]
	--------------------------------------------------------------------------------

	-- campos actuales despues de la actualizacion
	DECLARE @nAsignacion_a	int;
	DECLARE @Usuario_a nvarchar(15);
	DECLARE @CodTicket_a int;
	DECLARE @Ticket_a nvarchar(5);
	DECLARE @IdCliente_a nvarchar(21);
	DECLARE @Estado_a int;
	DECLARE @FechaAsignación_a datetime;
	DECLARE @nAtendidos_a int;
	DECLARE @IpEstacion_a nvarchar(16);
	DECLARE @CodEstacion_a int;

	-- variables adiccionales para la fecha y hora de la solicitud
	DECLARE @dia int;			     DECLARE @mes int;				 DECLARE @anno int;					DECLARE @FechaActual datetime;
	-- setear dichas variables
	SET @FechaActual = GETDATE();   SET @dia = DAY(@FechaActual);    SET @mes = MONTH(@FechaActual);   SET @anno = YEAR(@FechaActual);
	

	-- variables para guardar los valors de las tickets del dia
	DECLARE @MovTicket int;		 DECLARE @Ticket nvarchar(5);   DECLARE @IdCliente nvarchar(21);
	DECLARE @EstadoTicket int;	 DECLARE @PrioridadTicket int;  DECLARE @FechaSolicitud datetime;

	-- obtener el area de la estacion activa
	DECLARE @IdZona nvarchar(2);
	

	-- controlar el cronometro de atención
	DECLARE @FechaInicia datetime;
	DECLARE @FechaFinaliza datetime;

	-- manejos de errores
	DECLARE @ErrorMessage nvarchar(4000); Declare @ErrorSeverity int; Declare @ErrorState int; 


	-- verificar si existe actualizacion en el estado de la estacion
    IF (UPDATE(Estado))
	BEGIN
		-------------------------------------------------------------------------------
		-- obtener el estado actual
		-------------------------------------------------------------------------------

		SELECT @nAsignacion_a = nAsignacion, @Usuario_a = Usuario , @CodTicket_a = CodTicket, @Ticket_a = Ticket , 
		       @IdCliente_a = IdCliente, @Estado_a = Estado,
		       @FechaAsignación_a = FechaAsignación, @nAtendidos_a = nAtendidos, @IpEstacion_a = IpEstacion, 
			   @CodEstacion_a = IdEstacion
		FROM INSERTED
		
		-- manejos de errores
		BEGIN TRY	
			-------------------------------------------------------------------------------
			-- verificar el estado actual y la acciones que se realizarán
			-------------------------------------------------------------------------------

			-- si la estacion acaba de ser activada, empezar a asignar el turno
			-- en orden de entrada del cliente, el estado del turno pasará a estado 2
			IF (@Estado_a = 1)
			BEGIN
				-- obtener la zona de la estacion que esta siendo afectado
				SELECT @IdZona = IdZona 
				FROM tblEstacion 
				WHERE  IP_Local = @IpEstacion_a AND IdEstacion = @CodEstacion_a

				-- Una vez detectada la zona de la estacion que acaba de activarse
				-- verificar si las zonas corresponden a las analista legales (03)
				-- asignar el turno a la analista correspondiente según los registros en Inversiones.Net
				IF @IdZona = '03' 
				BEGIN
					DECLARE @Variable_inservible int;
				END

				-- LO BUENO Y PROBADO DESDE ACA --
				-- si no es abogado, asignar un turno automatico
				ELSE
				BEGIN
					-- Listar los turnos del dia ordenados segun las primeras solicitudes, la zona de la estacion y que esten en espera(1)
					-- guardarlos en las variables
					SELECT TOP(1) @MovTicket = IdSolicitud, @Ticket = Ticket, @IdCliente = IdCliente
					FROM tblTurno_Solicitud
					WHERE DATEPART(DAY, FechaSolicitud) = @dia AND DATEPART(MONTH, FechaSolicitud) = @mes AND DATEPART(YEAR, FechaSolicitud) = @anno
					AND EstadoSolicitud = 1 AND IdZona = @IdZona
					ORDER BY FechaSolicitud ASC

					-- obtenido los datos, actualizar la tabla tblTurno_Asignacion
					UPDATE tblTurno_Asignacion SET CodTicket = @MovTicket, Ticket = @Ticket, IdCliente = @IdCliente, Estado = 2
					WHERE IpEstacion = @IpEstacion_a AND IdEstacion = @CodEstacion_a

					-- actualizar el estado del ticket
					UPDATE tblTurno_Solicitud SET EstadoSolicitud = 2
					WHERE IdSolicitud = @MovTicket;
				END	

			END

			-- el analista inicializa la atencion al cliente, guardar los datos en tblTurno_Atencion
			-- actualizar en la tabla tblTurno_Solicitud y tblTurno_Asignacion el estado de la ticket
			IF (@Estado_a = 3)
			BEGIN
				SET @FechaInicia = GETDATE();

				-- verificar si existe un IdSolicitud de ticket en tblTurno_Atencion
				-- si existe solo actualizar los nuevos datos, sino insertarlos
				DECLARE @IdTicketT int;
				SELECT @IdTicketT = COUNT(IdSolicitud) FROM tblTurno_Atencion WHERE IdSolicitud = @CodTicket_a;

				--VALIDAR el IdSolicitud para evitar duplicacion de Tickets en tblTurno_Atencion
				-- Si no existe el ticket en tblTurno_Atencion entonces insertar
				IF @IdTicketT = 0
				BEGIN
					-- insertar en tblTurno_Atencion
					INSERT INTO tblTurno_Atencion (IdSolicitud, Ticket, Usuario, IdCliente, FechaAtencion, TiempoInicia)
					VALUES (@CodTicket_a, @Ticket_a, @Usuario_a, @IdCliente_a, @FechaInicia, CONVERT(VARCHAR(10), @FechaInicia,114));
				END

				-- En el caso de exista solo actualizar
				ELSE
				BEGIN
					-- insertar en tblTurno_Atencion
					UPDATE tblTurno_Atencion SET FechaAtencion = @FechaInicia, TiempoInicia = CONVERT(VARCHAR(10), @FechaInicia,114)
				END

				-- obtenido los datos, actualizar la tabla tblTurno_Asignacion
				UPDATE tblTurno_Asignacion SET Estado = 3
				WHERE IpEstacion = @IpEstacion_a AND IdEstacion = @CodEstacion_a

				-- actualizar el estado del ticket
				UPDATE tblTurno_Solicitud SET EstadoSolicitud = 3
				WHERE IdSolicitud = @CodTicket_a;
			END

			-- el analista finaliza la atencion al cliente, actualizar los datos en tblTurno_Atencion
			-- actualizar en la tabla tblTurno_Solicitud y tblTurno_Asignacion el estado de la ticket
			IF (@Estado_a = 4)
			BEGIN
				SET @FechaFinaliza = GETDATE();

				-- actualizar en tblTurno_Atencion
				UPDATE tblTurno_Atencion SET TiempoFinaliza = CONVERT(VARCHAR(10), @FechaFinaliza, 114)
				WHERE IdSolicitud = @CodTicket_a

				-- actualizar el estado del ticket a 4 (ticket atendido)
				UPDATE tblTurno_Solicitud SET EstadoSolicitud = 4
				WHERE IdSolicitud = @CodTicket_a;

			END

			-- si la estacion se desactiva, es posible que hayan turnos asignado a la estacion
			-- por lo que es necesario pasar el estado del turno a "en espera" para que pueda ser asignado a otra estacion
			IF (@Estado_a = 0)
			BEGIN
					-- Detectar el estado actual de la estacion
					SELECT @EstadoTicket = EstadoSolicitud FROM tblTurno_Solicitud WHERE IdSolicitud = @CodTicket_a
				
					--si desactivará la estacion terminando una atencion, solo actualizar turno_asignacion
					IF @EstadoTicket = 4
					BEGIN
						-- obtenido los datos, actualizar la tabla tblUsuario_AsignacionTurno
						UPDATE tblTurno_Asignacion SET CodTicket = NULL, Ticket = NULL, IdCliente = NULL
						WHERE nAsignacion = @nAsignacion_a AND IpEstacion = @IpEstacion_a AND IdEstacion = @CodEstacion_a
					END

					IF @EstadoTicket = 0
					BEGIN
						-- obtenido los datos, actualizar la tabla tblUsuario_AsignacionTurno
						UPDATE tblTurno_Asignacion SET CodTicket = NULL, Ticket = NULL, IdCliente = NULL
						WHERE nAsignacion = @nAsignacion_a AND IpEstacion = @IpEstacion_a AND IdEstacion = @CodEstacion_a
					END

					-- si desactiva mientras no haya atendido un cliente, pero la estación posee ese ticket a la estación
					IF @EstadoTicket <> 4 AND @EstadoTicket <> 0
					BEGIN
						-- obtenido los datos, actualizar la tabla tblUsuario_AsignacionTurno
						UPDATE tblTurno_Asignacion SET CodTicket = NULL, Ticket = NULL, IdCliente = NULL
						WHERE nAsignacion = @nAsignacion_a AND IpEstacion = @IpEstacion_a AND IdEstacion = @CodEstacion_a

						-- actualizar el estado del ticket
						UPDATE tblTurno_Solicitud SET EstadoSolicitud = 1
						WHERE IdSolicitud = @CodTicket_a;
					END

				--
				-- DELETE FROM tblTurno_Atencion WHERE IdSolicitud = @CodTicket_a

			END

			-- cuando no haya turnos que atender
			IF (@Estado_a = -1)
			BEGIN
				-- obtenido los datos, actualizar la tabla tblUsuario_AsignacionTurno
				UPDATE tblTurno_Asignacion SET /*CodTicket = NULL, Ticket = NULL, IdCliente = NULL,*/ Estado = -1
				WHERE nAsignacion = @nAsignacion_a AND IpEstacion = @IpEstacion_a AND IdEstacion = @CodEstacion_a
			END

		END TRY

		BEGIN CATCH
			--Capturamos el error correspondiente en la transaccion..
			   Select @ErrorMessage = ERROR_MESSAGE(),@ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();	

			--Imprimimos el error. 	   		       
			 Raiserror (@ErrorMessage,@ErrorSeverity,@ErrorState); 
		END CATCH
			
	END
END

-- UPDATE tblTurno_Asignacion SET Estado = 1 WHERE Usuario = 'DOBREGON';

/*
	SELECT TOP(1) IdSolicitud, Ticket, IdCliente, Prioridad
				FROM tblTurno_Solicitud
				WHERE DATEPART(DAY, FechaSolicitud) = @dia AND DATEPART(MONTH, FechaSolicitud) = @mes AND DATEPART(YEAR, FechaSolicitud) = @anno
				AND EstadoSolicitud = 1 AND IdZona = '01'
				ORDER BY FechaSolicitud ASC

*/