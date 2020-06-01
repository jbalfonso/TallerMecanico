using NLog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallerMecanico.Modelo;

namespace TallerMecanico.Servicios
{
    /// <summary>
    /// Clase servicio para trabajar con la base de datos, 
    /// gestiona los datos que tienen que ver con los empleados
    /// </summary>
    public class EmpleadoServicio : ServicioGenerico<empleado>
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private DbContext contexto;
        private const int EMPLEADO = 1;
        private const int ENCARGADO = 2;
        private const int GERENTE = 3;

        
         /// <summary>
         /// Almacena el empleado que ha iniciado sesion
         /// </summary>
         public empleado empleLogin { get; set; }
        
        /// <summary>
        /// Constructor de la clase servicio
        /// </summary>
        /// <param name="context"></param>
         public EmpleadoServicio(DbContext context) : base(context)
         {
            contexto = context;
         }

       
        /// <summary>
        /// Metodo que comprueba las credenciales del empleado en la base de datos
        /// </summary>
        /// <param name="user">usuario a logearse</param>
        /// <param name="pass">contraseña del usuario a logearse</param>
        /// <returns></returns>
        public Boolean login(String user, String pass)
        {
            Boolean correcto = true;
            try
            {
                empleLogin = contexto.Set<empleado>().Where(u => u.Login == user).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("Error fatal al iniciar sesion",ex);               
            }
            if (empleLogin == null)
            {
                correcto = false;
            }
            else if (!empleLogin.Login.Equals(user) || !empleLogin.Contraseña.Equals(pass))
            {
                correcto = false;
            }

            return correcto;
        }
       
        /// <summary>
        /// Comprueba si en la base de datos existe un empleado con ese login
        /// El login de un empleado debe ser unico
        /// </summary>
        /// <param name="emp">login del empleado</param>
        /// <returns>devuelve true si es unico y no existe ese login en la base de datos, 
        /// devuelve false, si el login existe en la base de datos</returns>
        public Boolean EmpleadoUnico(string emp)
        {
            bool unico = true;
            try
            {
                if (contexto.Set<empleado>().Where(u => u.Login == emp).Count() > 0)
                {
                    unico = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error ya existe un empleado con ese login",ex);                
                unico = false;
            }
            
            return unico;
        }
       
         /// <summary>
         /// Devuelve un empleado en funcion del nombre pasado
         /// </summary>
         /// <param name="nom">Nombre del empleado</param>
         /// <returns>devuelve el empleado que coincida con ese nombre</returns>
        public empleado getEmpleadoPorNombre(string nom)
        {
            empleado emp=null;
            try
            {        
                emp = contexto.Set<empleado>().Where(u => u.Nombre == nom).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error("Error al obtener el empleado a partir del nombre",ex);
            }
            return emp;
        }

        /// <summary>
        /// Obtiene los empleados, los cuales tengan el rol de empleado
        /// </summary>
        /// <returns>La lista de empleados que tengan el rol de empleado</returns>
        public List<empleado> getEmpleados()
        {
            return contexto.Set<empleado>().Where(u => u.rol.CodigoRol == EMPLEADO).ToList();
        }

        /// <summary>
        /// Obtiene los empleados, los cuales tengan el rol de encargados
        /// </summary>
        /// <returns>Devuelve la lista de empleados que tengan el rol de encargado</returns>
        public List<empleado> getEncargados()
        {
            return contexto.Set<empleado>().Where(u => u.rol.CodigoRol == ENCARGADO).ToList();
        }

        /// <summary>
        /// Obtiene los empleados, los cuales tengan el rol de gerentes
        /// </summary>
        /// <returns>Devuelve la lista de empleados que tengan el rol de gerente</returns>
        public List<empleado> getGerentes()
        {
            return contexto.Set<empleado>().Where(u => u.rol.CodigoRol == GERENTE).ToList();
        }

        /// <summary>
        /// Obtiene el ultimo id del empleado de la base de datos
        /// </summary>
        /// <returns>devuelve el codigo del id</returns>
        public int getLastId()
        {
            empleado empl = contexto.Set<empleado>().OrderByDescending(a => a.CodigoEmpleado).FirstOrDefault();
            return empl.CodigoEmpleado;
        }
    }
}
