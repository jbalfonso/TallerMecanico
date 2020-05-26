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
    /// Lógica de interacción para AddPieza.xaml
    /// </summary>
    public partial class AddPieza : MetroWindow
    {
        
        private MVPieza mvpieza;
        Logger logger;       

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="mvpieza">Clase que gestiona las piezas</param>
        public AddPieza(MVPieza mvpieza)
        {
            InitializeComponent();            
            this.mvpieza = mvpieza;            
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvpieza.OnErrorEvent));
            DataContext = mvpieza;
            mvpieza.btnGuardar = guardar;
            inicializa();
        }

        

        /// <summary>
        /// Inicializa los componentes del dialogo
        /// </summary>
        private void inicializa()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Gestiona el boton de guardar, valida el dialogo y guarda la pieza en la base de datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (mvpieza.IsValid(this))
            {
                if (mvpieza.guarda())
                {
                    logger.Info("Pieza nueva añadida con codigo: " + mvpieza.piezaNueva.CodigoPieza);
                    
                    this.DialogResult = true;
                }
                else
                {
                    logger.Error("Ha habido un error en la base de datos al añadir una pieza nueva");
                    await this.ShowMessageAsync("Error","Ha habido un error al guardar la pieza en la base de datos");
                    this.DialogResult = false;
                }
            }
            else
            {
                await this.ShowMessageAsync("Informacion","Rellene todos los campos");
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
        /// Gestiona el texto que se introduce en el textbox, hace que solo se puedan introducir numeros
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
