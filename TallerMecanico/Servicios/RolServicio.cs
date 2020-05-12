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
    /// gestiona los datos que tienen que ver con los roles
    /// </summary>
    public class RolServicio : ServicioGenerico<rol>
    {
        private DbContext contexto;
        private RolServicio rolServ;

        /// <summary>
        /// Constructor de la clase servicio
        /// </summary>
        /// <param name="context">contexto de la base de datos</param>
        public RolServicio(DbContext context) : base(context)
        {
            this.contexto = context;
           
        }

        /// <summary>
        /// Obtiene el ultimo id del rol de la base de datos
        /// </summary>
        /// <returns>devuelve el ultimo id</returns>
        public int getLastId()
        {
            rol rol = contexto.Set<rol>().OrderByDescending(a => a.CodigoRol).FirstOrDefault();
            return rol.CodigoRol;
        }

    }
}
