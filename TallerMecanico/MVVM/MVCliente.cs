﻿using NLog;
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
        private cliente clntNuevo;

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
