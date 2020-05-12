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

namespace TallerMecanico.Vista.Dialogos.empleadoDialogo
{
    /// <summary>
    /// Lógica de interacción para BorrarEmpleado.xaml
    /// </summary>
    public partial class BorrarEmpleado : MetroWindow
    {       
        private MVEmpleado mvempleado;
        private Logger logger;
        private bool seleccionado = false;
        private string login;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvempleado">Clase que gestiona a los empleados</param>
        public BorrarEmpleado(MVEmpleado mvempleado)
        {
            InitializeComponent();            
            this.mvempleado = mvempleado;
            logger = LogManager.GetCurrentClassLogger();
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvempleado.OnErrorEvent));
            DataContext = mvempleado;
            mvempleado.btnGuardar = borrar;
            muestraRequisitoContrasena();
        }

        /// <summary>
        /// Muestra los elementos de validacion de la contraseña,
        /// establece la contraseña del empleado seleccionado
        /// </summary>
        private void muestraRequisitoContrasena()
        {
            passBoxRequired.Visibility = Visibility.Visible;
            passBox.BorderBrush = Brushes.Red;
            passBox.Password = mvempleado.empleadoNuevo.Contraseña;

        }

        /// <summary>
        /// Gestiona el boton de borrar, 
        /// llama al metodo compruebaLoginUnico y si es true, 
        /// comprueba si el login del empleado es unico o no y llama al metodo borrar, 
        /// si el metodo devuleve false, llama directamente al metodo borrar empleado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Borrar_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings settings = new MetroDialogSettings()
            {
                NegativeButtonText = "No",
                AffirmativeButtonText = "Continuar",
                FirstAuxiliaryButtonText = "Cancelar"
            };

            MessageDialogResult result = await this.ShowMessageAsync("Confirmacion de seguridad", "Ha hecho clic en borrar empleado, esta seguro de que desea borrar el empleado?, si es asi haga clic en ''Continuar'' en caso contrario haga clic en ''No'', si desea cerrar todos los dialogos haga clic en ''Cancelar'' ", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
            if (result == MessageDialogResult.Affirmative)
            {

                if (seleccionado)
                {
                    mvempleado.editar = true;
                    if (compruebaLoginUnico())
                    {
                        if (mvempleado.comprobarLoginUnico())
                        {
                            borrarEmpleado();
                        }
                        else
                        {
                            await this.ShowMessageAsync("Informacion", "El login especificado ya existe en la base de datos");
                            mvempleado.empleadoNuevo.Login = login;
                        }
                    }
                    else
                    {
                        borrarEmpleado();
                    }
                }
                else
                {
                    MessageDialogResult result2 = await this.ShowMessageAsync("Informacion", "Tiene que elegir el login de un empleado, si no desea modificar un empleado haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
                    if (result2 == MessageDialogResult.Negative)
                    {
                        this.Close();
                    }
                }
            }
            else if (result == MessageDialogResult.FirstAuxiliary)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Comprueba que el formulario sea valido,
        /// comprueba la validacion del passwordbox 
        /// y borra el empleado de la base de datos
        /// </summary>
        private async void borrarEmpleado()
        {
            if (mvempleado.IsValid(this))
            {
                if (string.IsNullOrWhiteSpace(passBox.Password))
                {
                    passBoxRequired.Visibility = Visibility.Visible;
                    passBox.BorderBrush = Brushes.Red;
                    borrar.IsEnabled = false;
                    await this.ShowMessageAsync("Informacion", "Rellene los campos requeridos, Incluido el de contraseña...");
                }
                else
                {
                    passBoxRequired.Visibility = Visibility.Collapsed;
                    passBox.BorderBrush = Brushes.Black;
                    mvempleado.empleadoNuevo.Contraseña = passBox.Password;

                    if (mvempleado.borrar())
                    {
                        logger.Info("Empleado Borrado con id: " + mvempleado.empleadoNuevo.CodigoEmpleado);
                        this.DialogResult = true;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "Ha habido un error al borrar el empleado en la base de datos");
                        logger.Error("Error","Ha habido un error al borrar el empleado en la base de datos");
                        this.DialogResult = false;
                    }
                }
            }
            else
            {
                await this.ShowMessageAsync("Informacion", "Rellene los campos requeridos..");
            }
        }

        /// <summary>
        /// Comprueba que el login introducido es igual o no al del empleado seleccionado
        /// </summary>
        /// <returns>Devuelve true si el login no es igual, 
        /// si es igual, devuelve false</returns>
        private Boolean compruebaLoginUnico()
        {
            bool correcto = true;            
            if (login == mvempleado.empleadoNuevo.Login)
            {
                correcto = false;
            }          
            return correcto;
        }

        /// <summary>
        /// Gestiona el cambio de contraseña del campo contraseña, 
        /// comprueba la validacion del campo contraseña,
        /// ocultando o mostrando los elementos de validacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passBox.Password))
            {
                passBoxRequired.Visibility = Visibility.Visible;
                passBox.BorderBrush = Brushes.Red;
                borrar.IsEnabled = false;
            }
            else
            {
                passBoxRequired.Visibility = Visibility.Collapsed;
                passBox.BorderBrush = Brushes.Black;
                if (seleccionado)
                {
                    borrar.IsEnabled = true;
                }                
            }
        }

        /// <summary>
        /// Gestiona el cambio de seleccion del login,
        /// pone un booleano a true, para informar de que se ha seleccionado un login,
        /// y se guarda el empleado elegido, para poder comparar los campos originalmente,
        /// con los campos modificados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboLogin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            empleado empleadoGuardado;
            seleccionado = true;
            passBox.Password = mvempleado.empleadoNuevo.Contraseña;
            empleadoGuardado = mvempleado.empleadoNuevo;
            login = empleadoGuardado.Login;
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
        /// Gestiona que se pueda escribir en el password box,
        /// hace que no se pueda escribir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PassBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            
        }
    }
}
