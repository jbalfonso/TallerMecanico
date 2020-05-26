using NLog;
using System;
using System.Collections.Generic;
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
    /// Clase de Gestion MVVM la cual gestiona lo relativo a los empleados
    /// </summary>
    public class MVEmpleado:MVBase
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private tallermecanicoEntities tEnt;
        private EmpleadoServicio empServ;
        private RolServicio rolServ;
        private empleado empNuevo;

        /// <summary>
        /// booleano el cual cuando esta en true,
        /// en vez de guardar, edita la averia
        /// </summary>
        public bool editar { get; set; }

        /// <summary>
        /// Constructor de la clase MVEmpleado
        /// </summary>
        /// <param name="ent">entidad de la base de datos taller mecanico</param>
        public MVEmpleado(tallermecanicoEntities ent)
        {
            this.tEnt=ent;
            inicializar();
        }

        /// <summary>
        /// Inicializa las variables de la clase
        /// </summary>
        private void inicializar()
        {
            empNuevo = new empleado();
            empServ = new EmpleadoServicio(tEnt);
            rolServ = new RolServicio(tEnt);
        }

        /// <summary>
        /// Gestiona el empleado Seleccionado
        /// </summary>
        public empleado empleadoNuevo
        {
            get
            {
                return empNuevo;
            }
            set
            {
                empNuevo = value;
                OnPropertyChanged("empleadoNuevo");
            }
        }

        /// <summary>
        /// Borra el empleado que este seleccionado
        /// </summary>
        /// <returns>Devuelve true si no ha habido ninguna excepcion, 
        /// devuelve false si hay alguna excepcion</returns>
        public Boolean borrar()
        {
            bool correcto = true;
            try
            {
                empServ.delete(empleadoNuevo);
                empServ.save();
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al borrar una pieza", dbex);

            }
            catch (Exception ex)
            {
                logger.Error("Ha habido error inesperado al borrar el empleado de la base de datos", ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Guarda un empleado en la base de datos, si editar esta en true, edita el empleado
        /// </summary>
        /// <returns>Devuelve true si no ha habido ninguna excepcion, si hay alguna excepcion devuelve false</returns>
        public Boolean guarda()
        {
            bool correcto = true;           
            try
            {            
                    if (editar)
                    {
                        empServ.edit(empleadoNuevo);
                    }
                    else
                    {
                        empleadoNuevo.CodigoEmpleado = empServ.getLastId() + 1;
                        empServ.add(empleadoNuevo);
                    }               
                empServ.save();                
            }catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al actualizar o al agregar un empleado",dbex);
            }
            catch (Exception ex)
            {
                logger.Error("Ha habido error inesperado al guardar el empleado de la base de datos", ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Gestiona la modificacion del empleado pasado como parametro
        /// </summary>
        /// <param name="empleadoModificar">empleado a modificar</param>
        /// <returns>devuelve true si todo es correcto, si hay algun problema devuelve false</returns>
        public Boolean modificaEmpleado(empleado empleadoModificar)
        {
            bool correcto = true;
            try
            {               
                empServ.edit(empleadoModificar);               
                empServ.save();
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al actualizar un empleado",dbex);
            }
            catch (Exception ex)
            {
                logger.Error("Ha habido error inesperado al modificar el empleado de la base de datos", ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Comprueba si el login seleccionado del empleado es unico y no existe en la base de datos, 
        /// lo hace llamando al metodo de la clase servicio empleadoUnico
        /// </summary>
        /// <returns>Devuelve true si el empleado es unico, 
        /// devuelve false si el login del empleado ya existe en la base de datos</returns>
        public Boolean comprobarLoginUnico()
        {
            bool correcto = true;
            try
            {
                if (!empServ.EmpleadoUnico(empleadoNuevo.Login))
                {
                    correcto = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al comprobar el login en la base de datos","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                logger.Error("Error al comprobar el login del usuario en la base de datos",ex);
                correcto = false;
            }            
            return correcto;
        }

        /// <summary>
        /// Comprueba si el login del empleado pasado como parametro es unico
        /// </summary>
        /// <param name="empleadoVerificar">empleado a comprobar el login</param>
        /// <returns>devuelve true si todo es correcto, devuelve false si hay algun problema</returns>
        public Boolean comprobarLoginEmpleado(empleado empleadoVerificar)
        {
            bool correcto = true;
            try
            {
                if (!empServ.EmpleadoUnico(empleadoVerificar.Login))
                {
                    correcto = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al comprobar el login en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Error("Error al comprobar el login del usuario en la base de datos", ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Devuelve la lista de roles, que hay en la base de datos
        /// </summary>
        public List<rol> listaRoles
        {

            get { try
                {
                    return rolServ.getAll().ToList();
                } catch (Exception ex) {
                    MessageBox.Show("Error al obtener los roles","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    logger.Error("Ha habido un error al obtener los roles de la base de datos",ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve un listado de empleados, que hay en la base de datos
        /// </summary>
        public List<empleado> listaEmpleados
        {
            get { try { return empServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Error al obtener los empleados", Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un error al obtener los empleados de la base de datos",ex);
                    return null;
                } }
        }
        
    }
}
