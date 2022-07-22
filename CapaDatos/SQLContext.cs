using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CapaDatos
{
    public class SQLContext : Configuraciones
    {

        //Permite detectar la conexion a sql server, si existe la reanuda, 
        //si no existe la instancia devolverá error
        protected SqlConnection Conectar(String CadenaConexion)
        {
            try
            {
                // Verificamos si no se ha instanciado a la conexión anteriormente
                if (iConnection == null){
                    iConnection = new SqlConnection(CadenaConexion);
                }

                // Abrimos la conexion
                if (iConnection.State != ConnectionState.Open){
                    iConnection.Open();
                }

            }
            catch (TimeoutException exception){
                iConnection.Close();
                iConnection.Dispose();
                Console.WriteLine("Got {0}", exception.GetType());
            }

            return iConnection;

        }

        /// <summary>
        /// Ejecuta una consulta y devuelve los datos
        /// </summary>
        /// <param name="Query">Consulta que se desea ejecutar en el servidor</param>
        /// <returns>Dataset con el conjunto de resultados de la consulta. Un dataset puede tener varios datatable.</returns>
        public DataSet ObtenerDatos(String Query)
        {
            try{
               
                // Comando para ejecutar las consultas
                SqlCommand cmSQL = new SqlCommand(Query, Conectar(this.strSigh));
                cmSQL.CommandTimeout = 300;


                // Variable del dataset y adaptador para generar los datos
                DataSet dsResult = new DataSet();
                SqlDataAdapter daSQL = new SqlDataAdapter(cmSQL);

                daSQL.Fill(dsResult);

                // Liberamos la memoria
                cmSQL.Dispose();
                daSQL.Dispose();

                iConnection.Close();
                iConnection.Dispose();
                iConnection = null;

                // Resultado
                return dsResult;
            }
            catch (Exception ex){
                throw new Exception("No se pudieron obtener los datos de la consulta..." + Environment.NewLine + ex.Message);
            }
        }


        /// <summary>
        /// Obtiene Registros de una Tabla Utilizando SQLDATAREADER
        /// </summary>
        /// <param name="spname">Nombre del Procedimiento que Obtiene los Datos</param>
        /// <returns>Retorna Registros</returns>
        public DataTable SelectAll(string spname)
        {

            SqlDataReader reader = null;
            SqlCommand cmProcedure = (SqlCommand)CrearComando(spname, this.strSigh);
            DataTable dt = new DataTable();
            try{
                reader = cmProcedure.ExecuteReader();
                if (reader.HasRows){
                    dt.Load(reader);
                } 
            }
            catch (SqlException ex) {
                throw new Exception("Error al Intentar Obtener Registros" + Environment.NewLine + ex.Message);
            }

            //eliminar la conexion
            finally{
                reader.Close();
                iConnection.Close();
                iConnection = null;
            }

            return dt;
        }

        /// <summary>
        /// Crea un comando para ejecutar un procedimiento y obtiene los parametros del procedimiento almacenado
        /// </summary>
        /// <param name="spName">Nombre del procedimiento que se desea ejecutar</param>
        /// <param name="ConnectionString">Cadena de conexion a utilizar</param>
        /// <returns></returns>
        private IDbCommand CrearComando(String spName, String ConnectionString)
        {
            try
            {

                // Creamos una conexión temporal
                SqlConnection cnSQL = new SqlConnection(ConnectionString);
                cnSQL.Open();

                // Creamos el comando para el procedimiento indicado en el parametro
                SqlCommand Comando = new SqlCommand(spName, cnSQL);
                Comando.CommandType = CommandType.StoredProcedure;
                SqlCommandBuilder.DeriveParameters(Comando);

                // Cerramos la conexión temporal
                cnSQL.Close();
                cnSQL.Dispose();

                // Asignamos la conexión principal al procedimiento y retornamos el comando creado
                Comando.Connection = Conectar(ConnectionString);
                return Comando;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el comando hacia la base de datos" + ex.Message);
            }

        }

        /// <summary>
        /// Permite agregar valores a cada uno de los parametros de un comando
        /// </summary>
        /// <param name="Comando">Comando que contiene los parametros</param>
        /// <param name="Valores">Colección de valores que se le asignaran al comando</param>
        private void CargarParametros(IDbCommand Comando, Object[] Valores)
        {
            // Recorremos cada uno de los parametros contenidos en el comando
            for (int i = 1; i < Comando.Parameters.Count; i++)
            {
                // Creamos la variable de tipo parametro SQL y le asignamos el parametro correspondiente del comando
                SqlParameter Parametro = (SqlParameter)Comando.Parameters[i];

                //Asignamos el valor del parametro 
                Parametro.Value = (i <= Valores.Length) ? Valores[i - 1] : null;
            }
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado (sin parametros) en la base de datos
        /// </summary>
        /// <param name="spName">Nombre del procedimiento almacenado que se desea ejecutar</param>
        /// <returns></returns>
        public int EjecutarProcedimiento(String spName, String ConnectionString)
        {
            return CrearComando(spName, ConnectionString).ExecuteNonQuery();
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado (con parametros) en la base de datos
        /// </summary>
        /// <param name="spName">Nombre del procedimiento almacenado que se desea ejecutar</param>
        /// <param name="Parametros">Parametros que necesita el procedimiento, los parametros deben ser indicados en el orden de declaración del procedimiento</param>
        /// <returns>Retorna los parametros conteniendo los valores output</returns>
        public Object EjecutarProcedimiento(String spName, params Object[] Parametros)
        {
            // Creamos el comando
            SqlCommand cmProcedure = (SqlCommand)CrearComando(spName, this.strSigh);

            // Cargamos los parametros del procedimiento
            CargarParametros(cmProcedure, Parametros);

            // Ejecutamos el procedimiento almacenado
            cmProcedure.ExecuteNonQuery();

            // Verificamos los parametros de salida para asignar los valores devueltos
            for (int i = 0; i <= Parametros.Length; i++)
            {
                IDataParameter ParametroTemp = (IDataParameter)cmProcedure.Parameters[i];
                if (ParametroTemp.Direction == ParameterDirection.InputOutput || ParametroTemp.Direction == ParameterDirection.Output)
                {
                    Parametros.SetValue(ParametroTemp.Value, i - 1);
                }
            }

            cmProcedure.Dispose();
            iConnection = null;
            return Parametros;
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado (con parametros) en la base de datos
        /// </summary>
        /// <param name="spName">Nombre del procedimiento almacenado que se desea ejecutar</param>
        /// <param name="Parametros">Parametros que necesita el procedimiento, los parametros deben ser indicados en el orden de declaración del procedimiento</param>
        /// <returns>Retorna los parametros conteniendo los valores output</returns>
        public bool EjecutarTransaccion(List<String> dtConsulta)
        {
            //crear objeto de trasaccion, comando, conexion
            SqlCommand cmSQL = new SqlCommand();
            SqlTransaction transaccion;
            SqlConnection conexion = Conectar(this.strSigh);

            //crear la conexion al comando
            cmSQL.Connection = conexion;

            //crear transaccion
            transaccion = conexion.BeginTransaction(IsolationLevel.ReadCommitted);
            //ejecutar la consulta
            cmSQL.Transaction = transaccion;

            try{
                //ejecutar la consulta
                for (int i = 0; i < dtConsulta.Count; i++){
                    // Comando para ejecutar las consultas
                    cmSQL.CommandText = dtConsulta[i].ToString();                  
                    cmSQL.ExecuteNonQuery();
                    cmSQL.Parameters.Clear();
                }

                //si todo esta bien, ejecutar el comit para aplicar los cambios
                transaccion.Commit();
                //cerrra comando y conexion
                cmSQL.Dispose();
                iConnection.Close();
                iConnection = null;
                //retornar true
                return true;

            }
            catch (Exception){
                //si todo sale mal en el ciclo, deshacer cambios
                transaccion.Rollback();
                //cerrar comando y conexion
                cmSQL.Dispose();
                iConnection.Close();
                iConnection = null;
                //retornar false
                return false;
            }
        }


        /// <summary>
        /// retorna un IDataReader
        /// </summary>
        /// <param name="ConsultaSQL"></param>
        /// <returns></returns>
        public IDataReader TraerDataReaderSql(string ConsultaSQL)
        {
            IDataReader cmq;
            SqlCommand cmd = new SqlCommand(ConsultaSQL, this.Conectar(this.strSigh));

            cmq = cmd.ExecuteReader();


            iConnection.Close();
            iConnection.Dispose();
            iConnection = null;
            cmd.Dispose();

            return cmq;
        }

        //    Public Function TraerDataReaderSql(ConsultaSQL As String) As IDataReader
        //    'Creamos el comando para ejecutar la consulta
        //    Dim cmSql As SqlCommand = New SqlCommand(ConsultaSQL, CType(Conectar, SqlConnection))
        //    Return cmSql.ExecuteReader()
        //    End Function



        /// <summary>
        /// Ejecuta un procedimiento almacenado (con parametros) en la base de datos
        /// </summary>
        /// <param name="consultas">lista de consulta a ejecutar</param>
        /// <param name="isolacion">metodo de insolaction para bloqueos de tablas cuando se ejecute la transaccion</param>
        /// <returns></returns>
        public string EjecutarTransaccion(List<String> consultas, IsolationLevel isolacion)
        {
            //bandera de exito de la transaccion
            string Message = "";
            string strMessage_SQL = "";

            //crear objeto de trasaccion, comando, conexion
            SqlCommand cmSQL = null;
            SqlTransaction transaccion = null;
            SqlConnection conexion = null;

            try {
                //instancia comando SqlCommand
                cmSQL = new SqlCommand();
                //crear conexion
                conexion = Conectar(this.strSigh);
                //crear la conexion del SqlCommand
                cmSQL.Connection = conexion;

                //crear transaccion con niveles de insolacion
                transaccion = conexion.BeginTransaction(isolacion);
                //adjuntar al comando la informacion de la transaccion
                cmSQL.Transaction = transaccion;

                //ejecutar la consulta segun la lista
                for (int i = 0; i < consultas.Count; i++) {
                    //obtener temporalmente la sentencia actual de ejecución
                    strMessage_SQL = consultas[i].ToString();

                    // Comando para ejecutar las consultas
                    cmSQL.CommandText = consultas[i].ToString();
                    cmSQL.ExecuteNonQuery();

                    //limpiar parametros
                    cmSQL.Parameters.Clear();
                }

                //si todo esta bien, ejecutar el commit para aplicar los cambios
                transaccion.Commit();

                //retornar true
                Message = "OK";

            }
            catch (Exception ex) {
                //si todo sale mal en el ciclo, deshacer cambios
                transaccion.Rollback();
                //retornar false
                Message = "Error al Intentar realizar la transacción: " + ex.Message + "=> Revisar la sentencia " + strMessage_SQL;
            }

            finally {
                //cerrar comando y conexion
                cmSQL.Dispose();
                iConnection.Close();
                iConnection = null;
            }


            return Message;
        }


        /// <summary>
        /// Obtiene la estructura de una tabla, incluyendo tipos de datos, tamaños de campos, si acepta o no valores null, etc.
        /// </summary>
        /// <param name="Tabla">Datatable con esquema de la tabla</param>
        /// <returns></returns>
        public DataTable ObtenerEsquemaTabla(String Tabla)
        {
            DataTable dtEsquema;
            // Comando para ejecutar la consulta
            SqlCommand cmSQL = new SqlCommand("Select * From " + Tabla, (SqlConnection)iConnection);

            // Obtenemos los datos del esquema de toda la tabla
            using (IDataReader Reader = cmSQL.ExecuteReader(CommandBehavior.SchemaOnly)){
                dtEsquema = Reader.GetSchemaTable();
            }

            cmSQL.Dispose();
            return dtEsquema;
        }
    }
}
