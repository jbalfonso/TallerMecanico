using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TallerMecanico.Modelo;
using TallerMecanico.Servicios;

namespace TallerMecanico.MVVM
{
    /// <summary>
    /// Clase de Gestion MVVM la cual gestiona lo relativo a los roles
    /// </summary>
    public class MVRol:MVBase
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private tallermecanicoEntities tEnt;
        private RolServicio rolServ;
        private PermisoServicio permServ;
        private EmpleadoServicio empServ;
        private permiso permSel;
        private rol rol;

        /// <summary>
        /// Lista de empleados que tienen el rol seleccionado, 
        /// se añaden los empleados en el metodo compruebaRolEmpleado
        /// </summary>
        public List<empleado> empConRol = new List<empleado>();

        /// <summary>
        /// booleano el cual cuando esta en true,
        /// en vez de guardar, edita el rol
        /// </summary>
        public bool editar { get; set; }

        /// <summary>
        /// Gestiona la colleccoin de permisos que tiene el rol
        /// </summary>
        public ICollection<permiso> permisosDrop{get; set;}

        /// <summary>
        /// Constructor de la clase de MVRol
        /// </summary>
        /// <param name="tEnt">entidad de la base de dato taller mecanico</param>
        public MVRol(tallermecanicoEntities tEnt)
        {
            this.tEnt = tEnt;
           
            inicializar();
        }

        /// <summary>
        /// Inicializa las variables de la clase
        /// </summary>
        private void inicializar()
        {
            rol = new rol();
            permSel = new permiso();
            rolServ = new RolServicio(tEnt);
            permServ = new PermisoServicio(tEnt);
            empServ = new EmpleadoServicio(tEnt);
            permisosDrop = new ObservableCollection<permiso>();
        }

        /// <summary>
        /// Gestiona los permisos que tiene el rol seleccionado
        /// </summary>
        public void permisosrolDrop()
        {
            permisosDrop = rolNuevo.permiso;
            
        }

        /// <summary>
        /// Gestiona el permiso seleccionado
        /// </summary>
        public permiso permisoSeleccionado
        {
            get { return permSel; }
            set { permSel = value; OnPropertyChanged("permisoSeleccionado"); }
        }

        /// <summary>
        /// Gestiona el rol seleccionado
        /// </summary>
        public rol rolNuevo
        {
            get { return rol; }
            set { rol = value; OnPropertyChanged("rolNuevo"); }
        }

        /// <summary>
        /// Borra el rol seleccionado
        /// </summary>
        /// <returns>Devuelve true si no hay ninguna excepcion, 
        /// si hay alguna excepcion devuelve false</returns>
        public Boolean borra()
        {
            bool correcto = true;
            try
            {
                rolServ.delete(rolNuevo);
                rolServ.save();
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al borrar una pieza", dbex);

            }
            catch (Exception ex)
            {
                correcto = false;
                logger.Error("Ha habido un problema inesperado al borrar la pieza de la base de datos",ex);
            }
            return correcto;
        }

        /// <summary>
        /// Guarda el rol en la base de datos,
        /// cuando editar esta en true en lugar de guardar, edita el rol
        /// </summary>
        /// <returns>Devuelve true si no hay ninguna excepcion al guardar el rol,
        /// si hay alguna excepcion devuelve false</returns>
        public Boolean guarda()
        {
            bool correcto = true;
            try
            {
                rolNuevo.permiso = permisosDrop;
                if (editar)
                {
                    rolServ.edit(rolNuevo);
                }
                else
                {                    
                    rolNuevo.CodigoRol = rolServ.getLastId() + 1;
                    rolServ.add(rolNuevo);
                }
                rolServ.save();
            }catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al actualizar o al agregar una Pieza", dbex);
            }
            catch (Exception ex)
            {
                correcto = false;
                logger.Error("Ha habido un problema inesperado al guardar la pieza de la base de datos", ex);
            }
            return correcto;
        }

        /// <summary>
        /// Se encarga de gestionar la eliminacion del rol,
        /// si algun empleado contiene este rol devuelve false,
        /// si no lo contiene ningun empleado devuelve true
        /// </summary>
        /// <returns>Devuelve true si el rol no lo tiene ningun empleado, 
        /// devuelve false si algun empleado tiene ese rol</returns>
        public bool compruebaRolEmpleado()
        {
            bool correcto = true;
            try
            {
                empConRol.Clear();
                foreach (empleado emp in empServ.getAll().ToList())
                {
                    if (rolNuevo == emp.rol)
                    {
                        correcto = false;
                        empConRol.Add(emp);
                    }
                }
            }
            catch (Exception ex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al comprobar el rol seleccionado con los roles de los empleados",ex);
            }
            return correcto;
        }

       /* public List<empleado> empleadosConRol
        {
            get { return empConRol;}
            set { empConRol = value; OnPropertyChanged("empleadosConRol"); }
        }*/

        /// <summary>
        /// Devuelve un listado de roles que hay en la base de datos
        /// </summary>
        public List<rol> listaRoles
        {
            get { try { return rolServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido un error inesperado al obtener la lista de los roles de la base de datos","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    logger.Error("Ha habido un error inesperado al obtener la lista de los roles de la base de datos",ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve un listado ed permisos que hay en la base de datos
        /// </summary>
        public List<permiso> listaPermisos
        {
            get { try { return permServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido un error inesperado al obtener la lista de los permisos de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un error inesperado al obtener la lista de los permisos de la base de datos", ex);
                    return null;
                } }
        }
    }
}
