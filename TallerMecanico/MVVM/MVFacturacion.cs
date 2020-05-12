using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TallerMecanico.Modelo;
using TallerMecanico.Servicios;
using TallerMecanico.MVVM;
using System.Data.Entity.Infrastructure;
using System.Windows;

namespace TallerMecanico.MVVM
{
    /// <summary>
    /// Clase de Gestion MVVM la cual gestiona lo relativo a la facturacion
    /// </summary>
    public class MVFacturacion:MVBase
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private tallermecanicoEntities tEnt;
        private EmpleadoServicio empServ;
        private AveriaServicio avServ;
        private ClienteServicio clientServ;
        private PiezaServicio pzaServ;       

        private List<averia> averiasSel = new List<averia>();

        /// <summary>
        /// Gestiona el listado de piezas, obtiene o establece el listado
        /// </summary>
        public List<pieza> piezas { get; set; }

        private averia avSel;
        private pieza pzaSel;
        private empleado empSel;
        private cliente clntSel;

        private DateTime fechaFactura;

        private Tarjeta tarjeta;
        private Paypal paypal;

        private const int tarjetaMetodo=1;
        private const int efectivoMetodo = 2;
        private const int paypalMetodo = 3;

        /// <summary>
        /// Gestiona el metodo de pago utilizado en el cobro o en la devolucion
        /// </summary>
        public int metodoPago { get; set; }
        
        /// <summary>
        /// Contructor de la clase MVFacturacion
        /// </summary>
        /// <param name="ent">entidad de la base de datos taller mecanico</param>
        public MVFacturacion(tallermecanicoEntities ent)
        {
            this.tEnt = ent;
            inicializar(); 
        }

        /// <summary>
        /// Inicializa las variables de la clase
        /// </summary>
        private void inicializar()
        {
            tarjeta = new Tarjeta("", "", "");
            paypal = new Paypal("","");
            avSel = new averia();
            pzaSel = new pieza();
            empSel = new empleado();
            clntSel = new cliente();
            fechaFactura = DateTime.Now;

            clientServ = new ClienteServicio(tEnt);
            avServ = new AveriaServicio(tEnt);
            empServ = new EmpleadoServicio(tEnt);
            pzaServ = new PiezaServicio(tEnt);


        }

        /// <summary>
        /// Gestiona la tarjeta de credito seleccionada
        /// </summary>
        public Tarjeta tarjetaSeleccionada
        {
            get
            {
                return tarjeta;
            }
            set
            {
                tarjeta = value;
                OnPropertyChanged("tarjetaSeleccionada");
            }
        }

        /// <summary>
        /// Gestiona la cuenta de paypal seleccionada
        /// </summary>
        public Paypal paypalSeleccionado
        {
            get
            {
                return paypal;
            }
            set
            {
                paypal = value;
                OnPropertyChanged("paypalSeleccionado");
            }
        }

        /// <summary>
        /// Gestiona el cliente seleccionado
        /// </summary>
        public cliente clienteSeleccionado
        {
            get
            {
                return clntSel;
            }
            set
            {
                clntSel = value;
                OnPropertyChanged("clienteSeleccionado");
            }
        }

        /// <summary>
        /// Gestiona el empleado Seleccionado
        /// </summary>
        public empleado empleadoSeleccionado
        {
            get
            {
                return empSel;
            }
            set
            {
                empSel = value;
                OnPropertyChanged("empleadoSeleccionado");
            }
        }

        /// <summary>
        /// Gestiona la averia Seleccionada
        /// </summary>
        public averia averiaSeleccionada
        {
            get { return avSel; }
            set { avSel = value; OnPropertyChanged("averiaSeleccionada"); }
        }

        /// <summary>
        /// Gestiona el listado de averias Seleccionadas
        /// </summary>
        public List<averia> averiaSeleccionadas
        {
            get { return averiasSel; }
            set { averiasSel = value; OnPropertyChanged("averiaSeleccionadas"); }
        }
        
        /// <summary>
        /// Gestiona la fecha de cobro seleccionada
        /// </summary>
        public DateTime fechaSeleccionada
        {
            get { return fechaFactura; }
            set { fechaFactura = value; OnPropertyChanged("fechaSeleccionada"); }
        }

        /// <summary>
        /// Gestiona la devolucion de la averia, devuelve la averia,
        /// y aumenta el stock de las piezas devueltas
        /// </summary>
        /// <returns> Devuelve true si no ha habido ninguna excepcion,
        /// si hay alguna excepcion devuelve false</returns>
        public Boolean abonaAveria()
        {
            bool correcto = true;
            try
            {
                if (averiaSeleccionada.Estado == "Devuelto")
                {
                    correcto = false;
                }
                else
                {
                    averiaSeleccionada.Estado = "Devuelto";
                    piezas = averiaSeleccionada.pieza.ToList();

                    avServ.edit(averiaSeleccionada);
                    avServ.save();
                    foreach (pieza pza in piezas)
                    {
                        pza.Cantidad = pza.Cantidad + 1;
                        pzaServ.edit(pza);
                        pzaServ.save();
                    }
                }
            }
            catch (DbUpdateException dbex)
            {
                correcto = false;
                logger.Error("Ha habido un problema al actualizar y cambiar el estado de la averia a devuelto, de la averia: "+averiaSeleccionada.CodigoAveria,dbex);
            }
            catch (Exception ex)
            {
                logger.Error("Ha habido error inesperado al facturar la averia en la base de datos", ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Devuelve un listado de clientes que hay en la base de datos
        /// </summary>
        public List<cliente> listaClientes
        {
            get { try { return clientServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido un error inesperado al obtener los clientes","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    logger.Error("Ha habido un error inesperado al obtener los clientes de la base de datos",ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve un listado de empleados que hay en la base de datos
        /// </summary>
        public List<empleado> listaEmpleados
        {
            get { try { return empServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido un error inesperado al obtener los empleados", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un error inesperado al obtener los empleados de la base de datos", ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve un listado de averias que hay en la base de datos
        /// </summary>
        public List<averia> listaAverias
        {
            get { try { return avServ.getAll().ToList(); } catch (Exception ex) {
                    MessageBox.Show("Ha habido un error inesperado al obtener las averias", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un error inesperado al obtener las averias de la base de datos", ex);
                    return null;
                } }
        }

        /// <summary>
        /// Devuelve un listado de averias que hay en la base de datos,/ 
        /// las cuales el estado este finalizado
        /// </summary>
        public List<averia> listaAveriasFinalizadas
        {
            get
            {
                try { return avServ.getAveriasPorEstado("Finalizado"); }
                catch (Exception ex)
                {
                    MessageBox.Show("Ha habido un error inesperado al obtener las averias por el estado ''Finalizado''", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.Error("Ha habido un error inesperado al obtener las averias con estado ''Finalizado'' de la base de datos", ex);
                    return null;
                }
            }
        }

    }
}
