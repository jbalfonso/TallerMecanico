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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TallerMecanico.Modelo;
using TallerMecanico.MVVM;
using TallerMecanico.Vista.Dialogos;
using TallerMecanico.Vista.Dialogos.averiaDialogo;
using TallerMecanico.Vista.Dialogos.empleadoDialogo;
using TallerMecanico.Vista.Dialogos.loginDialogo;
using TallerMecanico.Vista.Dialogos.piezaDialogo;
using TallerMecanico.Vista.ControlesUsuario;
using TallerMecanico.Vista.Dialogos.rolDialogo;
using TallerMecanico.Vista.Dialogos.clientesDialogo;
using TallerMecanico.Vista.Charts;
using TallerMecanico.Servicios;
using CrystalDecisions.CrystalReports.Engine;
using ToastNotifications;
using ToastNotifications.Position;
using ToastNotifications.Lifetime;
using ToastNotifications.Lifetime.Clear;
using ToastNotifications.Messages;


namespace TallerMecanico
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        /// <summary>
        /// Notificador, que utiliza toast para mostrar las notificaciones
        /// </summary>
        Notifier notifier = new Notifier(cfg =>
        {
            try
            {               
                cfg.PositionProvider = new PrimaryScreenPositionProvider(                    
                    corner: Corner.TopRight,
                    offsetX: 10,
                    offsetY: 180);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromDays(999),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(12));

                cfg.DisplayOptions.TopMost = true;               

                cfg.Dispatcher = Application.Current.Dispatcher;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex+"");
            }
        });

        private Logger logger;
        private empleado empleadoLogin;
        private tallermecanicoEntities tEnt;

        private MVEmpleado mvempleado;
        private MVAveria mvaveria;
        private MVPieza mvpieza;
        private MVRol mvrol;
        private MVCliente mvcliente;
        private MVFacturacion mvfactura;
        private ServicioSQL sqlServ;
        private ReportDocument rd;
        

        private List<int> permisosUsuarioLogeado = new List<int>();


        #region constantesCobro
        //Constantes de los tipos de metodos de pago que hay
        private const int tarjeta = 1;
        private const int efectivo = 2;
        private const int paypal = 3;

        #endregion

        #region constantesPermisos
        //Constantes de los nombres de los tipos de permisos que hay en la aplicacion
        private const int perm_anyadirAveria = 1;
        private const int perm_resolverAveria = 2;
        private const int perm_anularAveria = 3;
        private const int perm_cobroCliente = 4;
        private const int perm_devolucionCliente = 5;
        private const int perm_anyadirPieza = 6;
        private const int perm_modificarEliminarPieza = 7;
        private const int perm_planificarTrabajo = 8;
        private const int perm_gestionarUsuario = 9;
        private const int perm_editarPermisos = 10;
        private const int perm_cambioContrasenya = 11;
        private const int perm_cambioContrasenyaPropia = 12;
        private const int perm_editarRoles = 13;
        private const int perm_anyadirCliente = 14;
        private const int perm_modificarEliminarCliente = 15;

        #endregion

        /// <summary>
        /// Constructor de la ventana principal MainWindow
        /// </summary>
        /// <param name="tEnt"> Entidad de la base de datos del tallerMecanico</param>
        /// <param name="empleLogin"> Empleado que a iniciado sesion en la aplicacion</param>
        public MainWindow(tallermecanicoEntities tEnt, empleado empleLogin)
        {
            InitializeComponent();
            this.empleadoLogin = empleLogin;
            this.tEnt = tEnt;       

            inicializar();
            paginaInicio();
            gestionUsuario();
            gestionaNotificaciones();     
        }  

                

        /// <summary>
        /// Inicializa los componentes principales de la aplicacion
        /// </summary>
        private void inicializar()
        {
           logger = LogManager.GetCurrentClassLogger();
            txblockNombreUsuario.Text = empleadoLogin.Nombre;
            mvempleado = new MVEmpleado(tEnt);
            mvaveria = new MVAveria(tEnt);
            mvpieza = new MVPieza(tEnt);
            mvrol = new MVRol(tEnt);
            mvcliente = new MVCliente(tEnt);
            mvfactura = new MVFacturacion(tEnt);
            sqlServ = new ServicioSQL(tEnt);
            rd = new ReportDocument();            
            
        }        


        #region usuario
        /// <summary>
        /// Gestiona los permisos del usuario logeado,
        /// oculta o muestra elementos de la aplicacion dependiendo de los permisos del usuario
        /// </summary>
        private void gestionUsuario()
        {
            permisosUsuarioLogeado.Clear();
            ICollection<permiso> permisosUsuario = empleadoLogin.rol.permiso;

            foreach (permiso perm in permisosUsuario)
            {
                permisosUsuarioLogeado.Add(perm.CodigoPermiso);
            }

            comprobarPermisos();


        }
        /// <summary>
        /// Oculta o muestra elementos de la aplicacion dependiendo de los permisos
        /// </summary>
        private void comprobarPermisos()
        {
            editarAveriaBtn.Visibility = Visibility.Collapsed;

            if (!permisosUsuarioLogeado.Contains(perm_anyadirAveria))
            {
                anadirAveriaBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                anadirAveriaBtn.Visibility = Visibility.Visible;
            }

            if (!permisosUsuarioLogeado.Contains(perm_resolverAveria))
            {
                editarAveriaBtn.Visibility = Visibility.Collapsed;
                anyadirResolucionBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                anyadirResolucionBtn.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_anularAveria))
            {
                AnularAveriaBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                AnularAveriaBtn.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_cobroCliente))
            {
                cobrarCliente.Visibility = Visibility.Collapsed;                
            }
            else
            {
                cobrarCliente.Visibility = Visibility.Visible;                
            }
            if (!permisosUsuarioLogeado.Contains(perm_devolucionCliente))
            {
                devolverCliente.Visibility = Visibility.Collapsed;
            }
            else
            {
                devolverCliente.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_anyadirPieza))
            {
                anadirPiezaBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                anadirPiezaBtn.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_modificarEliminarPieza))
            {
                editarPiezaBtn.Visibility = Visibility.Collapsed;
                eliminarPiezaBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                editarPiezaBtn.Visibility = Visibility.Visible;
                eliminarPiezaBtn.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_planificarTrabajo))
            {
                tabPlanificacion.Visibility = Visibility.Collapsed;
            }
            else
            {
                tabPlanificacion.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_gestionarUsuario))
            {
                EmpleadoGroup.Visibility = Visibility.Collapsed;
            }
            else
            {
                EmpleadoGroup.Visibility = Visibility.Visible;
            }

            if (!permisosUsuarioLogeado.Contains(perm_cambioContrasenya))
            {                
                ModificarEmpleadoBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                ModificarEmpleadoBtn.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_cambioContrasenyaPropia))
            {
                
                CambiarContrasenaBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                CambiarContrasenaBtn.Visibility = Visibility.Visible;
            }
           
            if (!permisosUsuarioLogeado.Contains(perm_editarRoles))
            {
                rolGroup.Visibility = Visibility.Collapsed;
            }
            else
            {
                rolGroup.Visibility = Visibility.Visible;
            }
            if (permisosUsuarioLogeado.Contains(perm_anyadirAveria) && permisosUsuarioLogeado.Contains(perm_anularAveria))
            {
                editarAveriaBtn.Visibility = Visibility.Visible;
            }
            else
            {
                editarAveriaBtn.Visibility = Visibility.Collapsed;
            }
            if (!permisosUsuarioLogeado.Contains(perm_editarRoles) && !permisosUsuarioLogeado.Contains(perm_cambioContrasenya)
                && !permisosUsuarioLogeado.Contains(perm_gestionarUsuario))
            {
                tabEmpleados.Visibility = Visibility.Collapsed;
            }
            else
            {
                tabEmpleados.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_anyadirAveria) && !permisosUsuarioLogeado.Contains(perm_resolverAveria) 
                && !permisosUsuarioLogeado.Contains(perm_anularAveria))
            {
                tabAverias.Visibility = Visibility.Collapsed;
            }
            else
            {
                tabAverias.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_anyadirPieza) && !permisosUsuarioLogeado.Contains(perm_modificarEliminarPieza))
            {
                tabPiezas.Visibility = Visibility.Collapsed;
            }
            else
            {
                tabPiezas.Visibility = Visibility.Visible;
            }
            if(!permisosUsuarioLogeado.Contains(perm_cobroCliente) && !permisosUsuarioLogeado.Contains(perm_devolucionCliente))
            {
                FacturacionGroup.Visibility = Visibility.Collapsed;
            }
            else
            {
                FacturacionGroup.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_anyadirCliente))
            {
                anyadirCliente.Visibility = Visibility.Collapsed;
            }
            else
            {
                anyadirCliente.Visibility = Visibility.Visible;
            }
            if (!permisosUsuarioLogeado.Contains(perm_modificarEliminarCliente))
            {
                modificarCliente.Visibility = Visibility.Collapsed;
                borrarCliente.Visibility = Visibility.Collapsed;
            }
            else
            {
                modificarCliente.Visibility = Visibility.Visible;
                borrarCliente.Visibility = Visibility.Visible;
            }
            if(!permisosUsuarioLogeado.Contains(perm_anyadirCliente) && !permisosUsuarioLogeado.Contains(perm_modificarEliminarCliente))
            {
                clientesGroup.Visibility = Visibility.Collapsed;
            }
            else
            {
                clientesGroup.Visibility = Visibility.Visible;
            }
            if(!permisosUsuarioLogeado.Contains(perm_cobroCliente) && 
                !permisosUsuarioLogeado.Contains(perm_devolucionCliente) && 
                !permisosUsuarioLogeado.Contains(perm_anyadirCliente) && 
                !permisosUsuarioLogeado.Contains(perm_modificarEliminarCliente))
            {
                ClientesTab.Visibility = Visibility.Collapsed;
            }
            else
            {
                ClientesTab.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region notificaciones

        /// <summary>
        /// Gestiona las notificaciones al inicio de la aplicacion,
        /// primero muestra las notificaciones de los pedidos que hay que hacer en la temporada
        /// luego muestra las notificaciones del stock 
        /// </summary>       
        private async void gestionaNotificaciones()
        {
            await Task.Delay(2000);

            int mesActual = DateTime.Now.Month;
            if (mesActual >= 4 && mesActual <= 6)
            {
                notifier.ShowInformation("Recordatorio, temporada de primavera, realizar pedido de Filtros de polen");
            }
            else if (mesActual >= 7 && mesActual <= 9)
            {
                notifier.ShowInformation("Recordatorio, temporada de verano, realizar pedido de Ruedas");
            }
            else if (mesActual >= 10 && mesActual <= 12)
            {
                notifier.ShowInformation("Recordatorio, temporada de otoño, realizar pedido de Correas de distribucion");
            }
            else if (mesActual >= 1 && mesActual <= 3)
            {
                notifier.ShowInformation("Recordatorio, temporada de invierno, realizar pedido de Limpiaparabrisas");
            }

            foreach (pieza pza in mvpieza.listaPiezas)
            {
                gestionaNotificacionPieza(pza);
            }

        }
        /// <summary>
        /// Gestiona la notificacion por pieza,
        /// si la cantidad de la pieza es inferior a 5,
        /// se muestra si se ha agotado el stock y la cantidad restante
        /// si la cantidad es mayor que 5 y menor que 15,
        /// muestra una notificacion de advertencia, con la cantidad del stock restante
        /// </summary>
        /// <param name="pza"></param>
        private void gestionaNotificacionPieza(pieza pza)
        {
            if (pza.Cantidad <= 5)
            {
                string cantidad = pza.Cantidad + "";
                if (pza.Cantidad == 0)
                {
                    notifier.ShowError("Se a agotado el stock de:  ''" + pza.Descripcion + "''");
                }
                else
                {
                    notifier.ShowError("Se esta agotando el stock de:  ''" + pza.Descripcion + "'' la cantidad restante es:  " + cantidad);
                }
            }
            else if (pza.Cantidad > 5 && pza.Cantidad <= 15)
            {
                notifier.ShowWarning("El volumen de stock de la pieza: ''" + pza.Descripcion + "'' es inferior a 15, vaya pensando en realizar un pedido nuevo");
            }
        }
        /// <summary>
        /// Este metodo gestiona el stock de piezas, 
        /// de las averias nuevas o modificadas, comprobando su nuevo stock
        /// </summary>
        private async void gestionaStockAverias()
        {
            try
            {
                foreach (pieza pza in mvaveria.averiaNueva.pieza)
                {
                    gestionaNotificacionPieza(pza);
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error", "Ha habido un error al gestionar la notificaciones de las piezas");
            }
        }
        private async void gestionaStockAveriaModificada(averia averia)
        {
            try
            {
                foreach (pieza pza in averia.pieza)
                {
                    gestionaNotificacionPieza(pza);
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error", "Ha habido un error al gestionar la notificaciones de las piezas");
            }
        }

        #endregion

        #region barraSuperior
        /// <summary>
        /// Gestor del cierre de la aplicacion al hacer clic en el icono de cerrar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            notifier.Dispose();            
            this.Close();
        }
        /// <summary>
        /// Gestor del boton de minimizar, minimiza la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// Gestor del boton de maximizar, cuando esta checkeado
        /// pone la aplicacion en su estado normal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizarBtn_Checked(object sender, RoutedEventArgs e)
        {

            this.WindowState = WindowState.Normal;
        }
        /// <summary>
        /// Gestor del boton maximizar, cuando no esta checkeado
        /// maximiza la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizarBtn_Unchecked(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }
        /// <summary>
        /// Es el controlador del clic izquierdo del raton,
        /// cuando este es presionado, permite el arrastre de la ventana
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion

        #region dropDownUsuario
        /// <summary>
        /// Gestor del boton de cambiar la contraseña propia
        /// cuando es presionado, muestra el flyout con el dialogo de cambiar la contraseña
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CambiarContrasena_Click(object sender, RoutedEventArgs e)
        {
            flyout.IsOpen = true;
        }

        /// <summary>
        /// Gestor del boton de cerrar sesion, carga el dialogo de login,
        /// lo muestra y cierra esta ventana
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login ventana = new Login();
            notifier.ClearMessages(new ClearAll());
            notifier.Dispose();
            ventana.Show();

            this.Close();
        }

        /// <summary>
        /// Gestor para la ocultacion del formulario,
        /// de cambio de contraseña propia, oculta el dialogo y la validacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cerrarCambiarContrasena_Click(object sender, RoutedEventArgs e)
        {
            cerrarCambiarContrasena();
        }

        private void cerrarCambiarContrasena()
        {
            flyout.IsOpen = false;
            contrasenaIncorrecta.Visibility = Visibility.Collapsed;
            contrasenaBlanco.Visibility = Visibility.Collapsed;
            contrasenaActual.BorderBrush = Brushes.White;
            contrasenaNueva.BorderBrush = Brushes.White;
            contrasenaActual.Password = "";
            contrasenaNueva.Password = "";
            mvempleado = new MVEmpleado(tEnt);
        }
        /// <summary>
        /// Gestor del boton de guardar contraseña propia,
        /// comprueba si los textbox estan vacios,
        /// y luego compara si la contraseña actual introducida es igual que la actual, 
        /// luego modifica la contraseña y la guarda
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GuardarCambiarContrasena_Click(object sender, RoutedEventArgs e)
        {
            contrasenaBlanco.Visibility = Visibility.Collapsed;
            contrasenaIncorrecta.Visibility = Visibility.Collapsed;
            try
            {
                if (!string.IsNullOrEmpty(contrasenaActual.Password) && !string.IsNullOrEmpty(contrasenaNueva.Password))
                {
                    if (empleadoLogin.Contraseña == contrasenaActual.Password)
                    {
                        mvempleado.editar = true;
                        empleadoLogin.Contraseña = contrasenaNueva.Password;
                        mvempleado.empleadoNuevo = empleadoLogin;

                        if (mvempleado.guarda())
                        {
                            cerrarCambiarContrasena();
                            await this.ShowMessageAsync("Exito", "El cambio se realizo correctamente...");
                        }
                        else
                        {
                            cerrarCambiarContrasena();
                            await this.ShowMessageAsync("Error", "Ha habido un error al modificar la contraseña en la base de datos");
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "La contraseña de login no coincide con la del usuario, no se realiza el cambio de la contraseña");
                        contrasenaActual.BorderBrush = Brushes.Red;
                        contrasenaNueva.BorderBrush = Brushes.Red;
                        contrasenaBlanco.Visibility = Visibility.Collapsed;
                        contrasenaIncorrecta.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    contrasenaActual.BorderBrush = Brushes.Red;
                    contrasenaNueva.BorderBrush = Brushes.Red;
                    contrasenaBlanco.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Ha habido un error al cambiar la contraseña en la base de datos", ex);
                await this.ShowMessageAsync("Error", "Ha habido un error al cambiar la contraseña");
            }
        }

        #endregion        

        #region empleado

        /// <summary>
        /// Gestiona el boton de crear empleado, 
        /// abre el formulario, y muestra un mensaje si se ha guardado o no
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void crearEmpleadobtn_click(object sender, RoutedEventArgs e)
        {
            AddEmpleado dialogo = new AddEmpleado(mvempleado);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Empleado añadido correctamente");
                mvempleado = new MVEmpleado(tEnt);
            }
            else
            {
                mvempleado = new MVEmpleado(tEnt);
            }
        }

        /// <summary>
        /// Gestiona el boton de modificacion de empleado, 
        /// abre el formulario y muestra un mensaje dependiendo del estado del formulario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void modificarEmpleadobtn_Click(object sender, RoutedEventArgs e)
        {
            ModificarEmpleado dialogo = new ModificarEmpleado(mvempleado);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Empleado modificado correctamente");
                if (mvempleado.empleadoNuevo == empleadoLogin)
                {                   
                        empleadoLogin = mvempleado.empleadoNuevo;
                        gestionUsuario();                    
                }
                mvempleado = new MVEmpleado(tEnt);
            }
            else
            {
                mvempleado = new MVEmpleado(tEnt);
            }
        }

        /// <summary>
        /// Gestiona el boton de borrado de empleado, 
        /// abre el dialogo y muestra un mensaje si el resultado es true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BorrarEmpleadoBtn_Click(object sender, RoutedEventArgs e)
        {
            BorrarEmpleado dialogo = new BorrarEmpleado(mvempleado,empleadoLogin);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Empleado borrado correctamente");
                mvempleado = new MVEmpleado(tEnt);
            }
            else
            {
                mvempleado = new MVEmpleado(tEnt);
            }
        }

        #endregion

        #region averia

        /// <summary>
        /// Gestiona el boton ed añadir averia, 
        /// muestra el dialogo, luego muestra un mensaje y comprueba el stock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void anyadirAveria_Click(object sender, RoutedEventArgs e)
        {
            AddAveria dialogo = new AddAveria(mvaveria,empleadoLogin);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {                
                await this.ShowMessageAsync("Informacion", "Averia insertada correctamente");
                gestionaStockAverias();
                mvaveria = new MVAveria(tEnt);
            }
            else
            {
                mvaveria = new MVAveria(tEnt);
            }
        }

        /// <summary>
        /// Gestiona el boton de modificar averia,
        /// muestra el dialogo, y luego comprueba el stock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void modificarAveria_Click(object sender, RoutedEventArgs e)
        {
            ModificarAveria dialogo = new ModificarAveria(mvaveria,this.gestionaStockAveriaModificada);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion","Averia modificada correctamente");                
                mvaveria = new MVAveria(tEnt);
            }
            else
            {
                mvaveria = new MVAveria(tEnt);
            }

        }

        /// <summary>
        /// Gestiona el boton de anular averia, 
        /// muestra el dialogo y comprueba el stock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void anularAveria_Click(object sender, RoutedEventArgs e)
        {
            AnularAveria dialogo = new AnularAveria(mvaveria);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion","Averia anulada corectamente");
                gestionaStockAverias();
                mvaveria = new MVAveria(tEnt);
            }
            else
            {
                mvaveria = new MVAveria(tEnt);
            }
        }

        /// <summary>
        /// Gestiona el boton de añadir resolucion, 
        /// el cual muestra el formulario de añadir una resolucion a una averia,
        /// y muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AnyadirResolucionBtn_Click(object sender, RoutedEventArgs e)
        {
            InterfazResolucion dialogo = new InterfazResolucion(mvaveria,empleadoLogin);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Resolucion agregada correctamente");
                mvaveria = new MVAveria(tEnt);
            }
            else
            {
                mvaveria = new MVAveria(tEnt);
            }
        }

        /// <summary>
        /// Muestra el control de usuario del listado de averias, 
        /// permite añadir averias, modificar, y anular
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BusquedaAveria_Click(object sender,RoutedEventArgs e)
        {
            UCBusquedaAverias control = new UCBusquedaAverias(tEnt,mvaveria,
                permisosUsuarioLogeado,this.gestionaStockAveriaModificada,empleadoLogin);

            if (Contenido.Children != null) Contenido.Children.Clear();
            Contenido.Children.Add(control);
        }

        #endregion

        #region pieza

        /// <summary>
        /// Abre el formulario del listado de las piezas, tambien permite la agregacion, eliminacion y modificacion de piezas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BusquedaPiezaBtn_Click(object sender, RoutedEventArgs e)
        {
            UCBusquedaPiezas control = new UCBusquedaPiezas(tEnt,mvpieza, permisosUsuarioLogeado,this.gestionaNotificacionPieza);
            if (Contenido.Children != null) Contenido.Children.Clear();
            Contenido.Children.Add(control);
        }

        /// <summary>
        /// Gestiona el boton de añadir pieza, 
        /// muestra el formulario y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AnadirPiezaBtn_Click(object sender, RoutedEventArgs e)
        {
            AddPieza dialogo = new AddPieza(mvpieza);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Pieza añadida corectamente");
                gestionaNotificacionPieza(mvpieza.piezaNueva);
                mvpieza = new MVPieza(tEnt);
            }
            else
            {
                mvpieza = new MVPieza(tEnt);
            }
        }

        /// <summary>
        /// Gestiona el boton de editar pieza, muestra el dialogo, y luego un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EditarPiezaBtn_Click(object sender, RoutedEventArgs e)
        {
            ModificarPieza dialogo = new ModificarPieza(mvpieza,this.gestionaNotificacionPieza);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Pieza Modificada corectamente");                
                mvpieza = new MVPieza(tEnt);
                
            }
            else
            {               
                mvpieza = new MVPieza(tEnt);                
            }
        }

        /// <summary>
        /// Gestiona el boton de eliminar pieza, 
        /// muestra el dialogo, y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EliminarPiezaBtn_Click(object sender, RoutedEventArgs e)
        {
            BorraPieza dialogo = new BorraPieza(mvpieza,mvaveria,this.gestionaStockAveriaModificada);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Pieza borrada del stock corectamente");
                mvpieza = new MVPieza(tEnt);
                mvaveria = new MVAveria(tEnt);
            }
            else
            {
                mvpieza = new MVPieza(tEnt);
                mvaveria = new MVAveria(tEnt);
            }
        }

        #endregion

        #region rol

        /// <summary>
        /// Gestiona el boton de añadir rol, 
        /// muestra el formulario y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AnadirRol_Click(object sender, RoutedEventArgs e)
        {
            AddRol dialogo = new AddRol(mvrol);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Rol añadido corectamente");
                mvrol = new MVRol(tEnt);
            }
            else
            {
                mvrol = new MVRol(tEnt);
            }
        }

        /// <summary>
        /// Gestiona el boton de editar rol, muestra el formulario, y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EditarRol_Click(object sender, RoutedEventArgs e)
        {
            ModificarRol dialogo = new ModificarRol(mvrol);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Rol modificado corectamente");
                if (mvrol.rolNuevo == empleadoLogin.rol)
                {
                    empleadoLogin.rol = mvrol.rolNuevo;
                    gestionUsuario();
                }
                mvrol = new MVRol(tEnt);                
            }
            else
            {
                mvrol = new MVRol(tEnt);
            }
        }


        /// <summary>
        /// Gestiona el boton de eliminar rol, muestra el dialogo, y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EliminarRol_Click(object sender, RoutedEventArgs e)
        {
           BorraRol dialogo = new BorraRol(mvrol,mvempleado,permisosUsuarioLogeado);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Rol borrado corectamente");
                mvrol = new MVRol(tEnt);
                mvempleado = new MVEmpleado(tEnt);
            }
            else
            {
                mvrol = new MVRol(tEnt);
                mvempleado = new MVEmpleado(tEnt);
            }
        }

        #endregion

        #region cliente

        /// <summary>
        /// Gestiona el boton de añadir clientes,
        /// muestra el dialogo, y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AnyadirCliente_Click(object sender, RoutedEventArgs e)
        {
            AddClientes dialogo = new AddClientes(mvcliente);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Cliente insertado corectamente");
                mvcliente = new MVCliente(tEnt);
                
            }
            else
            {
                mvcliente = new MVCliente(tEnt);
            }
        }

        /// <summary>
        /// Gestiona el boton de modificar cliente,muestra el dialogo y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ModificarCliente_Click(object sender, RoutedEventArgs e)
        {
            ModificarCliente dialogo = new ModificarCliente(mvcliente);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Cliente modificado corectamente");
                mvcliente = new MVCliente(tEnt);
            }
            else
            {
                mvcliente = new MVCliente(tEnt);
            }
        }

        /// <summary>
        /// Gestiona el boton de borrar cliente, muestra el dialogo, y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BorrarCliente_Click(object sender, RoutedEventArgs e)
        {
            BorrarCliente dialogo = new BorrarCliente(mvcliente,mvaveria,permisosUsuarioLogeado);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                await this.ShowMessageAsync("Informacion", "Cliente borrado corectamente");
                mvcliente = new MVCliente(tEnt);
                mvaveria = new MVAveria(tEnt);
            }
            else
            {
                mvcliente = new MVCliente(tEnt);
                mvaveria = new MVAveria(tEnt);
            }
        }


        /// <summary>
        /// Gestiona el boton de cobrar, muestra el formulario de cobro, 
        /// y despues muestra el informe de la factura del cliente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CobrarCliente_Click(object sender, RoutedEventArgs e)
        {
            InterfazCobro dialogo = new InterfazCobro(mvfactura,empleadoLogin);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                UCFactura control = new UCFactura(rd, mvfactura);
                if (Contenido.Children != null) Contenido.Children.Clear();
                Contenido.Children.Add(control);

                mvfactura = new MVFacturacion(tEnt);
            }
            else
            {
                mvfactura = new MVFacturacion(tEnt);
            }
        }

        /// <summary>
        /// Gestiona el boton de devolucion, 
        /// muestra el formulario, para la devolucion del servicio del cliente, 
        /// y luego muestra el informe del justificante de devolucion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DevolverCliente_Click(object sender, RoutedEventArgs e)
        {
            InterfazDevolucion dialogo = new InterfazDevolucion(mvfactura,empleadoLogin);
            dialogo.ShowDialog();
            if (dialogo.DialogResult == true)
            {
                UCDevolucion control = new UCDevolucion(rd, mvfactura);
                if (Contenido.Children != null) Contenido.Children.Clear();
                Contenido.Children.Add(control);

                mvfactura = new MVFacturacion(tEnt);
            }
            else
            {
                mvfactura = new MVFacturacion(tEnt);
            }
        }

        #endregion

        #region Graficos

        /// <summary>
        /// Gestiona el boton de mostrar el chart,
        /// muestra el control de usuario que carga el chart en el grid contenido
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChartResolucion_Click(object sender, RoutedEventArgs e)
        {
           UCChart  control = new UCChart(tEnt, mvaveria);
            if (Contenido.Children != null) Contenido.Children.Clear();
            Contenido.Children.Add(control);
        }

        #endregion

        #region informes

        /// <summary>
        /// Gestiona el boton de mostrar informe pieza,
        /// muestra el control de usuario que gestiona la generacion del informe de las piezas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InformePieza_Click(object sender, RoutedEventArgs e)
        {
            UCInformePieza control = new UCInformePieza(sqlServ,rd);
            if (Contenido.Children != null) Contenido.Children.Clear();
            Contenido.Children.Add(control);
        }

        /// <summary>
        /// Gestiona el boton de mostrar el informe de averia, 
        /// muestra el control de usuario que gestiona la generacion del informe de las averias
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InformeAveria_Click(object sender, RoutedEventArgs e)
        {
            UCInformeAverias control = new UCInformeAverias(sqlServ, rd);
            if (Contenido.Children != null) Contenido.Children.Clear();
            Contenido.Children.Add(control);
        }

        #endregion

        #region ventana

        /// <summary>
        /// Carga un control de usuario el cual muestra el titulo de la aplicacion
        /// con la fecha y la hora
        /// </summary>
        private void paginaInicio()
        {
            UCInicio UCInicio = new UCInicio();
            if (Contenido.Children != null) Contenido.Children.Clear();
            Contenido.Children.Add(UCInicio);
        }

        /// <summary>
        /// Gestor del control ribbon, 
        /// quita los elementos visuales inecesarios
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ribbon_Loaded(object sender, RoutedEventArgs e)
        {
            Grid child = VisualTreeHelper.GetChild((DependencyObject)sender, 0) as Grid;
            if (child != null)
            {
                child.RowDefinitions[0].Height = new GridLength(0);
            }
        }

        /// <summary>
        /// Gestiona el boton de limpar el contenido del centro de la aplicacion
        /// y del grid contenido, cargar el metodo paginaInicio 
        /// el cual carga el user control del inicio de la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimpiarContenido_Click(object sender, RoutedEventArgs e)
        {
            paginaInicio();
        }

        /// <summary>
        /// Gestiona el evento de cuando el usuario hace doble clic en la ventana,
        /// en caso de que la ventana no este maximizada, la maximiza, y pone el toglebutton de maximizar a false,
        /// en caso de que la ventana este maximizada, la pone en estado normal y pone el tooglebutton de maximizar a true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void minimizarDobleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                e.Handled = true;
                if (this.WindowState == WindowState.Normal)
                {
                    this.WindowState = WindowState.Maximized;
                    maximizeBtn.IsChecked = false;
                }
                else if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                    maximizeBtn.IsChecked = true;
                }
            }
        }

        #endregion
    }
}
