using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace CapaNegocio
{
    public class Cliente
    {

        //propiedad objeto a la capa de datos
        private CapaDatos.SQLContext DContexto = new CapaDatos.SQLContext();


        //Listado de clientes registrados en el SIGH
        //devolverá una lista de tipo objeto Cliente
        public List<CapaEntidad.Cliente> ListadoCliente()
        {
            //crear la lista
            List<CapaEntidad.Cliente> lista = new List<CapaEntidad.Cliente>();
            DataTable dtCliente = new DataTable();
            string strSQL = "";

            try{
                strSQL = "EXEC spr_ListaCliente";
                dtCliente = DContexto.ObtenerDatos(strSQL).Tables[0];
                if (dtCliente.Rows.Count > 0){
                    for (int i = 0; i < dtCliente.Rows.Count; i++){
                        lista.Add(new CapaEntidad.Cliente {
                            IdCliente = dtCliente.Rows[i]["IdCliente"].ToString(),
                            Cedula = dtCliente.Rows[i]["Cedula"].ToString(),
                            CodEmpleado = dtCliente.Rows[i]["CodEmp"].ToString(),
                            NoAsegurado = dtCliente.Rows[i]["NoAsegurado"].ToString(),
                            NombreCompleto = dtCliente.Rows[i]["NombreCompleto"].ToString(),

                            Id_Institucion = dtCliente.Rows[i]["Institucion"].ToString(),
                            NombreInstitucion = dtCliente.Rows[i]["Nombre"].ToString(),
                            FechaIngreso = dtCliente.Rows[i]["FechaIngreso"].ToString(),
                            Cargo = dtCliente.Rows[i]["Cargo"].ToString(),
                            Ocupacion = dtCliente.Rows[i]["Ocupacion"].ToString(),

                            Id_TipoPension = dtCliente.Rows[i]["IdPension"].ToString(),
                            TipoPension = dtCliente.Rows[i]["Descripcion"].ToString(),

                            Pasaporte = dtCliente.Rows[i]["Pasaporte"].ToString(),
                            CodFiscal = dtCliente.Rows[i]["CodFiscal"].ToString(),
                            Sexo = dtCliente.Rows[i]["Sexo"].ToString(),
                            TipoCliente = Convert.ToInt32(dtCliente.Rows[i]["IdPersona"]),
                            DCliente = dtCliente.Rows[i]["DescripcionPersona"].ToString(),
                            Estado = Convert.ToInt32(dtCliente.Rows[i]["Estado"]),
                        });
                    }
                }
            }
            catch (Exception){
                return null;
            }


            return lista;
        }

        //Información del cliene registrados en el SIGH
        //devolverá un objeto que contendrá en sus propiedades la información
        //que existe en BD de dicho cliente
        public CapaEntidad.Cliente DetalleCliente(string IdCliente)
        {
            // crear objeto cliente
            CapaEntidad.Cliente cliente = new CapaEntidad.Cliente();
            DataTable dtCliente = new DataTable();
            string strSQL = "";

            try{
                //ejecutar consulta
                strSQL = "EXEC spr_ClienteLista '" + IdCliente + "'";
                dtCliente = DContexto.ObtenerDatos(strSQL).Tables[0];

                //asignar los valores obtenidos de la consultas a las propiedades del objeto
                if (dtCliente.Rows.Count > 0){
                    for (int i = 0; i < dtCliente.Rows.Count; i++){
                         cliente.IdCliente = dtCliente.Rows[i]["IdCliente"].ToString();
                         cliente.Cedula = dtCliente.Rows[i]["Cedula"].ToString();
                         cliente.CodEmpleado = dtCliente.Rows[i]["CodEmp"].ToString();
                         cliente.NoAsegurado = dtCliente.Rows[i]["NoAsegurado"].ToString();
                         cliente.NombreCompleto = dtCliente.Rows[i]["NombreCompleto"].ToString();

                         cliente.Id_Institucion = dtCliente.Rows[i]["Institucion"].ToString();
                         cliente.NombreInstitucion = dtCliente.Rows[i]["Nombre"].ToString();
                         cliente.FechaIngreso = dtCliente.Rows[i]["FechaIngreso"].ToString();
                         cliente.Cargo = dtCliente.Rows[i]["Cargo"].ToString();
                         cliente.Ocupacion = dtCliente.Rows[i]["Ocupacion"].ToString();

                         cliente.Id_TipoPension = dtCliente.Rows[i]["IdPension"].ToString();
                         cliente.TipoPension = dtCliente.Rows[i]["Descripcion"].ToString();

                         cliente.Pasaporte = dtCliente.Rows[i]["Pasaporte"].ToString();
                         cliente.CodFiscal = dtCliente.Rows[i]["CodFiscal"].ToString();
                         cliente.Sexo = dtCliente.Rows[i]["Sexo"].ToString();
                         cliente.TipoCliente = Convert.ToInt32(dtCliente.Rows[i]["IdPersona"]);
                         cliente.DCliente = dtCliente.Rows[i]["DescripcionPersona"].ToString();
                         cliente.Estado = (dtCliente.Rows[i]["Estado"] is DBNull) ? 0 : Convert.ToInt32(dtCliente.Rows[i]["Estado"]);   
                    }
                }

            }
            catch (Exception){
                return null;
            }


            return cliente;
        }



        //Listado de clientes en inversionesNET, devolvera los registros de activos y/o pensionados
        //que existen
        public List<CapaEntidad.Cliente> ListadoCliente_Inversiones(string filter = null, string campo = null, string valor = null)
        {
            //almacenará la lista de clientes que existen en inversiones
            List<CapaEntidad.Cliente> ListadoCliente = new List<CapaEntidad.Cliente>();

            //crear la consulta en base a la cedula
            string strConsulta = (filter == null) ? "spr_ClientesLista_InversionesNET null, '" + campo + "', '" + valor + "'"  : "spr_ClientesLista_InversionesNET '" + filter + "'";

            DataTable dtClientes = new DataTable();
            try{
                dtClientes = DContexto.ObtenerDatos(strConsulta).Tables[0];
                if (dtClientes.Rows.Count > 0){
                    for (int i = 0; i < dtClientes.Rows.Count; i++){
                        ListadoCliente.Add(new CapaEntidad.Cliente {
                            IdCliente = dtClientes.Rows[i]["Id_Cliente"].ToString(),
                            Cedula = dtClientes.Rows[i]["Cedula"].ToString(),
                            CodEmpleado = dtClientes.Rows[i]["Id_Emp"].ToString(),
                            NoAsegurado = dtClientes.Rows[i]["Id_Issdhu"].ToString(),
                            NombreCompleto = dtClientes.Rows[i]["Nombres_Apellidos"].ToString().Replace("  ", " ").Trim(),
                            Id_Institucion = dtClientes.Rows[i]["Id_Institucion"].ToString(),
                            NombreInstitucion = dtClientes.Rows[i]["NombreInstitucion"].ToString().Trim(),
                            Id_TipoPension = dtClientes.Rows[i]["Id_TipoPension"].ToString(),
                            TipoPension = dtClientes.Rows[i]["TipoPension"].ToString(),
                            FechaIngreso = (Convert.ToDateTime(dtClientes.Rows[i]["Fecha_Ingreso"])).ToString("dd/MM/yyyy"),
                            Cargo = dtClientes.Rows[i]["Cargo"].ToString().Trim(),
                            Ocupacion = dtClientes.Rows[i]["Ocupacion"].ToString().Trim(),
                            CodFiscal = dtClientes.Rows[i]["Id_Fiscal"].ToString().Trim(),
                            Sexo = dtClientes.Rows[i]["Sexo"].ToString().Trim(),
                            Estado = Convert.ToInt32(dtClientes.Rows[i]["Estado"]),
                            TipoCliente = 1,
                        });
                    }
                }

                return ListadoCliente;
            }
            catch (Exception){
                return null;
            }
        }

       
    }
}
