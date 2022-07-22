using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CapaNegocio
{
    public class Instituciones
    {
        //referencia a la capa de datos
        CapaDatos.SQLContext sDatos = new CapaDatos.SQLContext();

        //Metodo que permite retorna la lista de instituciones registradas
        public List<CapaEntidad.Institucion> ListaInstitucion()
        {
            //Declarar la lista a retornar
            List<CapaEntidad.Institucion> lista = new List<CapaEntidad.Institucion>();
            //Datatable que guardara el resultado de la consulta a la capa de datos
            DataTable dtInstitucion = new DataTable();
            try{
                //Recolectar datos 
                // TO DO: crear procedimiento almacenado que devuelva las instituciones afiliadas que existen en inversiones.net
                dtInstitucion = sDatos.ObtenerDatos("SELECT * FROM tblClientes_Instituciones").Tables[0];
                //Agregar los resultado a la lista generica
                for (int i = 0; i < dtInstitucion.Rows.Count; i++)
                {
                    lista.Add(new CapaEntidad.Institucion
                    {
                        IdInstitucion = dtInstitucion.Rows[i]["Id_Institucion"].ToString(),
                        Siglas = dtInstitucion.Rows[i]["Siglas"].ToString(),
                        NombreInstitucion = " " + dtInstitucion.Rows[i]["Siglas"].ToString() + "  (" + dtInstitucion.Rows[i]["Nombre"].ToString() + ")",
                    });
                }
            }
            catch (Exception){
                return null;
            }

            //retornar la lista de instituciones
            return lista;
        }
    }
}
