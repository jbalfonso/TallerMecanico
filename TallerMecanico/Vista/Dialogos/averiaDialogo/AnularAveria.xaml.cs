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

namespace TallerMecanico.Vista.Dialogos.averiaDialogo
{
    /// <summary>
    /// Lógica de interacción para AnularAveria.xaml
    /// </summary>
    public partial class AnularAveria : MetroWindow
    {
        
        private MVAveria mvaveria;
        private Logger logger;
        private bool seleccionado = false;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>        
        /// <param name="mvaveria">Clase de gestion de las averias</param>
        public AnularAveria(MVAveria mvaveria)
        {
            InitializeComponent();            
            this.mvaveria = mvaveria;
            inicializar();
        }
        /// <summary>
        /// Inicializa componentes de la aplicacion
        /// </summary>
        private void inicializar()
        {
            
            logger = LogManager.GetCurrentClassLogger();
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvaveria.OnErrorEvent));
            DataContext = mvaveria;
            mvaveria.btnGuardar = anular;  
        }

        /// <summary>
        /// Gestor del boton de anular, 
        /// comprueba si se a seleccionado una averia, 
        /// anula la averia seleccionada, y aumenta el stock de las piezas utilizadas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonAnular_Click(object sender, RoutedEventArgs e)
        {
            MetroDialogSettings settings = new MetroDialogSettings()
            {
                NegativeButtonText = "No", AffirmativeButtonText = "Continuar", FirstAuxiliaryButtonText = "Cancelar"
            };

            MessageDialogResult result = await this.ShowMessageAsync("Confirmacion de seguridad", "Ha hecho clic en anular averia, "+System.Environment.NewLine+" esta seguro de que desea anular la averia? "+System.Environment.NewLine+" si es asi haga clic en ''Continuar'' en caso contrario haga clic en ''No'', "+System.Environment.NewLine+" si desea cerrar todos los dialogos haga clic en ''Cancelar'' ",MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,settings);
             if (result == MessageDialogResult.Affirmative)
            {
                if (seleccionado)
                {
                    if (mvaveria.anula())
                    {
                        if (mvaveria.aumentaStock())
                        {
                            logger.Info("Aumentado el stock, al anular una averia");
                        }
                        else
                        {
                            await this.ShowMessageAsync("Error","Ha habido un problema al reducir el stock");
                        }
                        logger.Info("Averia anulada con codigo: " + mvaveria.averiaNueva.CodigoAveria);
                        this.DialogResult = true;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "Ha habido un error al anular la averia en la base de datos");
                        this.DialogResult = false;
                    }
                }
                else
                {
                    MessageDialogResult result2 = await this.ShowMessageAsync("Informacion", "Tiene que elegir el codigo de la averia a modificar, si no desea modificar una averia haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
                    if (result2 == MessageDialogResult.Negative)
                    {
                        this.Close();
                    }
                }
            }else if (result == MessageDialogResult.FirstAuxiliary)
            {
                this.Close();
            }    
        }

        /// <summary>
        /// Gestor del boton de cancelar, cierra el dialogo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

        }
        /// <summary>
        /// Cuando se selecciona una averia,
        /// pone un booleano a true, para informar, 
        /// de que se a elegido una averia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboCodigo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            seleccionado = true;
            
        }
    }
}
