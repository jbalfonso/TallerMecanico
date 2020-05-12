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
    /// Lógica de interacción para ModiciarAveria.xaml
    /// </summary>
    public partial class ModificarAveria :MetroWindow
    {
        
        private MVAveria mvaveria;
        private Logger logger;
        private bool seleccionado = false;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>        
        /// <param name="mvaveria">Clase de gestion de las averias</param>
        public ModificarAveria(MVAveria mvaveria)
        {
            InitializeComponent();            
            this.mvaveria = mvaveria;
            inicializar();
        }

        /// <summary>
        /// Inicializa los componentes ed la aplicaion
        /// </summary>
        private void inicializar()
        {            
            logger = LogManager.GetCurrentClassLogger();
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvaveria.OnErrorEvent));
            DataContext = mvaveria;
            mvaveria.btnGuardar = guardar;          
            DateResolucion.IsEnabled = false;
            txtResolucion.IsEnabled = false;
        }

        /// <summary>
        /// Gestor del botond de guardar, 
        /// valida si se a seleccionado una averia, 
        /// valida el dialogo, y modifica la averia, 
        /// tambien modifica el stock dependiendo de si se añaden o se quitan piezas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (seleccionado)
            {
                mvaveria.editar = true;
                if (mvaveria.IsValid(this))
                {
                    if (mvaveria.modificaStock())
                    {
                        if (mvaveria.guarda())
                        {
                            logger.Info("Averia nueva creada con codigo: " + mvaveria.averiaNueva.CodigoAveria);
                            this.DialogResult = true;
                        }
                        else
                        {
                            await this.ShowMessageAsync("Error", "Ha habido un error al insertar la averia en la base de datos");
                            logger.Error("Ha habido un error al modificar la averia en la base de datos");
                            this.DialogResult = false;
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error","Ha habido un error al modificar el stock de las piezas que contiene la averia");
                    }                   
                }
                else
                {
                    await this.ShowMessageAsync("Informacion", "Rellene los campos requeridos");
                }
            }
            else
            {
                MessageDialogResult result = await this.ShowMessageAsync("Informacion", "Tiene que elegir el codigo de la averia a modificar, si no desea modificar una averia haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Negative)
                {
                    this.Close();
                }
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
        /// Gestiona la seleccion de la averia a modificar, 
        /// pone un booleano a true, para comprobar si se ha seleccionado una averia, 
        /// guarda las piezas de la averia en una variable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboCodigo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            seleccionado = true;
            mvaveria.piezasModificaciones = mvaveria.averiaNueva.pieza.ToList();            
        }

        /// <summary>
        /// Gestiona el boton de añadir una pieza, añade la pieza a la averia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void anyadirPieza_Click(object sender, RoutedEventArgs e)
        {
            mvaveria.averiaNueva.pieza.Add(mvaveria.piezaSeleccionada);
            piezasAveriaCombo.Items.Refresh();
        }


        /// <summary>
        /// Gestiona el boton de eliminar una pieza,
        /// elimina una pieza de la averia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removePieza_Click(object sender, RoutedEventArgs e)
        {
            mvaveria.averiaNueva.pieza.Remove(mvaveria.piezaSeleccionada);
            piezasAveriaCombo.Items.Refresh();
        }

        /// <summary>
        /// Comprueba el estado seleccionado y 
        /// dependiendo del estado oculta o muestra elementos de la aplicacion
        /// </summary>
        private void compruebaEstado()
        {
            averia av2 = (averia)mvaveria.averiaNueva;
            if (av2.Estado=="En espera" || av2.Estado == "En proceso" || av2.Estado == "Pendiente")
            {
                DateResolucion.IsEnabled = false;
                txtResolucion.IsEnabled = false;
            }
            if (av2.Estado == "Finalizado" || av2.Estado == "Devuelto")
            {
                DateResolucion.IsEnabled = true;
                txtResolucion.IsEnabled = true;
            }
            
        }

        /// <summary>
        /// Gestiona el cambio de estado del combo de estado,
        /// llama a comprueba estado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            compruebaEstado();
        }

        /// <summary>
        /// Gestiona el tipo de texto introducido en el textbox, hace que solo se introduzcan numeros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }
    }
}
