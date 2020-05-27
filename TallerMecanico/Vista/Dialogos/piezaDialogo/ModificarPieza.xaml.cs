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
    /// Lógica de interacción para ModificarPieza.xaml
    /// </summary>
    public partial class ModificarPieza : MetroWindow
    {
        private bool seleccionado = false;
        private pieza piezaModificar;
        private MVPieza mvpieza;
        private Logger logger;
        private Action<pieza> gestionaNotificacionPieza;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvpieza">Clase que gestiona las piezas</param>
        /// <param name="gestionaNotificacionPieza">Metodo de la clase Mainwindow, gestiona las notificaciones de las piezas</param>
        public ModificarPieza(MVPieza mvpieza,Action<pieza>gestionaNotificacionPieza)
        {
            InitializeComponent();
            this.mvpieza = mvpieza;
            this.gestionaNotificacionPieza = gestionaNotificacionPieza;
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvpieza.OnErrorEvent));
            DataContext = mvpieza;
            mvpieza.btnGuardar = guardar;

            inicializa();
        }
        /// <summary>
        /// Inicializa los componentes de la aplicacion
        /// </summary>
        private void inicializa()
        {
            logger = LogManager.GetCurrentClassLogger();
            piezaModificar = new pieza();
        }

        /// <summary>
        /// Gestiona el boton de guardar, 
        /// comprueba si se ha seleccionado una pieza, 
        /// y valida el dialogo, despues edita la pieza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (seleccionado)
                {                   
                        piezaModificar.Tipo = tipo.Text;
                        piezaModificar.Cantidad = int.Parse(cantidad.Text);
                        piezaModificar.Descripcion = descripcion.Text;
                        
                        if (mvpieza.modificaPieza(piezaModificar))
                        {
                            logger.Info("Pieza modificada con codigo: " + piezaModificar.CodigoPieza);
                            gestionaNotificacionPieza(piezaModificar);
                            this.DialogResult = true;
                        }
                        else
                        {
                            logger.Error("Ha habido un error en la base de datos al modificar una pieza");
                            await this.ShowMessageAsync("Error", "Ha habido un error al modificar la pieza en la base de datos");
                            this.DialogResult = false;
                        }                  
                }
                else
                {
                    MessageDialogResult result2 = await this.ShowMessageAsync("Informacion", "Para continuar tiene que elegir la pieza a modificar, si no desea modificar una pieza haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
                    if (result2 == MessageDialogResult.Negative)
                    {
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error","Ha habido un error en el dialogo al guardar la pieza");
                logger.Error("Ha habido un error en el dialogo al guardar la pieza", ex);
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
        /// Gestiona el cambio de seleccion de la pieza, pone un booleano a true, para informar de que se ha seleccionado una pieza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboPieza_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           piezaModificar = (pieza)comboPieza.SelectedItem;
            seleccionado = true;           
            tipo.Text = piezaModificar.Tipo;
            cantidad.Text = piezaModificar.Cantidad+"";
            descripcion.Text = piezaModificar.Descripcion;            
        }

        /// <summary>
        /// Gestiona el texto introducido en el textbox, hace que solo se introduzcan numeros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
