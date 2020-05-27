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
using TallerMecanico.Vista.Dialogos.averiaDialogo;

namespace TallerMecanico.Vista.Dialogos.clientesDialogo
{
    /// <summary>
    /// Lógica de interacción para BorrarCliente.xaml
    /// </summary>
    public partial class BorrarCliente : MetroWindow
    {        
        private MVCliente mvcliente;
        private Logger logger;
        private bool selecciona = false;
        private MVAveria mvaveria;
        private List<int> permisosUsuarioLogeado;

        private const int perm_anularAveria = 3;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvcliente">Clase que gestiona los clientes</param>
        /// <param name="mvaveria">Clase que gestiona las averias</param>
        /// <param name="permisosUsuarioLogeado">Permisos que tiene el usuario logeado en la aplicacion</param>
        public BorrarCliente(MVCliente mvcliente,MVAveria mvaveria, List<int> permisosUsuarioLogeado)
        {
            InitializeComponent();            
            this.mvcliente = mvcliente;
            this.mvaveria = mvaveria;
            this.permisosUsuarioLogeado = permisosUsuarioLogeado;
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvcliente.OnErrorEvent));
            DataContext = mvcliente;
            mvcliente.btnGuardar = borrar;
            inicializa();

        }
        /// <summary>
        /// Inicializa componentes de la aplicacion
        /// </summary>
        private void inicializa()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        private void gestionaPermisos()
        {
            if (!permisosUsuarioLogeado.Contains(perm_anularAveria))
            {
                clickDerecho.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Gestiona el boton de borrar, 
        /// comprueba si se ha seleccionado un cliente, valida el dialogo,
        /// y borra el cliente de la base de datos
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
            MessageDialogResult result = await this.ShowMessageAsync("Confirmacion de seguridad", "Ha hecho clic en borrar cliente, esta seguro de que desea borrar el cliente?, si es asi haga clic en ''Continuar'' en caso contrario haga clic en ''No'', si desea cerrar todos los dialogos haga clic en ''Cancelar'' ", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
            if(result == MessageDialogResult.Affirmative)
            {
                if (selecciona)
                {
                    if (mvcliente.IsValid(this))
                    {
                        if (mvcliente.compruebaClienteAveria())
                        {
                           
                            if (mvcliente.borra())
                            {
                                logger.Info("Cliente borrado con codigo: " + mvcliente.clienteNuevo.CodigoCliente);
                                this.DialogResult = true;
                            }
                            else
                            {
                                logger.Error("Ha habido un error en la base de datos al borrar un Cliente");
                                await this.ShowMessageAsync("Error", "Ha habido un error al borrar el cliente de la base de datos");
                                this.DialogResult = false;
                            }
                        }
                        else
                        {
                            await this.ShowMessageAsync("Error", "No se puede borrar el cliente, porque hay averias que dependen de el, " + System.Environment.NewLine + " elimine las siguientes averias, y despues borre el cliente");
                            dgaverias.Visibility = Visibility.Visible;
                            dgaverias.ItemsSource = mvcliente.clienteConAveria;
                            dgaverias.Items.Refresh();
                        }
                        
                    }
                    else
                    {
                        await this.ShowMessageAsync("Informacion", "Rellene todos los campos requeridos");
                    }
                }
                else
                {
                    MessageDialogResult result2 = await this.ShowMessageAsync("Informacion", "Para continuar tiene que elegir el cliente a borrar, si no desea borrar un cliente haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
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
        /// Gestiona el boton de cancelar, cierra la aplicacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Gestiona el cambio de seleccion del cliente,
        /// pone a true un booleano para informar de que se ha seleccionado un cliente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selecciona = true;
        }

        /// <summary>
        /// Gestiona la muestra y borrado de la averia seleccionada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BorrarDialogo_Click(object sender, RoutedEventArgs e)
        {
            if (dgaverias.SelectedItem != null)
            {
                mvaveria.averiaNueva = (averia)dgaverias.SelectedItem;
                 AnularAveria dialogo = new AnularAveria(mvaveria);
                dialogo.ShowDialog();
                if (dialogo.DialogResult == true)
                {
                    
                        mvcliente.clienteConAveria.Remove((averia)dgaverias.SelectedItem);
                        dgaverias.Items.Refresh();
                        await this.ShowMessageAsync("Informacion", "Borrado correctamente");
                    
                }
            }
        }
    }
}
