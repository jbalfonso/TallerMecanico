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

namespace TallerMecanico.Vista.Dialogos.clientesDialogo
{
    /// <summary>
    /// Lógica de interacción para ModificarCliente.xaml
    /// </summary>
    public partial class ModificarCliente : MetroWindow
    {
        
        private MVCliente mvcliente;
        private Logger logger;
        private bool selecciona = false;

        /// <summary>
        /// Constructor del dialogo
        /// </summary>
        /// <param name="tent"></param>
        /// <param name="mvcliente"></param>
        public ModificarCliente(MVCliente mvcliente)
        {
            InitializeComponent();            
            this.mvcliente = mvcliente;

            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(mvcliente.OnErrorEvent));
            DataContext = mvcliente;
            mvcliente.btnGuardar = guardar;

            inicializa();
        }

        /// <summary>
        /// Inicializa componentes del dialogo
        /// </summary>
        private void inicializa()
        {
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Gestiona el boton de guardar, valida el documento,
        /// y comprueba que se haya seleccionado un cliente,
        /// despues lo edita
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (selecciona)
            {
                if (mvcliente.IsValid(this))
                {
                    mvcliente.editar = true;
                    if (mvcliente.guarda())
                    {
                        logger.Info("Cliente modificado con codigo: " + mvcliente.clienteNuevo.CodigoCliente);
                        this.DialogResult = true;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error","Ha habido un error al modificar el cliente en la base de datos");
                        logger.Error("Ha habido un error en la base de datos al modificar un Cliente");
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
                MessageDialogResult result2 = await this.ShowMessageAsync("Informacion", "Para continuar tiene que elegir el cliente a modificar, si no desea modificar un cliente haga clic en 'Cancel'", MessageDialogStyle.AffirmativeAndNegative);
                if (result2 == MessageDialogResult.Negative)
                {
                    this.Close();
                }
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
        /// Gestiona el cambio del empleado seleccionado,
        /// pone un booleano a true, para informar de que se ha seleccionado un empleado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selecciona = true;
        }
    }
}
