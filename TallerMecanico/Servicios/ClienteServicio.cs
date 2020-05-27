using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallerMecanico.Modelo;

namespace TallerMecanico.Servicios
{
    /// <summary>
    /// Clase servicio para trabajar con la base de datos, 
    /// gestiona los datos que tienen que ver con los clientes
    /// </summary>
    public class ClienteServicio : ServicioGenerico<cliente>
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private DbContext contexto;

      /// <summary>
      /// Constructor de la clase Servicio
      /// </summary>
      /// <param name="context">contexto de la base de datos taller mecanico</param>
        public ClienteServicio(DbContext context) : base(context)
        {
            contexto = context;
        }

        /// <summary>
        /// Obtiene el ultimo id del cliente que hay en la base de datos
        /// </summary>
        /// <returns></returns>
        public int getLastId()
        {
            cliente cliente = contexto.Set<cliente>().OrderByDescending(a => a.CodigoCliente).FirstOrDefault();
            return cliente.CodigoCliente;
        }

    }
}
