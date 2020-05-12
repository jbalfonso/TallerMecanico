using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CreditCardValidator;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using TallerMecanico.Vista.ControlesUsuario;
using System.Text.RegularExpressions;

namespace TallerMecanico.Vista.Dialogos.clientesDialogo
{
    /// <summary>
    /// Lógica de interacción para InterfazCobro.xaml
    /// </summary>
    public partial class InterfazCobro : MetroWindow
    {
     
        private MVFacturacion mvfactura;
        private Logger logger = LogManager.GetCurrentClassLogger();

        private bool clienteSeleccionado = false;
        private bool controladorCliente = false;
        private bool empleadoSeleccionado = false;
        private bool averiaSeleccionada = false;

        private const int tarjeta = 1;
        private const int efectivo = 2;
        private const int paypal = 3;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvfactura">Clase que gestiona la factura del cliente</param>
        public InterfazCobro(MVFacturacion mvfactura)
        {
            InitializeComponent();            
            this.mvfactura = mvfactura;
            DataContext = mvfactura;
            inicializar();
        }

        /// <summary>
        /// Inicializa componentes de la aplicacion
        /// </summary>
        private void inicializar()
        {
            radioEfectivo.IsEnabled = false;
            radioPaypal.IsEnabled = false;
            radioTarjeta.IsEnabled = false;
        }

        /// <summary>
        /// Gestiona el boton de cobrar, 
        /// pasa los datos seleccionados en el dialogo, 
        /// a la clase gestora mvfactura, dependiendo del metodo de pago,
        /// gestiona la validacion de la tarjeta de credito o de la cuenta de paypal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Cobrar_Click(object sender, RoutedEventArgs e)
        {
            DateTime fechaFactura = (DateTime)fechaCobro.SelectedDate;
            DateTime fechaActual = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);            
            
            if (fechaFactura >= fechaActual)
            {
                if (mvfactura.metodoPago == tarjeta)
                {
                    manipularTarjeta();
                }
                else if (mvfactura.metodoPago == efectivo)
                {
                    this.DialogResult = true;
                }
                else if (mvfactura.metodoPago == paypal)
                {
                    manipularPaypal();
                }
                else
                {
                    await this.ShowMessageAsync("Informacion", "Debe seleccionar un metodo de pago");
                }
            }
            else
            {             
                await this.ShowMessageAsync("Informacion", "La fecha de facturacion no puede ser menor que la fecha actual");
            }    
        }
               
        /// <summary>
        /// Gestiona la validacion de la tarjeta de credito, 
        /// y pasa los datos a la clase mvfactura
        /// </summary>
        private async void manipularTarjeta()
        {
            CreditCardDetector detector;

            try
            {                
                if (numeroTarjeta.IsMaskFull)
                {
                    detector = new CreditCardDetector(numeroTarjeta.Text);
                    if (detector.IsValid())
                    {                       
                        if (caducidadTarjeta.IsMaskFull)
                        {
                            if (ccvTarjeta.IsMaskFull)
                            {
                                string[] caducidadSeparada = caducidadTarjeta.Text.Split('/');
                                int mesCaducidad = int.Parse(caducidadSeparada[0]);
                                int anyoCaducidad = int.Parse(caducidadSeparada[1]);

                                int anyoActual = int.Parse(DateTime.Now.ToString("yy"));
                                int mesActual = int.Parse(DateTime.Now.ToString("MM"));

                                if (mesCaducidad > 12)
                                {
                                    await this.ShowMessageAsync("Informacion","El mes de caducidad es erroneo, no existe el mes: "+mesCaducidad );
                                }
                                else
                                {
                                    if (anyoCaducidad == anyoActual)
                                    {
                                        if (mesCaducidad <= mesActual)
                                        {
                                            await this.ShowMessageAsync("Informacion", "La operacion no se puede completar porque la tarjeta esta caducada, mes: " + mesCaducidad);
                                        }
                                        else
                                        {
                                            this.DialogResult = true;
                                        }
                                    }
                                    else if (anyoCaducidad < anyoActual)
                                    {
                                        await this.ShowMessageAsync("Informacion", "La operacion no se puede completar porque la tarjeta esta caducada, año: " + anyoCaducidad);
                                    }
                                    else
                                    {
                                        this.DialogResult = true;
                                    }
                                }                                
                            }
                            else
                            {
                                await this.ShowMessageAsync("Informacion","CCV Tarjeta incompleto");
                            }
                        }
                        else
                        {
                            await this.ShowMessageAsync("Informacion","Fecha caducidad incompleta");
                        }                       
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error","El numero de tarjeta: "+ numeroTarjeta.Text +" no es valido");
                    }
                }
                else
                {
                    await this.ShowMessageAsync("Informacion", "Numero Tarjeta incorrecto, compruebe su longitud, solo se acepta visa y mastercard");
                }
            }
            catch (ArgumentException ex)
            {
                logger.Error("Error en los argumentos de la tarjeta de credito en la interfazCobro del cliente", ex);
            }
            catch (Exception ex2)
            {
                logger.Error("Error excepcion desconocida en la interfaz de cobro", ex2);
            }
        }

        /// <summary>
        /// Gestiona la validacion de los datos introducidos de la cuenta de paypal,
        /// que el correo sea valido, y que la contraseña no esta vacia
        /// </summary>
        private async void manipularPaypal()
        {            
            if (!string.IsNullOrWhiteSpace(paypalCorreo.Text))
            {
                if (validarEmail(paypalCorreo.Text))
                {
                    if (!string.IsNullOrWhiteSpace(paypalContrasenya.Password))
                    {
                        this.DialogResult = true;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Informacion", "La contraseña de la cuenta de paypal no puede estar vacia");
                    }
                }
                else
                {
                    await this.ShowMessageAsync("Informacion", "A introducido una direccion de correo erronea");
                }
                
            }
            else
            {
                await this.ShowMessageAsync("Informacion","La direccion de correo no puede estar vacia");
            }
        }

        /// <summary>
        /// Valida si el correo proporcionado, es valido, segun unos criterios
        /// </summary>
        /// <param name="emailAddress">Direccion de correo en formato string</param>
        /// <returns>Devuelve true si el correo es valido, devuelve false si el correo no es valido</returns>
        private bool validarEmail(string emailAddress)
        {
            var regex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[com,es,net,org,info,biz,edu,gob,cat](?:[com,es,net,org,info,biz,edu,gob,cat]*[com,es,net,org,info,biz,edu,gob,cat])?)\Z";
            bool isValid = Regex.IsMatch(emailAddress, regex, RegexOptions.IgnoreCase);
            return isValid;
        }

        /// <summary>
        /// Gestiona el boton de cancelar, cierra la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Gestiona cuando se selecciona el radio button de la tarjeta de credito, 
        /// oculta o muestra elementos de la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioTarjeta_Checked(object sender, RoutedEventArgs e)
        {
            gridEfectivo.Visibility = Visibility.Collapsed;
            gridpaypal.Visibility = Visibility.Collapsed;
            gridTarjeta.Visibility = Visibility.Visible;
            mvfactura.metodoPago = 1;
        }

        /// <summary>
        /// Gestiona cuando se selecciona el radio button de la compra con efectivo, 
        /// oculta o muestra elementos de la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioEfectivo_Checked(object sender, RoutedEventArgs e)
        {
            gridEfectivo.Visibility = Visibility.Visible;
            gridpaypal.Visibility = Visibility.Collapsed;
            gridTarjeta.Visibility = Visibility.Collapsed;
            mvfactura.metodoPago = 2;
        }

        /// <summary>
        /// Gestiona cuando se selecciona el radio button de la cuenta de paypal
        /// oculta o muestra elementos de la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioPaypal_Checked(object sender, RoutedEventArgs e)
        {
            gridEfectivo.Visibility = Visibility.Collapsed;
            gridpaypal.Visibility = Visibility.Visible;
            gridTarjeta.Visibility = Visibility.Collapsed;
            mvfactura.metodoPago = 3;
        }

        /// <summary>
        /// Gestiona el cambio de seleccion del cliente a facturar, 
        /// oculta y muestra elementos de validacion del dialogo, 
        /// y reinicia el textbox del precio y el de las piezas, 
        /// llama al metodo gestion del pago
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClienteCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clienteSeleccionado = true;
            
            validacionAveria.Visibility = Visibility.Collapsed;
            List<averia> averiasCliente = new List<averia>();
            foreach  (averia av in mvfactura.clienteSeleccionado.averia)
            {
                if (av.Estado == "Finalizado")
                {
                    averiasCliente.Add(av);
                }
            }
            averiaCombo.ItemsSource = averiasCliente;
            averiaCombo.Items.Refresh();
            
            if (controladorCliente)
            {
                controladorCliente = false;
                piezasCombo.ItemsSource = null;
                piezasCombo.Items.Clear();
                piezasCombo.Items.Refresh();
                preciotxt.Text = "0€";
                
                validacionPrecio.Visibility = Visibility.Visible;
                validacionPiezas.Visibility = Visibility.Visible;
                averiaSeleccionada = false;
                
            }
            controladorCliente = true;
            
            gestorPago();
        }
        
        /// <summary>
        /// Gestiona el cierre de la averia, 
        /// añade el precio segun la averia seleccionada, 
        /// y las piezas que tiene esa averia, 
        /// tambien oculta o muestra elementos de validacion del dialogo,
        /// llama al metodo gestor pago
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AveriaCombo_Closed(object sender, RoutedEventArgs e)
        {
            try {                 
            averiaSeleccionada = true;
            double precio = 0.0;
            List<pieza> piezasTotales = new List<pieza>();
            List<List<pieza>> piezasPorAveria = new List<List<pieza>>();

            foreach (averia av in mvfactura.averiaSeleccionadas)
            {
                    if (av.Precio != null)
                    {
                        precio += (double)av.Precio;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error","La averia debe de tener un precio para poder ser cobrada, modifique la averia o añada la resolucion");
                        averiaSeleccionada = false;
                    }
                    piezasPorAveria.Add(av.pieza.ToList());
                }
            preciotxt.Text = precio + "€";

            foreach (List<pieza> lista in piezasPorAveria)
            {
                foreach (pieza pz in lista)
                { piezasTotales.Add(pz);}
            }

            mvfactura.piezas = piezasTotales;
            piezasCombo.ItemsSource = piezasTotales;
            piezasCombo.Items.Refresh();
            
            if (averiaCombo.SelectedItems.Count > 0)
            {
                validacionPrecio.Visibility = Visibility.Collapsed;

                if (averiaCombo.SelectedItems.Count == 1)
                {
                    Cobrar.IsEnabled = true;
                }
                else
                {
                    Cobrar.IsEnabled = false;
                    await this.ShowMessageAsync("Informacion", "No se permite seleccionar mas de 1 averia");
                }

            }
            else
            {
                
                validacionPrecio.Visibility = Visibility.Visible;
                averiaSeleccionada = false;
            }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error","Ha habido un error al seleccionar una averia");
                logger.Error("Ha habido un error al seleccionar una averia en la interfaz de cobro, comprobar que el precio de la averia no sea null",ex);
            }
            gestorPago();
        }

      /// <summary>
      /// Gestiona la habilitacion o deshabilitacion 
      /// de los radio button del metodo de pago 
      /// y de los grid de dichos metodos de pago
      /// </summary>
        private void gestorPago()
        {
            if(empleadoSeleccionado && clienteSeleccionado && averiaSeleccionada)
            {
                radioEfectivo.IsEnabled = true;
                radioPaypal.IsEnabled = true;
                radioTarjeta.IsEnabled = true;                
            }
            else
            {
                radioEfectivo.IsEnabled = false;
                radioPaypal.IsEnabled = false;
                radioTarjeta.IsEnabled = false;
                gridTarjeta.Visibility = Visibility.Collapsed;
                gridEfectivo.Visibility = Visibility.Collapsed;
                gridpaypal.Visibility = Visibility.Collapsed;
                radioEfectivo.IsChecked = false;
                radioPaypal.IsChecked = false;
                radioTarjeta.IsChecked = false;
            }
        }

        /// <summary>
        /// Gestiona el cambio de seleccion del empleado, 
        /// poniendo un boleano a true para informar de que se ha seleccionado un empleado,
        /// y oculta los elementos de validacion del empleado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmpleadoCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            empleadoSeleccionado = true;
            validacionEmpleado.Visibility = Visibility.Collapsed;
            gestorPago();
        }


        /// <summary>
        /// Gestiona cuando se abre la averia, 
        /// oculta el mensaje de la validacion de las piezas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AveriaCombo_Opened(object sender, RoutedEventArgs e)
        {
            validacionPiezas.Visibility = Visibility.Collapsed;
            
        }
    }
}
