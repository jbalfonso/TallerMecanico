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
    /// Lógica de interacción para AddRol.xaml
    /// </summary>
    public partial class AddRol : MetroWindow
    {
        private Logger logger;

        private MVRol mvrol;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvrol">Clase que gestiona los roles</param>
        public AddRol(MVRol mvrol)
        {
            InitializeComponent();
            this.mvrol = mvrol;
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvrol.OnErrorEvent));
            DataContext = mvrol;
            mvrol.btnGuardar = guardar;
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Gestiona el boton de guardar, 
        /// valida el dialogo y guarda el rol en la base de datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (mvrol.IsValid(this))
            {
                if (mvrol.guarda())
                {
                    logger.Info("Rol añadido con codigo: " + mvrol.rolNuevo.CodigoRol);
                    this.DialogResult = true;
                }
                else
                {
                    logger.Error("Ha habido un error en la base de datos al añadir un rol");
                    await this.ShowMessageAsync("Error","Ha habido un error al añadir el rol en la base de datos");
                    this.DialogResult = false;
                }
            }
            else
            {
                await this.ShowMessageAsync("Informacion", "Rellene todos los campos requeridos");
            }
        }

        /// <summary>
        /// Gestiona el boton de cancelar, 
        /// cierra el dialogo,
        /// y deselecciona todo del combo de permisos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            permisos.UnSelectAll();
            this.DialogResult = false;
        }


    }
}
