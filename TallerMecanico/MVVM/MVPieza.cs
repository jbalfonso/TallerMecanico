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
    /// Clase de Gestion MVVM la cual gestiona lo relativo a las piezas
    /// </summary>
    public class MVPieza:MVBase
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private tallermecanicoEntities tEnt;
        private AveriaServicio avServ;
        private PiezaServicio pzaServ;
        private pieza pzaNueva;

        /// <summary>
        /// booleano el cual cuando esta en true,
        /// en vez de guardar, edita la pieza
        /// </summary>
        public bool editar { get; set; }

        private int codigoPieza;
        private string descripcion;

        /// <summary>
        /// Predicado de filtros para los filtros
        /// del control de usuario de busqueda de piezas
        /// </summary>
        public Predicate<object> predicadoFiltro;

        /// <summary>
        /// Lista de predicados, los cuales se aplican 
        /// al control de usuario de busqueda de piezas
        /// </summary>
        public List<Predicate<pieza>> criteriosPieza;

        /// <summary>
        /// Constructor de la clase de MVPieza
        /// </summary>
        /// <param name="ent">Entidad de la base de datos taller mecanico</param>
        public MVPieza(tallermecanicoEntities ent)
        {
            this.tEnt = ent;
            inicializar();
        }

        /// <summary>
        /// Inicializa las variables de la clase
        /// </summary>
        private void inicializar()
        {
            pzaNueva = new pieza();
            avServ = new AveriaServicio(tEnt);
            pzaServ = new PiezaServicio(tEnt);
            criteriosPieza = new List<Predicate<pieza>>();
            predicadoFiltro = new Predicate<object>(filtroCombinado);
        }

        /// <summary>
        /// Gestiona el texto del filtro descripcion
        /// </summary>
        public string txt_filtroDescripcion
        {
            get { return descripcion; }
            set { descripcion = value; OnPropertyChanged("txt_filtroTipo"); }
        }

        /// <summary>
        /// Gestiona el codigo de la pieza seleccionado para el filtro del codigo
        /// </summary>
        public int txt_filtroCodigoPieza
        {
            get { return codigoPieza; }
            set { codigoPieza = value; OnPropertyChanged("txt_filtroCodigoPieza"); }
        }

        /// <summary>
        /// Gestiona el filtro combinado del control de usuario busqueda pieza
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Devuelve false, si criteriosPieza es igual a cualquier numero que no sea 0,
        /// si se cumple los criterios, devuelve true</returns>
        private bool filtroCombinado(object item)
        {
            bool esta = false;
            if (item != null)
            {
                pieza p = (pieza)item;
                if(criteriosPieza.Count != 0)
                {
                    if(criteriosPieza.TrueForAll(x => x(p)))
                    {
                        esta = true;
                    }
                }
            }
            return esta;
        }

        /// <summary>
        ///  Gestiona la pieza seleccionada
        /// </summary>
        public pieza piezaNueva
        {
            get { return pzaNueva; }
            set { pzaNueva = value; OnPropertyChanged("piezaNueva"); }
        }

        /// <summary>
        /// Borra la pieza seleccionada
        /// </summary>
        /// <returns>devuelve true si no ha habido ninguna excepcion,
        /// si hay alguna excepcion devuelve false</returns>
        public Boolean borra()
        {
            bool correcto = true;
            try
            {
                pzaServ.delete(piezaNueva);
                pzaServ.save();
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al borrar una pieza", dbex);

            }
            catch (Exception ex)
            {
                correcto = false;
                logger.Error("Ha habido un error inesperado al borrar la pieza de la base de datos",ex);
            }
            return correcto;
        }

        /// <summary>
        /// Guarda la pieza seleccionada, 
        /// si editar es true en lugar de guardar la pieza la edita
        /// </summary>
        /// <returns>Devuelve true si no hay ninguna excepcion al guardar la pieza,
        /// si hay alguna excepcion devuelve false</returns>
        public Boolean guarda()
        {
            bool correcto = true;
            try
            {
                if (editar)
                {
                    pzaServ.edit(piezaNueva);
                }
                else
                {
                    pzaNueva.CodigoPieza = pzaServ.getLastId() + 1;
                    pzaServ.add(piezaNueva);
                }
                pzaServ.save();

            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al actualizar o al agregar una Pieza", dbex);               
            }
            catch (Exception ex)
            {
                correcto = false;
                logger.Error("Ha habido un error inesperado al guardar la pieza en la base de datos", ex);
            }
            return correcto;
        }

        /// <summary>
        /// Devuelve un listado de piezas que hay en la base de datos
        /// </summary>
        public List<pieza> listaPiezas
        {
            get { try { return pzaServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido un error inesperado al obtener las piezas de la base de datos","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    logger.Error("Ha habido un error inesperado al obtener las piezas de la base de datos",ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve un listado de codigo de piezas que hay en la base de datos
        /// </summary>
        public List<int> listaCodigoPiezas
        {
            get { try { return pzaServ.getCodigopieza(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido un error inesperado al obtener el listado del codigo de las piezas, de la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un error inesperado al obtener el listado del codigo de las piezas, de la base de datos", ex);
                    return null;
                } }
        }
    }
}
