using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class Zona
    {

        //Crear la referencia a la capa de datos con un objeto
        CapaDatos.SQLContext Access = new CapaDatos.SQLContext();


        /// <summary>
        /// Metodo de ListarZona
        /// </summary>
        /// <returns> List<CapaEntidad.Zona> </returns>
        public List<CapaEntidad.Zona> ListarZona()
        {
            //crar la lista para la zonas existente
            List<CapaEntidad.Zona> zona = new List<CapaEntidad.Zona>();
            DataTable dtZona = new DataTable();
            try
            {
                dtZona = Access.ObtenerDatos("SELECT * FROM tblZona").Tables[0];
                if (dtZona.Rows.Count > 0)
                {
                    for (int i = 0; i < dtZona.Rows.Count; i++)
                    {
                        zona.Add(new CapaEntidad.Zona
                        {
                            IdZona = dtZona.Rows[i]["IdZona"].ToString(),
                            Siglas = dtZona.Rows[i]["Siglas"].ToString(),
                            Descripcion = dtZona.Rows[i]["Descripcion"].ToString(),
                            Responsable = dtZona.Rows[i]["Responsable"].ToString(),
                            FechaRegistro = Convert.ToDateTime(dtZona.Rows[i]["FechaRegistra"]),
                            Estado = Convert.ToInt32(dtZona.Rows[i]["Estado"])
                        });
                    }
                }
                return zona;

            }
            catch (Exception)
            {
                return null;
            }

        }
        /// <summary>
        /// Metodo de ListarZona
        /// </summary>
        /// <returns>Lista con la lista de zonas existenes</returns>
        public CapaEntidad.Zona DetalleZona(string CodZona)
        {

            CapaEntidad.Zona zona = new CapaEntidad.Zona();
            DataTable dtZona = new DataTable();
            try
            {
                dtZona = Access.ObtenerDatos("SELECT * FROM tblZona WHERE IdZona = '" + CodZona + "'").Tables[0];
                if (dtZona.Rows.Count > 0)
                {
                    for (int i = 0; i < dtZona.Rows.Count; i++)
                    {
                        zona.IdZona = dtZona.Rows[i]["IdZona"].ToString();
                        zona.Siglas = dtZona.Rows[i]["Siglas"].ToString();
                        zona.Descripcion = dtZona.Rows[i]["Descripcion"].ToString();
                        zona.Responsable = dtZona.Rows[i]["Responsable"].ToString();
                        zona.FechaRegistro = Convert.ToDateTime(dtZona.Rows[i]["FechaRegistra"]);
                        zona.Estado = Convert.ToInt32(dtZona.Rows[i]["Estado"]);
                    }
                }
                return zona;

            }
            catch (Exception)
            {
                return null;
            }

        }




    }
}
