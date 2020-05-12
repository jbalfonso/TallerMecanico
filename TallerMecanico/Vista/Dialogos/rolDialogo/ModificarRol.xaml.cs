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
    /// Lógica de interacción para ModificarRol.xaml
    /// </summary>
    public partial class ModificarRol : MetroWindow
    {

        private MVRol mvrol;
        private Logger logger;
        private bool selecciona = false;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvrol">Clase que gestiona los roles</param>
        public ModificarRol(MVRol mvrol)
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
        /// comprueba si se a seleccionado un rol a modificar,
        /// valida el dialogo, y edita el rol
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (selecciona)
            {
                if (mvrol.IsValid(this))
                {
                    mvrol.editar = true;
                    mvrol.permisosDrop = (ICollection<permiso>)checkCombo.SelectedItemsOverride;
                    mvrol.rolNuevo.permiso = mvrol.permisosDrop;
                    if (mvrol.guarda())
                    {
                        logger.Info("Rol modificado con codigo: " + mvrol.rolNuevo.CodigoRol);
                        this.DialogResult = true;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error","Ha habido un error al modificar el rol en la base de datos");
                        logger.Error("Ha habido un error en la base de datos al modificar un rol");
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
                MessageDialogResult result2 = await this.ShowMessageAsync("Informacion", "Para continuar tiene que elegir el rol a modificar, si no desea modificar un rol haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
                if (result2 == MessageDialogResult.Negative)
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Gestiona el boton de cancelar, cierra el dialogo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// Gestiona el cambio de seleccion del rol, pone un booleano a true,
        /// para informar de que se ha seleccionado un rol, 
        /// y actualiza la lista de permisos, del checkcombo box
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
