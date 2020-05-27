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
    /// Clase de Gestion MVVM la cual gestiona lo relativo a los clientes
    /// </summary>
    public class MVCliente:MVBase
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private tallermecanicoEntities tEnt;
        private ClienteServicio clientServ;
        private AveriaServicio avServ;
        private cliente clntNuevo;

        /// <summary>
        /// Lista de averias que tienen el cliente seleccionado,
        /// se añaden las averias en el metodo compruebaClienteAveria
        /// </summary>
        public List<averia> clienteConAveria = new List<averia>();

        /// <summary>
        /// booleano el cual cuando esta en true,
        /// en vez de guardar, edita la averia
        /// </summary>
        public bool editar { get; set; }

        /// <summary>
        /// Constructor de la clase MvCliente
        /// </summary>
        /// <param name="ent">Entidad de la base de datos de taller mecanico</param>
        public MVCliente(tallermecanicoEntities ent)
        {
            this.tEnt = ent;
            inicializar();
        }

        /// <summary>
        /// Inicializa las variables de la clase
        /// </summary>
        private void inicializar()
        {
            clientServ = new ClienteServicio(tEnt);
            avServ = new AveriaServicio(tEnt);
            clntNuevo = new cliente();
        }

        /// <summary>
        /// Gestiona el cliente seleccionado
        /// </summary>
        public cliente clienteNuevo
        {
            get { return clntNuevo; }
            set { clntNuevo = value; OnPropertyChanged("clienteNuevo"); }
        }

        /// <summary>
        /// Este metodo comprueba que el cliente a borrar no tenga averias en la base de datos
        /// si tiene averias, estas se añaden a clienteConRol
        /// </summary>
        /// <returns>Devuelve true si no tiene averias en la base de datos, devuelve false, si hay averias en la base de datos</returns>
        public bool compruebaClienteAveria()
        {
            bool correcto = true;
            try
            {
                clienteConAveria.Clear();
                foreach (averia av in avServ.getAll().ToList())
                {
                    if (clienteNuevo == av.cliente1)
                    {
                        correcto = false;
                        clienteConAveria.Add(av);
                    }
                }
            }
            catch (Exception ex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al comprobar el rol seleccionado con los roles de los empleados", ex);
            }
            return correcto;
        }

        /// <summary>
        /// Gestiona el borrado del cliente
        /// </summary>
        /// <returns>Devuelve true si no ha habido ninguna excepcion, devuelve false si ha habido alguna excepcion</returns>
        public Boolean borra()
        {
            bool correcto = true;
            try
            {               
                clientServ.delete(clienteNuevo);
                clientServ.save();
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al borrar un cliente", dbex);

            }
            catch (Exception ex)
            {
                logger.Error("Ha habido error inesperado al borrar el cliente de la base de datos",ex);
                correcto = false;
            }
            return correcto;
        }
       

        /// <summary>
        /// Guarda el cliente en la base de datos, si editar es true, edita el cliente
        /// </summary>
        /// <returns>Devuelve true si no ha habido ningun problema, 
        /// si ha habido algun problema devuelve false</returns>
        public Boolean guarda()
        {
            bool correcto = true;
            try
            {
                if (editar)
                {
                    clientServ.edit(clienteNuevo);
                }
                else
                {
                    clienteNuevo.CodigoCliente = clientServ.getLastId() + 1;
                    clientServ.add(clienteNuevo);
                }
                clientServ.save();
            }

            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al actualizar o al agregar un Cliente", dbex);
            }
            catch (Exception ex)
            {
                logger.Error("Ha habido error inesperado al guardar el cliente de la base de datos", ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Gestiona la modificacion del cliente pasado como parametro
        /// </summary>
        /// <param name="clienteModificar">Cliente a modificar</param>
        /// <returns>Devuelve true si todo es correcto, devuelve false si hay algun problema</returns>
        public Boolean modificaCliente(cliente clienteModificar)
        {
            bool correcto = true;
            try
            {                
                clientServ.edit(clienteModificar);             
                clientServ.save();
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al actualizar un Cliente", dbex);
            }
            catch (Exception ex)
            {
                logger.Error("Ha habido error inesperado al actualizar el cliente de la base de datos", ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Devuelve una lista de clientes que hay en la base de datos
        /// </summary>
        public List<cliente> listaClientes
        {
            get { try { return clientServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido un error inesperado al obtener los clientes de la base de datos", "Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    logger.Error("Ha habido un error inesperado al obtener los clientes de la base de datos",ex);
                    return null;
                } }
        }

    }
}
