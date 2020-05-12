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
    /// gestiona los datos que tienen que ver con los permisos
    /// </summary>
    public class PermisoServicio:ServicioGenerico<permiso>
    {
        private DbContext contexto;
        private RolServicio rolServ;

        /// <summary>
        /// Constructor de la clase servicio
        /// </summary>
        /// <param name="context"></param>
        public PermisoServicio(DbContext context) : base(context)
        {
            this.contexto = context;

        }

        /// <summary>
        /// Obtiene el ultimo id del permiso de la base de datos
        /// </summary>
        /// <returns>Devuelve el id del permiso de la base de datos</returns>
        public int getLastId()
        {
            permiso permiso = contexto.Set<permiso>().OrderByDescending(a => a.CodigoPermiso).FirstOrDefault();
            return permiso.CodigoPermiso;
        }
    }
}
