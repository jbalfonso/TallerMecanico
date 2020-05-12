using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TallerMecanico.Modelo;
using TallerMecanico.MVVM;

namespace TallerMecanico.Vista.Dialogos.clientesDialogo
{
    /// <summary>
    /// Lógica de interacción para AddClientes.xaml
    /// </summary>
    public partial class AddClientes : MetroWindow
    {
       
        private MVCliente mvcliente;
        private Logger logger;

        /// <summary>
        /// Constructor de la clase dialogo
        /// </summary>
        /// <param name="mvcliente">Clase que gestiona los clientes</param>
        public AddClientes(MVCliente mvcliente)
        {
            InitializeComponent();          
            this.mvcliente = mvcliente;
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvcliente.OnErrorEvent));
            DataContext = mvcliente;
            mvcliente.btnGuardar = guardar;
            inicializar();
        }

        /// <summary>
        /// Inicializa los componentes de la aplicacion
        /// </summary>
        private void inicializar()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Gestiona el boton de guardar, comprueba, si el dialogo es valido y guarda el cliente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (mvcliente.IsValid(this))
            {
                if (mvcliente.guarda())
                {
                    logger.Info("Cliente añadido con codigo: " + mvcliente.clienteNuevo.CodigoCliente);
                    this.DialogResult = true;
                }
                else
                {
                    logger.Error("Ha habido un error en la base de datos al añadir un Cliente");
                    await this.ShowMessageAsync("ERROR","Ha habido un error inesperado al guardar el cliente en la base de datos");
                    this.DialogResult = false;
                }
            }
            else
            {
                await this.ShowMessageAsync("Informacion", "Rellene todos los campos requeridos");
            }
        }

        /// <summary>
        /// Gestor del boton de cancelar, cierra la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
    }
