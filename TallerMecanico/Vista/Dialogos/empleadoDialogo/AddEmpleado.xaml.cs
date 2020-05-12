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
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class AddEmpleado : MetroWindow
    {        
        Logger logger;
        private MVEmpleado mvempleado;        

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvempleado">Clase que gestiona a los empleados</param>
        public AddEmpleado(MVEmpleado mvempleado)
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
        /// Pone visible el elemento de validacion del requisito de la contraseña
        /// </summary>
        private void muestraRequisitoContrasena()
        {
            passBoxRequired.Visibility = Visibility.Visible;
            passBox.BorderBrush = Brushes.Red;
            
        }
        

        /// <summary>
        /// Gestiona el boton de guardar, 
        /// comprueba que el login del empleado sea unico, valida el formulario,
        /// y guarda el empleado en la base de datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {          
            if (mvempleado.comprobarLoginUnico())
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
                            logger.Info("Empleado nuevo creado con id: " + mvempleado.empleadoNuevo.CodigoEmpleado);
                            this.DialogResult = true;
                        }
                        else
                        {
                            await this.ShowMessageAsync("Error", "Ha habido un error al insertar el empleado en la base de datos");
                            logger.Error("Error","Ha habido un error al añadir el empleado en la base de datos");
                            this.DialogResult = false;
                        }
                    }                    
                }
                else
                {
                    await this.ShowMessageAsync("Informacion", "Rellene los campos requeridos..");
                }
            }
            else
            {
                await this.ShowMessageAsync("Informacion","El login especificado ya existe en la base de datos");
            }
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
        /// Gestiona cuando cambia el texto del campo contraseña,
        /// oculta o muestra, los elementos de validacion, 
        /// y dependiendo de si el dialogo es valido o no,
        /// muestra o oculta el boton de guardar
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
                if (mvempleado.IsValid(this))
                {
                    guardar.IsEnabled = true;
                }
            }            
        }
    }
}
