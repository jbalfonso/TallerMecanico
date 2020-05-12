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

namespace TallerMecanico.Vista.Dialogos.rolDialogo
{
    /// <summary>
    /// Lógica de interacción para BorraRol.xaml
    /// </summary>
    public partial class BorraRol : MetroWindow
    {

        private MVRol mvrol;
        private Logger logger;
        private bool selecciona = true;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvrol">Clase que gestiona los roles</param>
        public BorraRol(MVRol mvrol)
        {
            InitializeComponent();
            this.mvrol = mvrol;
            logger = LogManager.GetCurrentClassLogger();
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvrol.OnErrorEvent));
            DataContext = mvrol;
            mvrol.btnGuardar = guardar;
        }

        /// <summary>
        /// Gestiona el boton de guardar, 
        /// comprueba si se ha seleccionado un rol, 
        /// valida el dialogo y borra el rol de la base de datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings settings = new MetroDialogSettings()
            {
                NegativeButtonText = "No",
                AffirmativeButtonText = "Continuar",
                FirstAuxiliaryButtonText = "Cancelar"
            };
            MessageDialogResult result = await this.ShowMessageAsync("Confirmacion de seguridad", "Ha hecho clic en borrar rol, esta seguro de que desea borrar el rol?, "+Environment.NewLine +"si es asi haga clic en ''Continuar'' en caso contrario haga clic en ''No'',"+Environment.NewLine+" si desea cerrar todos los dialogos haga clic en ''Cancelar'' ", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
            if (result == MessageDialogResult.Affirmative)
            {
                if (selecciona)
                {
                    if (mvrol.IsValid(this))
                    {
                        mvrol.editar = true;
                        mvrol.permisosDrop = (ICollection<permiso>)checkCombo.SelectedItemsOverride;
                        mvrol.rolNuevo.permiso = mvrol.permisosDrop;
                        if (mvrol.borra())
                        {
                            logger.Info("Rol borrado con codigo: " + mvrol.rolNuevo.CodigoRol);
                            this.DialogResult = true;
                        }
                        else
                        {
                            logger.Error("Ha habido un error en la base de datos al borrar un rol");
                          await this.ShowMessageAsync("Error","Ha habido un error al borrar el rol de la base de datos");
                            this.DialogResult = false;
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("Informacion", "Rellene todos los campos requeridos");
                    }
                }
                else
                {
                    MessageDialogResult result2 = await this.ShowMessageAsync("Informacion", "Para continuar tiene que elegir el rol a borrar, si no desea borrar un rol haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
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
        /// Gestiona el cambio de seleccion del combo del rol, 
        /// pone un booleano a true, y añade los permisos de ese rol,
        /// al checkcombobox de los permisos, y refresca la lista de los items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selecciona = true;
            mvrol.permisosDrop = mvrol.rolNuevo.permiso;

            checkCombo.SelectedItemsOverride = mvrol.permisosDrop.ToList();
            checkCombo.Items.Refresh();
        }
    }
}
