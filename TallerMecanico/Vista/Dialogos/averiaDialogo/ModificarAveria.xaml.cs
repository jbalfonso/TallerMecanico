using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para ModiciarAveria.xaml
    /// </summary>
    public partial class ModificarAveria :MetroWindow
    {
        
        private MVAveria mvaveria;
        private Logger logger;
        private bool seleccionado = false;
        private averia averiaModificar;
        private Action<averia> gestionaStockAveriaModificada;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>        
        /// <param name="mvaveria">Clase de gestion de las averias</param>
        /// <param name="gestionaStockAveriaModificada">Metodo de la clase principal MainWindow, gestiona las notificaciones de las piezas</param>
        public ModificarAveria(MVAveria mvaveria,Action<averia> gestionaStockAveriaModificada)
        {
            InitializeComponent();            
            this.mvaveria = mvaveria;
            this.gestionaStockAveriaModificada = gestionaStockAveriaModificada;
            inicializar();
            
        }

        /// <summary>
        /// Inicializa los componentes ed la aplicaion
        /// </summary>
        private void inicializar()
        {            
            logger = LogManager.GetCurrentClassLogger();
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvaveria.OnErrorEvent));
            DataContext = mvaveria;
            mvaveria.btnGuardar = guardar;          
            DateResolucion.IsEnabled = false;
            txtResolucion.IsEnabled = false;
            averiaModificar = new averia();
        }

        /// <summary>
        /// Gestor del botond de guardar, 
        /// valida si se a seleccionado una averia, 
        /// valida el dialogo, y modifica la averia, 
        /// tambien modifica el stock dependiendo de si se añaden o se quitan piezas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (seleccionado)
            {
                if (recogeDatos())
                {
                    if (mvaveria.modificaStock(averiaModificar))
                    {
                     
                        if (mvaveria.modificaAveria(averiaModificar))
                        {
                            logger.Info("Averia nueva creada con codigo: " + averiaModificar.CodigoAveria);
                            gestionaStockAveriaModificada(averiaModificar);
                            this.DialogResult = true;
                        }
                        else
                        {
                            await this.ShowMessageAsync("Error", "Ha habido un error al insertar la averia en la base de datos");
                            logger.Error("Ha habido un error al modificar la averia en la base de datos");
                            this.DialogResult = false;
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error","Ha habido un error al modificar el stock de las piezas que contiene la averia");
                    }
                }                
            }
            else
            {
                MessageDialogResult result = await this.ShowMessageAsync("Informacion", "Tiene que elegir el codigo de la averia a modificar, si no desea modificar una averia haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Negative)
                {
                    this.Close();
                }
            }                      
        }

        /// <summary>
        /// Metodo que recoge todos los datos necesarios del formulario
        /// en caso de que haya algun problema, mediante un booleano
        /// este devuelve false
        /// </summary>
        /// <returns>Devuelve true si todo es correcto, devuelve false si ha habido algun problema</returns>
        private bool recogeDatos()
        {
            bool correcto = true;
            try
            {
                averiaModificar.Descripcion = Descripcion.Text;
                averiaModificar.Tipo = Tipo.Text;

                averiaModificar.empleado = (empleado)ComboEmpleado.SelectedItem;

                averiaModificar.FechaRecepcion = (DateTime)datePickerRecepcion.SelectedDate;

                averiaModificar.cliente1 = (cliente)Cliente.SelectedItem;
                if (DateResolucion.SelectedDate != null)
                {
                    averiaModificar.FechaResolucion = (DateTime)DateResolucion.SelectedDate;
                }
                else
                {
                    averiaModificar.FechaResolucion = null;
                }
                averiaModificar.Resolucion = txtResolucion.Text;
                if (!string.IsNullOrEmpty(precio.Text))
                {
                    if (precio.Text.Contains(","))
                    {
                        string precioFormateado = precio.Text;
                        precioFormateado.Replace(",", ".");
                        precio.Text = precioFormateado;

                        averiaModificar.Precio = Math.Round(double.Parse(precio.Text),2);
                    }
                    else
                    {
                        averiaModificar.Precio = Math.Round(double.Parse(precio.Text, CultureInfo.InvariantCulture),2);
                    }                    
                }
                else
                {
                    averiaModificar.Precio = null;
                }

                averiaModificar.Observaciones = Observaciones.Text;
            }
            catch (FormatException ex)
            {
                this.ShowMessageAsync("Error","Ha habido un error de conversion, con el precio de la averia, compruebe, que no contenga espacios");
                correcto = false;
            }
            catch (Exception ex)
            {
                this.ShowMessageAsync("Error","Ha habido un error fatal al obtener los datos del formulario");
                logger.Error("Ha habido un error al obtener los datos del formulario al modificar una averia",ex);
                correcto = false;
            }
            return correcto;
        }

        /// <summary>
        /// Gestor del boton de cancelar, cierra el dialogo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

        }

        /// <summary>
        /// Gestiona la seleccion de la averia a modificar, 
        /// pone un booleano a true, para comprobar si se ha seleccionado una averia, 
        /// guarda las piezas de la averia en una variable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboCodigo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            seleccionado = true;
            averia av = (averia)comboDescripcion.SelectedItem;
            averiaModificar = av;
            mvaveria.piezasModificaciones = averiaModificar.pieza.ToList();
            trasladaDatos();
            
            
        }

        /// <summary>
        /// Se encarga de trasladar los datos del objeto averia al formulario
        /// </summary>
        private void trasladaDatos()
        {
            Descripcion.Text = averiaModificar.Descripcion;
            Tipo.Text = averiaModificar.Tipo;
            if (averiaModificar.Estado != null)
            {
                comboEstado.SelectedItem = averiaModificar.Estado;
            }
            if (averiaModificar.empleado != null)
            {
                ComboEmpleado.SelectedItem = averiaModificar.empleado;
            }

            if (averiaModificar.FechaRecepcion != null)
            {
                datePickerRecepcion.SelectedDate = averiaModificar.FechaRecepcion;
                datePickerRecepcion.DisplayDate = averiaModificar.FechaRecepcion;
            }

            if (averiaModificar.cliente1 != null)
            {
                Cliente.SelectedItem = averiaModificar.cliente1;
            }
            
            if (averiaModificar.FechaResolucion != null)
            {
                DateResolucion.SelectedDate = averiaModificar.FechaResolucion;
                DateResolucion.DisplayDate = (DateTime)averiaModificar.FechaResolucion;
            }

            txtResolucion.Text = averiaModificar.Resolucion;
            precio.Text = averiaModificar.Precio + "";

            if (averiaModificar.pieza != null && averiaModificar.pieza.Count > 0)
            {
                piezasAveriaCombo.ItemsSource = averiaModificar.pieza;
            }
            Observaciones.Text = averiaModificar.Observaciones;
        }
        
        /// <summary>
        /// Gestiona el boton de añadir una pieza, añade la pieza a la averia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void anyadirPieza_Click(object sender, RoutedEventArgs e)
        {          
            averiaModificar.pieza.Add((pieza)piezaComboBox.SelectedItem);
            piezasAveriaCombo.ItemsSource = averiaModificar.pieza;
            piezasAveriaCombo.Items.Refresh();
        }


        /// <summary>
        /// Gestiona el boton de eliminar una pieza,
        /// elimina una pieza de la averia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removePieza_Click(object sender, RoutedEventArgs e)
        {                      
            averiaModificar.pieza.Remove((pieza)piezasAveriaCombo.SelectedItem);
            piezasAveriaCombo.ItemsSource = averiaModificar.pieza;
            piezasAveriaCombo.Items.Refresh();
        }

        /// <summary>
        /// Comprueba el estado seleccionado y 
        /// dependiendo del estado oculta o muestra elementos de la aplicacion
        /// </summary>
        private void compruebaEstado()
        {            
            if (averiaModificar.Estado=="En espera" || averiaModificar.Estado == "En proceso" || averiaModificar.Estado == "Pendiente")
            {
                DateResolucion.IsEnabled = false;
                txtResolucion.IsEnabled = false;
            }
            if (averiaModificar.Estado == "Finalizado" || averiaModificar.Estado == "Devuelto")
            {
                DateResolucion.IsEnabled = true;
                txtResolucion.IsEnabled = true;
            }
            
        }

        /// <summary>
        /// Gestiona el cambio de estado del combo de estado,
        /// llama a comprueba estado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            averiaModificar.Estado = (string)comboEstado.SelectedItem;
            compruebaEstado();            
        }

        /// <summary>
        /// Gestiona el tipo de texto introducido en el textbox, hace que solo se introduzcan numeros decimales
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>     
        private void FiltroTextoPrecio(object sender, KeyEventArgs e)
        {   
            //Solo permite numeros y "."
            e.Handled = !(((e.Key.GetHashCode() >= 34 && e.Key.GetHashCode() <= 43)||( e.Key.GetHashCode()>=75 && e.Key.GetHashCode()<=83 )));

            // comprueba si el "." esta en el textbox
            if (e.Key.GetHashCode() == 144 || e.Key.GetHashCode()==88)
                e.Handled = (sender as TextBox).Text.Contains(".");
        }
    }
}
