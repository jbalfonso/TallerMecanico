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
    /// gestiona los datos que tienen que ver con las averias
    /// </summary>
    public class AveriaServicio : ServicioGenerico<averia>
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private DbContext contexto;

       /// <summary>
       /// Constructor de la clase servicio
       /// </summary>
       /// <param name="context"></param>
        public AveriaServicio(DbContext context) : base(context)
        {
            contexto = context;
        }

        /// <summary>
        /// Obtiene las averias por el tipo de averia
        /// </summary>
        /// <param name="tipo">tipo de la averia</param>
        /// <returns>devuelve una lista de averias segun el tipo de averia pasado</returns>
        public List<averia> getAveriasPorTipo(string tipo)
        {
            return contexto.Set<averia>().Where(m => m.Tipo == tipo).ToList();
        }

        /// <summary>
        /// Obtiene las averias por el estado
        /// </summary>
        /// <param name="estado">Estado de averia</param>
        /// <returns>Devuelve una lista de averias segun el estado de la averia pasado</returns>
        public List<averia> getAveriasPorEstado(string estado)
        {
            return contexto.Set<averia>().Where(m => m.Estado == estado).ToList();
        }

        /// <summary>
        /// Obtiene el ultimo id que esta en la base de datos
        /// </summary>
        /// <returns>devuelve el id de la ultima averia que esta en la base de datos</returns>
        public int getLastId()
        {
            averia averia = contexto.Set<averia>().OrderByDescending(a => a.CodigoAveria).FirstOrDefault();
            return averia.CodigoAveria;
        }

        public DateTime fechaMaxima { get { return getAll().Max(v => v.FechaRecepcion); } }
        public DateTime fechaMinima { get { return getAll().Min(v => v.FechaRecepcion); } }
    }
}
