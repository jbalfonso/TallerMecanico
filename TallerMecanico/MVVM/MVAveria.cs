using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Clase de Gestion MVVM la cual gestiona lo relativo a las averias
    /// </summary>
    public class MVAveria : MVBase
    {
        
        private Logger logger = LogManager.GetCurrentClassLogger();
        private tallermecanicoEntities tEnt;
        private EmpleadoServicio empServ;
        private AveriaServicio avServ;
        private ClienteServicio clientServ;
        private PiezaServicio pzaServ;
        private averia avNueva;
        private pieza pzaSel;

        /// <summary>
        /// booleano el cual cuando esta en true,
        /// en vez de guardar, edita la averia
        /// </summary>
        public bool editar { get; set; }
        private List<string> estados = new List<string>();
        private List<string> estadosNoDevueltos = new List<string>();
        private List<pieza> piezasModificacion = new List<pieza>();

        private DateTime fechaInc;
        private DateTime fechaFin;

        private string tipo;
        private string estado;

        /// <summary>
        /// Predicado de filtros para los filtros
        /// del control de usuario de busqueda de averias
        /// </summary>
        public Predicate<object> predicadoFiltro;

        /// <summary>
        /// Lista de predicados, los cuales se aplican 
        /// al control de usuario de busqueda de averias
        /// </summary>
        public List<Predicate<averia>> criteriosAveria;

        /// <summary>
        /// Constructor de la clase MVAveria
        /// </summary>
        /// <param name="ent">entidad de la base de datos taller mecanico</param>
        public MVAveria(tallermecanicoEntities ent)
        {
            this.tEnt = ent;

            inicializar();
            inicializaEstados();
        }
        /// <summary>
        /// Metodo el cual inicializa todas las variables, cuando se crea la clase
        /// </summary>
        private void inicializar()
        {
            try
            {
                avNueva = new averia();
                empServ = new EmpleadoServicio(tEnt);
                avServ = new AveriaServicio(tEnt);
                clientServ = new ClienteServicio(tEnt);
                pzaServ = new PiezaServicio(tEnt);

                fechaInc = avServ.fechaMinima;
                fechaFin = avServ.fechaMaxima;
                criteriosAveria = new List<Predicate<averia>>();
                predicadoFiltro = new Predicate<object>(filtroCombinado);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha habido un error al inicializar la clase MVAveria","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                logger.Error("Error al inicializar componentes de la clase MVAveria",ex);
            }
        }
        /// <summary>
        /// Inicializa las variables de los estados de la averia
        /// </summary>
        private void inicializaEstados()
        {
            estados.Add("Finalizado");
            estados.Add("Pendiente");
            estados.Add("En proceso");
            estados.Add("En espera");
            estados.Add("Devuelto");
            estadosNoDevueltos.Add("Finalizado");
            estadosNoDevueltos.Add("Pendiente");
            estadosNoDevueltos.Add("En proceso");
            estadosNoDevueltos.Add("En espera");
            
        }
        /// <summary>
        /// Metodo el cual reduce el stock de las piezas que contiene la averia seleccionada
        /// </summary>
        /// <returns>Devuelve true si no hay ningun error y todo es correcto, 
        /// devuelve false si hay algun problema</returns>
        public bool reduceStock()
        {
            bool correcto = true;
            try
            {
                ICollection<pieza> piezas = averiaNueva.pieza;
                foreach (pieza p in piezas)
                {
                    int cantidad = p.Cantidad;
                    p.Cantidad = cantidad - 1;

                    pzaServ.edit(p);
                    pzaServ.save();
                }
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema en la base de datos al reducir el stock de la pieza", dbex);
            }
            catch (Exception ex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al reducir el stock de la pieza", ex);
            }
            return correcto;
            
        }

        /// <summary>
        /// Metodo el cual aumenta el stock de las piezas que contiene la averia seleccionada
        /// </summary>
        /// <returns>Devuelve true si no hay ningun problema, 
        /// si no hay ninguna excepcion,si hay alguna excepcion devuelve false </returns>
        public bool aumentaStock()
        {
            bool correcto = true;
            try
            {
                ICollection<pieza> piezas = averiaNueva.pieza;
                foreach (pieza p in piezas)
                {
                    int cantidad = p.Cantidad;
                    p.Cantidad = cantidad + 1;
                    pzaServ.edit(p);
                    pzaServ.save();
                }
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema en la base de datos al aumentar el stock de la pieza", dbex);
            }
            catch (Exception ex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al aumentar el stock de la pieza", ex);
            }
            return correcto;

        }
        /// <summary>
        /// Modifica el stock de las piezas que tiene la averia seleccionada, 
        /// dependiendo de si al editar la averia, se añaden piezas o se retiran,
        /// se modifica el stock aumentandolo o reduciendolo
        /// </summary>
        /// <returns>Devuelve true, si no hay ninguna excepcion y todo es correcto, 
        /// devuelve false si hay algun problema</returns>
        public bool modificaStock(averia averia)
        {
            bool correcto = true;
            try
            {
                foreach (pieza pza in piezasModificacion)
                {
                    if (!averiaNueva.pieza.Contains(pza))
                    {
                        pza.Cantidad = pza.Cantidad + 1;
                        pzaServ.edit(pza);
                        pzaServ.save();
                    }
                }
                foreach (pieza pza in averia.pieza)
                {
                    if (!piezasModificacion.Contains(pza))
                    {
                        pza.Cantidad = pza.Cantidad - 1;
                        pzaServ.edit(pza);
                        pzaServ.save();
                    }
                }
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al modificar el stock de la pieza, despues de modificar una averia en la base de datos",dbex);

            }
            catch (Exception ex)
            {
                correcto = false;
                MessageBox.Show("Ha habido un error al modificar el stock en la base de datos","",MessageBoxButton.OK,MessageBoxImage.Error);
                logger.Error("Ha habido un error inesperado al modificar el stock de la pieza despues de modificar una averia en la base de datos",ex);

            }

            return correcto;
        }

        /// <summary>
        /// Gestor del listado de piezas de antes de modificar la averia
        /// </summary>
        public List<pieza> piezasModificaciones
        {
            get { return piezasModificacion; }
            set { piezasModificacion = value; OnPropertyChanged("piezasModificaciones"); }
        }

        /// <summary>
        /// Gestor de la fecha maxima que se debe utilizar
        /// </summary>
        public DateTime fechaMaxima
        {
            get { try { return avServ.fechaMaxima; } catch (Exception ex) {
                    MessageBox.Show("Ha habido un error al obtener la fechaMaxima","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    logger.Error("Error al obtener la fecha maxima de la base de datos",ex);
                    return new DateTime(); } }
        }

        /// <summary>
        /// Gestor de la fecha minima que se debe utilizar
        /// </summary>
        public DateTime fechaMinima
        {
            get { try { return avServ.fechaMinima; } catch (Exception ex)
                {
                    MessageBox.Show("Ha habido un error al obtener la fechaMinima", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Error al obtener la fecha minima de la base de datos", ex);
                    return new DateTime();
                } }
        }

        /// <summary>
        /// Gestor de la propiedad tipo, 
        /// para el filtro por tipo del dialogo de busqueda de averia
        /// </summary>
        public string txt_filtroTipo
        {
            get { return tipo; }
            set { tipo = value; OnPropertyChanged("txt_filtroTipo"); }
        }

        /// <summary>
        /// Gestor de la propiedad estado
        /// para el filtro por estado del dialogo de busqueda de averia
        /// </summary>
        public string txt_filtroEstado
        {
            get { return estado; }
            set { estado = value; OnPropertyChanged("txt_filtroEstado"); }
        }

        /// <summary>
        /// Gestiona la fecha inicial, en la cual se debe filtrar
        /// </summary>
        public DateTime fechaInicial
        {
            get { return fechaInc; }
            set { fechaInc = value; OnPropertyChanged("fechaInicial"); }
        }

        /// <summary>
        /// Gestiona la fecha final, en la cual se debe filtrar
        /// </summary>
        public DateTime fechaFinal
        {
            get { return fechaFin; }
            set { fechaFin = value; OnPropertyChanged("fechaFinal"); }
        }

        /// <summary>
        /// Gestor del filtro combinado del control de usuario de busqueda de averia
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Devuelve false, si criteriosAveria es igual a cualquier numero que no sea 0,
        /// si se cumple los criterios, devuelve true </returns>
        private bool filtroCombinado(object item)
        {
            bool esta = false;
            if (item != null)
            {
                averia a = (averia)item;
                if(criteriosAveria.Count != 0)
                {
                    if(criteriosAveria.TrueForAll(x => x(a)))
                    {
                        esta = true;
                    }
                }
            }
            return esta;
        }

        /// <summary>
        /// Gestiona la averia Seleccionada
        /// </summary>
        public averia averiaNueva
        {
            get
            {
                return avNueva;
            }
            set
            {
                avNueva = value;                
                OnPropertyChanged("averiaNueva");
            }
        }

        /// <summary>
        /// Gestiona la pieza seleccionada
        /// </summary>
        public pieza piezaSeleccionada
        {
            get
            {
                return pzaSel;
            }
            set
            {
                pzaSel = value;               
                OnPropertyChanged("piezaSeleccionada");
            }
        }

        /// <summary>
        /// Gestiona la anulacion de la averia
        /// </summary>
        /// <returns>Devuelve true si todo a ido bien, y no a saltado ninguna excepcion,
        /// si hay alguna excepcion devuelve false</returns>
        public Boolean anula()
        {
            bool correcto = true;
            try
            {
                avServ.delete(averiaNueva);
                avServ.save();
            }catch(DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al anular una averia", dbex);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha habido una excepcion al anular","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                logger.Error("Ha habido un problema inesperado al anular la averia de la base de datos",ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Gestiona el boton de guardar averia, guarda la averia y si editar es true, la edita
        /// </summary>
        /// <returns>Devuelve true si no ha habido alguna excepcion,
        /// si hay alguna excepcion devuelve false</returns>
        public Boolean guarda()
        {
            bool correcto = true;

            try
            {
                if (editar)
                {                    
                    avServ.edit(averiaNueva);
                }
                else
                {
                    averiaNueva.CodigoAveria = avServ.getLastId() + 1;
                    avServ.add(averiaNueva);                    
                }
                avServ.save();
            }

            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al actualizar o al agregar una Averia", dbex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha habido una excepcion al guardar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Error("Ha habido un problema inesperado al guardar la averia de la base de datos", ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Gestiona la modificacion de la averia pasada, como parametro
        /// </summary>
        /// <param name="averia">averia a modificar</param>
        /// <returns>devuelve true si se ha modificado correctamente, devuelve false si ha habido algun problema</returns>
        public Boolean modificaAveria(averia averia)
        {
            bool correcto = true;
            try
            {               
                avServ.edit(averia);
                avServ.save();
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al actualizar una Averia", dbex);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha habido una excepcion al actualizar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.Error("Ha habido un problema inesperado al actualizar la averia de la base de datos", ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Devuelve una lista de averias, que hay en la base de datos
        /// </summary>
        public List<averia> listaAverias
        {
            get { try { return avServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido una excepcion al obtener las averias", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un problema inesperado al obtener las averias de la base de datos", ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve una lista de empleados, que hay en la base de datos
        /// </summary>
        public List<empleado> listaEmpleados
        {
            get { try { return empServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido una excepcion al obtener los empleados", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un problema inesperado al obtener los empleados de la base de datos", ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve una lista de clientes, que hay en la base de datos
        /// </summary>
        public List<cliente> listaClientes
        { get { try { return clientServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido una excepcion al obtener los clientes", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un problema inesperado al obtener los clientes de la base de datos", ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve una lista de piezas, que hay en la base de datos
        /// </summary>
        public List<pieza> listaPiezas
        {
            get { try { return pzaServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido una excepcion al obtener las piezas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un problema inesperado al obtener las piezas de la base de datos", ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve un listado de todas las averias
        /// que no estan en estado "Devuelto"
        /// </summary>
        public List<averia> listaAveriasNoDevueltas
        {
            get {
                try {
                    List<averia> averias = new List<averia>();
                     averias.AddRange(avServ.getAveriasPorEstado("En proceso"));
                    averias.AddRange(avServ.getAveriasPorEstado("En espera"));
                    averias.AddRange(avServ.getAveriasPorEstado("Finalizado"));
                    averias.AddRange(avServ.getAveriasPorEstado("Pendiente"));
                    List<averia> averiasOrdenadas = averias.OrderBy(o => o.CodigoAveria).ToList();
                    return averiasOrdenadas;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha habido una excepcion al obtener las piezas", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un problema inesperado al obtener las piezas de la base de datos", ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// Devuelve una lista de estados, menos el estado "Devuelto"
        /// </summary>
        public List<string> listaEstados
        {
            get { return estados; }
        }
        /// <summary>
        /// Devuelve una lista de estados, incluido el estado "Devuelto"
        /// </summary>
        public List<string> listaEstadosNoDevuelto
        {
            get { return estadosNoDevueltos; }
        }
    }
}
