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
    /// gestiona los datos que tienen que ver con las piezas
    /// </summary>
    public class PiezaServicio : ServicioGenerico<pieza>
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private DbContext contexto;

       
         /// <summary>
         /// Constructor de la clase servicio
         /// </summary>
         /// <param name="context"></param>
        public PiezaServicio(DbContext context) : base(context)
        {
            contexto = context;
        }
        /// <summary>
        /// Obtiene el ultimo id de la pieza de la base de datos
        /// </summary>
        /// <returns>Devuelve el id de la pieza</returns>
        public int getLastId()
        {
            pieza pieza = contexto.Set<pieza>().OrderByDescending(a => a.CodigoPieza).FirstOrDefault();
            return pieza.CodigoPieza;
        }

        /// <summary>
        /// Obtiene todos los codigo de las piezas que estan en la base de datos
        /// </summary>
        /// <returns>Devuelve una lista con el codigo de las piezas</returns>
        public List<int> getCodigopieza()
        {            
            var query=contexto.Database.SqlQuery<int>("select CodigoPieza from Pieza");
            return query.ToList();               
        }
    }
}
