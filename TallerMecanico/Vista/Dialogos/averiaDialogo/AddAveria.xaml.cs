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


namespace TallerMecanico.Vista.Dialogos.averiaDialogo
{
    /// <summary>
    /// Lógica de interacción para AddAveria.xaml
    /// </summary>
    public partial class AddAveria : MetroWindow
    {
        Logger logger;
       
        private MVAveria mvaveria;        
        
        /// <summary>
        /// Constructor del dialogo
        /// </summary>        
        /// <param name="mvaveria">Clase de gestion de las averias</param>
        public AddAveria(MVAveria mvaveria)
        {
            InitializeComponent();           
            this.mvaveria = mvaveria;
            
            logger = LogManager.GetCurrentClassLogger();           
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvaveria.OnErrorEvent));
            DataContext = mvaveria;
            mvaveria.btnGuardar = guardar;
            inicializar();
        }

        /// <summary>
        /// Inicializa los componentes del dialogo
        /// </summary>
        private void inicializar()
        {
            mvaveria.averiaNueva.FechaRecepcion =System.DateTime.Now;
            txtResolucion.IsEnabled = false;
            DateResolucion.IsEnabled = false;
        }

        /// <summary>
        /// Gestor del boton de guardar, 
        /// realiza comprobaciones para que el formulario sea valido 
        /// y guarda la averia en la base de datos, despues reduce el stock de las piezas utilizadas, 
        /// si hay algun error en la validacion muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (datePickerRecepcion.SelectedDate > DateResolucion.SelectedDate)
            {
                await this.ShowMessageAsync("Informacion","La fecha de recepcion no puede ser mayor que la de resolucion");
            }
            else
            {
            if (mvaveria.IsValid(this))
            {
                if (mvaveria.guarda())
                {
                    if (mvaveria.reduceStock())
                    {
                        logger.Info("Stock reducido");
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "Ha habido un problema al reducir el stock");
                    }
                    logger.Info("Averia nueva creada con codigo: " + mvaveria.averiaNueva.CodigoAveria);
                    this.DialogResult = true;
                }
                else
                {
                    await this.ShowMessageAsync("Error","Ha habido un error al insertar la averia en la base de datos");
                    this.DialogResult = false;
                }
            }
            else
            {
                await this.ShowMessageAsync("Informacion", "Rellene los campos requeridos");
            }
            }

        }
        /// <summary>
        /// Gestor del boton cancelar del dialogo, cierra el dialogo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        /// <summary>
        /// Gestor del boton de añadir una pieza a una averia,
        /// añade una pieza seleccionada a una averia seleccionada anteriormente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void anyadirPieza_Click(object sender, RoutedEventArgs e)
        {           
            mvaveria.averiaNueva.pieza.Add(mvaveria.piezaSeleccionada);            
            piezasAveriaCombo.Items.Refresh();
        }
        
        /// <summary>
        /// Gestor del boton de eliminar pieza, elimina una pieza seleccionada de la averia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removePieza_Click(object sender, RoutedEventArgs e)
        {
            mvaveria.averiaNueva.pieza.Remove(mvaveria.piezaSeleccionada);
            piezasAveriaCombo.Items.Refresh();
        }
        
        /// <summary>
        /// Gestor del cambio del elemento seleccionado del combobox de estado, 
        /// llama al metodo comprueba estado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            compruebaEstado();
        }
        /// <summary>
        /// Comprueba el estado seleccionado, 
        /// y dependiendo del estado elegido, 
        /// muestra o oculta elementos del dialogo
        /// </summary>
        private void compruebaEstado()
        {
            averia mvaveriaNueva = (averia)mvaveria.averiaNueva;
            if (mvaveriaNueva.Estado == "En espera" || mvaveriaNueva.Estado == "En proceso" || mvaveriaNueva.Estado == "Pendiente")
            {
                DateResolucion.IsEnabled = false;
                txtResolucion.IsEnabled = false;
            }
            if (mvaveriaNueva.Estado == "Finalizado" || mvaveriaNueva.Estado == "Devuelto")
            {
                DateResolucion.IsEnabled = true;
                txtResolucion.IsEnabled = true;
            }

        }

        /// <summary>
        /// Gestiona el texto introducido en el textbox, hace que solo se puedan introducir numeros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
