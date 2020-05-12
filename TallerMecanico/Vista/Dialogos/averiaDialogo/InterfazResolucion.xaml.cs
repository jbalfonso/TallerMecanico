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
    /// Lógica de interacción para InterfazResolucion.xaml
    /// </summary>
    public partial class InterfazResolucion : MetroWindow
    {
        private MVAveria mvaveria;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private bool averiaSeleccionada = false;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvaveria">Clase gestor de las averias</param>
        public InterfazResolucion(MVAveria mvaveria)
        {
            InitializeComponent();
            this.mvaveria = mvaveria;
            inicializar();
        }
        /// <summary>
        /// Inicializa componentes de la aplicacion, y la validacion de los datos
        /// </summary>
        private void inicializar()
        {
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvaveria.OnErrorEvent));
            DataContext = mvaveria;
            mvaveria.btnGuardar = guardar;
            mvaveria.averiaNueva.FechaResolucion = DateTime.Now;
        }

        /// <summary>
        /// Gestor del boton de guardar, edita la averia,
        /// para añadirle la resolucion de esta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (averiaSeleccionada)
            {
                mvaveria.editar = true;
                if (mvaveria.guarda())
                {
                    this.DialogResult = true;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "Ha habido un error al insertar la resolucion en la base de datos");
                    logger.Error("Ha habido un error al guardar la resolucion de la averia");
                }
            }
            else
            {
                await this.ShowMessageAsync("Informacion", "Debe seleccionar una averia para poder continuar");
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

        /// <summary>
        /// Gestiona el cambio de seleccion de la averia seleccionada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ComboAveria_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mvaveria.averiaNueva.Estado != "Devuelto")
            {               
                averiaSeleccionada = true;
                gestorValidacion();
            }
            else
            {
                averiaSeleccionada = false;
                gestorValidacion();
                await this.ShowMessageAsync("Informacion", "La averia seleccionada, ya ha sido devuelta," +
                    System.Environment.NewLine + " por lo tanto no se puede añadir una resolucion a una averia que ya la tiene");
            }


        }

        /// <summary>
        /// Gestiona la validacion de los elementos de la aplicacion,
        /// ocultando o haciendo visibles los elementos de validacion
        /// </summary>
        private void gestorValidacion()
        {
            if (!string.IsNullOrEmpty(precioAveria.Text))
            {
                validacionPrecio.Visibility = Visibility.Collapsed;
            }
            else
            {
                validacionPrecio.Visibility = Visibility.Visible;
            }
            if (!string.IsNullOrEmpty(txtResolucion.Text))
            {
                validaciontxtResolucion.Visibility = Visibility.Collapsed;
            }
            else
            {
                validaciontxtResolucion.Visibility = Visibility.Visible;
            }
            if (DateResolucion.SelectedDate <= DateTime.Now)
            {
                validacionFechaResolucion.Visibility = Visibility.Collapsed;
            }
            else
            {
                validacionFechaResolucion.Visibility = Visibility.Visible;
            }
            if (averiaSeleccionada && !string.IsNullOrEmpty(txtResolucion.Text) && !string.IsNullOrEmpty(precioAveria.Text) && DateResolucion.SelectedDate <= DateTime.Now)
            {
                guardar.IsEnabled = true;
            }
            else
            {
                guardar.IsEnabled = false;
            }

        }

        /// <summary>
        /// Gestiona el cambio de seleccion de la fecha de resolucion, 
        /// llama al metodo gestorValidacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DateResolucion_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            gestorValidacion();
        }
        /// <summary>
        /// Gestiona el cambio de seleccion del texto de resolucion,
        /// llama al metodo gestorValidacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtResolucion_TextChanged(object sender, TextChangedEventArgs e)
        {
            gestorValidacion();
        }

        /// <summary>
        /// Gestiona el texto introducido en el textbox, hace que solo se puedan introducir numeros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrecioAveria_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
