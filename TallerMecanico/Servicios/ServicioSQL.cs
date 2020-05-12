
using MySql.Data.MySqlClient;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallerMecanico.Modelo;

namespace TallerMecanico.Servicios
{
   public class ServicioSQL
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
       
       /// <summary>
       /// Variable que almacena el contexto de la base de datos
       /// </summary>
        private DbContext contexto;
        /// <summary>
        /// Variable que almacena la conexion con la base de datos
        /// </summary>
        
        private MySqlConnection connection;
        /// <summary>        
        /// Constructor de la clase         
        /// Realiza la conexión con la base de datos         
        /// </summary> 
        public ServicioSQL(DbContext context)
        {
            contexto = context;
            abreConexion();
        }
        /// <summary>
        /// Propiedad pública que devuelve la conexión con la base de datos         
        /// </summary> 
        public MySqlConnection conexion { get { return connection; } }

        private void abreConexion()
        {
            string entitityConnectionString2 = ConfigurationManager.ConnectionStrings["TallerMecanico.Properties.Settings.tallermecanicoConnectionString"].ConnectionString;
                     
            connection = new MySqlConnection(entitityConnectionString2);
            connection.Open();
        }

        /// <summary>
        /// Obtiene el connection string por el nombre del proveedor
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        private static string GetConnectionStringByProvider(string providerName)
        {   // Retorna null si hay fallo             
            string returnValue = null;

            // Obtenemos la cadena de conexión del fichero de configuración App.config             
            ConnectionStringSettingsCollection settings = ConfigurationManager.ConnectionStrings;

            // Si encontramos cadenas de configuración en el fichero App.config    
            if (settings != null)
            {
                // Recorremos todas las cadenas de conexión del fichero            
                foreach (ConnectionStringSettings cs in settings)
                {
                    if (cs.ProviderName == providerName)
                    {
                        returnValue = cs.ConnectionString;
                        break;
                    }
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Obtiene los datos mediante un DataTable, pasandole la sentencia sql a ejecutar
        /// </summary>
        /// <param name="query">Sentencia sql a ejecutar</param>
        /// <returns>Obtiene un DataTable, con los datos obtenidos</returns>
        public DataTable getDatos(string query)
        {             // Creamos un DataSet             
            DataSet ds = new DataSet();
            // Obtenemos los datos en función de la conexión y de la sentencia SELECT             
            MySqlDataAdapter adapt = new MySqlDataAdapter(query, connection);
            // Guardamos los datos en el DataAdapter (Equivalente a ResulSet)             
            adapt.Fill(ds);
            // Devolvemos el DataAdapter            
            return ds.Tables[0];
        }
        }
}
