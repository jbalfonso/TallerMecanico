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
    /// Lógica de interacción para ModificarEmpleado.xaml
    /// </summary>
    public partial class ModificarEmpleado : MetroWindow
    {
        
        private MVEmpleado mvempleado;
        private Logger logger;
        private bool seleccionado = false;
        private string login;
        
        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvempleado">Clase que gestiona los empleados</param>
        public ModificarEmpleado(MVEmpleado mvempleado)
        {
            InitializeComponent();            
            this.mvempleado = mvempleado;           
            logger = LogManager.GetCurrentClassLogger();
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvempleado.OnErrorEvent));
            DataContext = mvempleado;
            mvempleado.btnGuardar = guardar;
            muestraRequisitoContrasena();
           
        }

        /// <summary>
        /// Gestiona que se muestre la
        /// validacion del campo contraseña
        /// </summary>
        private void muestraRequisitoContrasena()
        {
            passBoxRequired.Visibility = Visibility.Visible;
            passBox.BorderBrush = Brushes.Red;
            passBox.Password = mvempleado.empleadoNuevo.Contraseña;
        }

        /// <summary>
        /// Gestiona el boton de guardar, comprueba que se haya seleccionado un empleado, 
        /// llama al metodo compruebaLoginUnico que si es verdadero,
        /// comprueba que el login sea unico, si es falso, edita el empleado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (seleccionado)
            { 
            mvempleado.editar = true;
                if (compruebaLoginUnico())
                {
                    if (mvempleado.comprobarLoginUnico())
                    { 
                        guardarEmpleado();
                    }
                    else
                    {
                        await this.ShowMessageAsync("Informacion", "El login especificado ya existe en la base de datos");
                        mvempleado.empleadoNuevo.Login = login;
                    }
                }
                else
                {
                    guardarEmpleado();
                }             
            }
            else
            {
                MessageDialogResult result = await this.ShowMessageAsync("Informacion", "Tiene que elegir el login de un empleado, si no desea modificar un empleado haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Negative)
                {
                    this.Close();
                }
            }



        }

        /// <summary>
        /// Comprueba que el dialogo sea valido, 
        /// que el campo de contraseña no este vacio,
        /// y edita el empleado
        /// </summary>
        private async void guardarEmpleado()
        {
            if (mvempleado.IsValid(this))
            {
                if (string.IsNullOrWhiteSpace(passBox.Password))
                {
                    passBoxRequired.Visibility = Visibility.Visible;
                    passBox.BorderBrush = Brushes.Red;
                    guardar.IsEnabled = false;
                    await this.ShowMessageAsync("Informacion", "Rellene los campos requeridos, Incluido el de contraseña...");
                }
                else
                {
                    passBoxRequired.Visibility = Visibility.Collapsed;
                    passBox.BorderBrush = Brushes.Black;
                    mvempleado.empleadoNuevo.Contraseña = passBox.Password;

                    if (mvempleado.guarda())
                    {
                        logger.Info("Empleado Modificado con id: " + mvempleado.empleadoNuevo.CodigoEmpleado);
                        this.DialogResult = true;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "Ha habido un error al modificar el empleado en la base de datos");
                        logger.Error("Ha habido un error al modificar el empleado en la base de datos");
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
            if (login==mvempleado.empleadoNuevo.Login)
            {                
                correcto = false;
            }            
            return correcto;
        }
       
        /// <summary>
        /// Gestiona el boton de cancelar, cierra la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Gestiona el cambio de seleccion de la contraseña, 
        /// dependiendo de si esta vacio el campo de contraseña o no,
        /// muestra o oculta los elementos de validacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PassBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(passBox.Password))
            {
                passBoxRequired.Visibility = Visibility.Visible;
                passBox.BorderBrush = Brushes.Red;
                guardar.IsEnabled = false;
            }
            else
            {
                passBoxRequired.Visibility = Visibility.Collapsed;
                passBox.BorderBrush = Brushes.Black;
                if (seleccionado)
                {
                    guardar.IsEnabled = true;
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

    }
}
