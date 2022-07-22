USE [Sadex]
GO
/****** Object:  StoredProcedure [dbo].[sprTramitesTotalPorDias]    Script Date: 28/03/2018 01:21:55 p.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER Procedure [dbo].[sprTramitesTotalPorDias]
@IdSistema As nchar(2),@Mes nvarchar(2),@Anno nvarchar(4)
As
Begin
--------------------------------------------------------------------------------------------------------------------------------------------------------
--Pivot Trámite Mensual por Usuario
--------------------------------------------------------------------------------------------------------------------------------------------------------
Declare @Mess As nvarchar(12); Set @Mess = DateName(MONTH,'01'+'/'+@Mes+'/'+@Anno);
With PivotTramiteMes As
(
Select UsuarioRegistra,[01],[02],[03],[04],[05],[06],[07],[08],[09],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19],[20],[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31],       
    ([01]+[02]+[03]+[04]+[05]+[06]+[07]+[08]+[09]+[10]+[11]+[12]+[13]+[14]+[15]+[16]+[17]+[18]+[19]+[20]+[21]+[22]+[23]+[24]+[25]+[26]+[27]+[28]+[29]+[30]+[31])Totales,
	 @Mess As 'Mes',@Anno As 'Año'      
From(Select NumeroTramite,UsuarioRegistra,DAY(FechaAgregado) As FechaAgregado
     From DocumentosImagen Where Month(FechaAgregado)= @Mes and Year(FechaAgregado) = @Anno And Id_Sistema = @IdSistema
     Group by NumeroTramite,UsuarioRegistra,DAY(FechaAgregado)) AS SourceTable
	 
pivot(Count(NumeroTramite) For FechaAgregado in(
      [01],[02],[03],[04],[05],[06],[07],[08],[09],[10],[11],[12],[13],[14],[15],[16],[17],[18],[19],[20],[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31]))As Total
)
Select * From PivotTramiteMes
Union all
Select 'Totales',ISNULL(Sum([01]),0),ISNULL(Sum([02]),0),ISNULL(Sum([03]),0),ISNULL(Sum([04]),0),ISNULL(Sum([05]),0),ISNULL(Sum([06]),0),
                 ISNULL(Sum([07]),0),ISNULL(Sum([08]),0),ISNULL(Sum([09]),0),ISNULL(Sum([10]),0),ISNULL(Sum([11]),0),ISNULL(Sum([12]),0),
				 ISNULL(Sum([13]),0),ISNULL(Sum([14]),0),ISNULL(Sum([15]),0),ISNULL(Sum([16]),0),ISNULL(Sum([17]),0),ISNULL(Sum([18]),0),
				 ISNULL(Sum([19]),0),ISNULL(Sum([20]),0),ISNULL(Sum([21]),0),ISNULL(Sum([22]),0),ISNULL(Sum([23]),0),ISNULL(Sum([24]),0),
				 ISNULL(Sum([25]),0),ISNULL(Sum([26]),0),ISNULL(Sum([27]),0),ISNULL(Sum([28]),0),ISNULL(Sum([29]),0),ISNULL(Sum([30]),0),
				 ISNULL(Sum([31]),0),ISNULL(Sum(Totales),0),@Mess,@Anno
From PivotTramiteMes;
--Probamos el procedimiento almacenado...
--Exec sprTramitesTotalPorDias '1','07','2017'
End;
