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
using TallerMecanico.Servicios;

namespace TallerMecanico.Vista.Dialogos.loginDialogo
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private Logger logger;
        private tallermecanicoEntities tEnt;
        private EmpleadoServicio empServ;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        public Login()
        {
            InitializeComponent();
            tEnt = new tallermecanicoEntities();
            empServ = new EmpleadoServicio(tEnt);
            logger = LogManager.GetCurrentClassLogger();
            
        }
     

        /// <summary>
        /// Gestiona el boton de iniciar sesion, 
        /// valida el campo de login y el de contraseña, 
        /// comprueba que el login introducido exista en la base de datos, 
        /// y habre el dialogo MainWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txbUsuario.Text) || string.IsNullOrWhiteSpace(txbPassword.Password))
            {
                MessageBox.Show("Compruebe que ningun campo este vacio, recuerde que el nombre de usuario y la contraseña no puede contener carateres en blanco", "Error campos vacios", MessageBoxButton.OK,MessageBoxImage.Error);
                if (string.IsNullOrWhiteSpace(txbUsuario.Text) && string.IsNullOrWhiteSpace(txbPassword.Password))
                {
                    txbPassword.BorderBrush = Brushes.Red;
                    requeContra.Visibility = Visibility.Visible;

                    txbUsuario.BorderBrush = Brushes.Red;
                    requeUsu.Visibility = Visibility.Visible;
                }
                else
                if (string.IsNullOrWhiteSpace(txbUsuario.Text))
                {
                    txbUsuario.BorderBrush = Brushes.Red;
                    txbPassword.BorderBrush = Brushes.Blue;
                    requeUsu.Visibility = Visibility.Visible;
                    requeContra.Visibility = Visibility.Collapsed;
                }
                else
                if (string.IsNullOrWhiteSpace(txbPassword.Password))
                {
                    txbPassword.BorderBrush = Brushes.Red;
                    txbUsuario.BorderBrush = Brushes.Blue;
                    requeContra.Visibility = Visibility.Visible;
                    requeUsu.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                txbPassword.BorderBrush = Brushes.Blue;
                txbUsuario.BorderBrush = Brushes.Blue;
                requeUsu.Visibility = Visibility.Collapsed;
                requeContra.Visibility = Visibility.Collapsed;
                existeInforme.Visibility = Visibility.Collapsed;
                if (empServ.login(txbUsuario.Text, txbPassword.Password))
                {
                    MainWindow ventanaPrincipal = new MainWindow(tEnt, empServ.empleLogin);
                    ventanaPrincipal.Show();
                    logger.Info("Ha iniciado sesion el usuario: "+empServ.empleLogin.Nombre+" - "+empServ.empleLogin.Apellido);
                    this.Close();                    
                }
                else
                {
                    existeInforme.Visibility = Visibility.Visible;
                    txbPassword.BorderBrush = Brushes.Red;
                    txbUsuario.BorderBrush = Brushes.Red;
                    labelLogin.Margin = new Thickness(0);
                }
            }
        }

        /// <summary>
        /// Permite el arrastre de la ventana
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (InvalidOperationException ex)
            {

            }
            
        }

        /// <summary>
        /// Gestiona el boton de cerrar la ventana, cierra la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cerrar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Gestor del boton de minimizar,
        /// minimiza la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Gestiona el boton de facebook, abre la pagina de facebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FacebookBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://es-es.facebook.com/login");

        }

        /// <summary>
        /// Gestiona el boton de twitter, abre la pagina de twitter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TwitterBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitter.com/login");

        }

        /// <summary>
        /// Gestiona el boton de google plus, abre la pagina de google plus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GooglePlusBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://accounts.google.com/signin/v2/identifier?passive=1209600&continue=https%3A%2F%2Faboutme.google.com%2Fu%2F0%2F%3Freferer%3Dgplus&followup=https%3A%2F%2Faboutme.google.com%2Fu%2F0%2F%3Freferer%3Dgplus&flowName=GlifWebSignIn&flowEntry=ServiceLogin");

        }
    }
}
