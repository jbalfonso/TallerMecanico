using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private averia averiaModificar;

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
            averiaModificar = new averia();
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
                if (recogeDatos())
                {
                    if (mvaveria.modificaAveria(averiaModificar))
                    {
                        logger.Info("Resolucion añadida a la averia con codigo: " + averiaModificar.CodigoAveria);
                        this.DialogResult = true;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "Ha habido un error al insertar la resolucion en la base de datos");
                        logger.Error("Ha habido un error al guardar la resolucion de la averia");
                    }
                }
            }
            else
            {
                await this.ShowMessageAsync("Informacion", "Debe seleccionar una averia para poder continuar");
            }
        }

        /// <summary>
        /// Metodo que se encarga de recoger todos los datos del formulario,
        /// mediante un booleano en caso de error este devuelve false
        /// </summary>
        /// <returns>si hay algun error devuelve false, si todo es correcto devuelve true</returns>
        private bool recogeDatos()
        {
            bool correcto = true;
            try
            {
                if (DateResolucion.SelectedDate != null)
                {
                    averiaModificar.FechaResolucion = (DateTime)DateResolucion.SelectedDate;
                }
                else
                {
                    averiaModificar.FechaResolucion = null;
                }

                averiaModificar.Resolucion = txtResolucion.Text;

                if (!string.IsNullOrEmpty(precioAveria.Text))
                {

                    if (precioAveria.Text.Contains(","))
                    {
                        string precioFormateado = precioAveria.Text;
                        precioFormateado.Replace(",", ".");
                        precioAveria.Text = precioFormateado;

                        averiaModificar.Precio = Math.Round(double.Parse(precioAveria.Text), 2);
                    }
                    else
                    {
                        averiaModificar.Precio = Math.Round(double.Parse(precioAveria.Text, CultureInfo.InvariantCulture), 2);
                    }
                }
                else
                {
                    averiaModificar.Precio = null;
                }
                averiaModificar.Estado = "Finalizado";

            }
            catch (FormatException)
            {
                this.ShowMessageAsync("Error", "Ha habido un problema al formatear el precio, comprueba que el precio no contenga caracteres en blanco ni espacios");
                correcto = false;
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Error", "Se ha producido un error al obtener los datos del formulario para modificar la averia");
                logger.Error("Se ha producido un error desconocido al obtener los datos del formulario para modificar la averia", ex);
                correcto = false;
            }
            return correcto;
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
                averiaModificar = (averia)comboAveria.SelectedItem;
                Descripcion.Text = averiaModificar.Descripcion;
                comboEstado.SelectedItem = averiaModificar.Estado;
                datePickerRecepcion.DisplayDate = averiaModificar.FechaRecepcion;
                datePickerRecepcion.SelectedDate = averiaModificar.FechaRecepcion;

                if (averiaModificar.FechaResolucion != null)
                {
                    DateResolucion.SelectedDate = averiaModificar.FechaResolucion;
                }
                DateResolucion.DisplayDate = DateTime.Now;
                txtResolucion.Text = averiaModificar.Resolucion;
                precioAveria.Text = averiaModificar.Precio + "";
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
        /// Gestiona el texto introducido en el textbox, hace que solo se puedan introducir numeros decimales
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param> 
        private void FiltroTextoPrecio(object sender, KeyEventArgs e)
        {
            // solo permite 0-9 y "."
            e.Handled = !(((e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43) || (e.Key.GetHashCode() >= 75 && e.Key.GetHashCode() <= 83)));

            // comprueba si "." existe en el textbox
            if (e.Key.GetHashCode() == 144 || e.Key.GetHashCode() == 88)
                e.Handled = (sender as TextBox).Text.Contains(".");
        }
    }
}
