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

namespace TallerMecanico.Vista.Dialogos.piezaDialogo
{
    /// <summary>
    /// Lógica de interacción para BorraPieza.xaml
    /// </summary>
    public partial class BorraPieza : MetroWindow
    {
        private bool seleccionado = false;
        
        private MVPieza mvpieza;
        private Logger logger;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvpieza"></param>
        public BorraPieza(MVPieza mvpieza)
        {
            InitializeComponent();            
            this.mvpieza = mvpieza;
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvpieza.OnErrorEvent));
            DataContext = mvpieza;
            mvpieza.btnGuardar = borrar;
            inicializa();
        }

        /// <summary>
        /// inicializa los componentes del dialogo
        /// </summary>
        private void inicializa()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Gestiona el boton de guardar,
        /// valida el dialogo, y borra la pieza
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
            MessageDialogResult result = await this.ShowMessageAsync("Confirmacion de seguridad", "Ha hecho clic en borrar pieza, esta seguro de que desea borrar la pieza?, si es asi haga clic en ''Continuar'' en caso contrario haga clic en ''No'', si desea cerrar todos los dialogos haga clic en ''Cancelar'' ", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);
            if (result == MessageDialogResult.Affirmative)
            {
                if (seleccionado)
                {
                    if (mvpieza.IsValid(this))
                    {
                        if (mvpieza.borra())
                        {
                            logger.Info("Pieza borrada con codigo: " + mvpieza.piezaNueva.CodigoPieza);
                            this.DialogResult = true;
                        }
                        else
                        {
                            logger.Error("Ha habido un error en la base de datos al borrar una pieza");
                            await this.ShowMessageAsync("Error","Ha habido un error al borrar la pieza de la base de datos");
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
                    MessageDialogResult result2 = await this.ShowMessageAsync("Informacion", "Para continuar tiene que elegir la pieza a borrar, si no desea borrar una pieza haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
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
        /// Gestiona el cambio de seleccion del combo de las piezas,
        /// pone un booleano a true para informar que se a seleccionado una pieza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboPieza_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            seleccionado = true;
        }
    }
}
