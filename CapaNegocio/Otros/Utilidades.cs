using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class Utilidades
    {

        CapaDatos.SQLContext Access = new CapaDatos.SQLContext();

        //Obtener videos para los televisores
        public List<CapaEntidad.Videos> ListaVideos()
        {
            // crear la lista
            List<CapaEntidad.Videos> lista = new List<CapaEntidad.Videos>();
            //Crear datatable donde se almacenarán temporalmente
            DataTable dtVideo = new DataTable();

            try
            {
                dtVideo = Access.ObtenerDatos("SELECT * FROM tblVideos").Tables[0];
                //recorrer el contenido de la carpeta
                for (int i = 0; i < dtVideo.Rows.Count; i++){
                    lista.Add(new CapaEntidad.Videos{
                            Id = Convert.ToInt32(dtVideo.Rows[i]["IdVideo"]),
                            Titulo = dtVideo.Rows[i]["Titulo"].ToString(),
                            Link = dtVideo.Rows[i]["Link"].ToString(),
                            Poster = dtVideo.Rows[i]["LinkPoster"].ToString(),
                            Artista = dtVideo.Rows[i]["Artista"].ToString(),
                            Formato = dtVideo.Rows[i]["Formato"].ToString()
                    });
                }

            }
            catch (Exception){
                return null;
            }

            return lista;
        }
    }
}
