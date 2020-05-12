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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TallerMecanico.Vista.Dialogos
{
    /// <summary>
    /// Lógica de interacción para UCInicio.xaml
    /// </summary>
    public partial class UCInicio : UserControl
    {
        /// <summary>
        /// Constructor del control de usuario
        /// </summary>
        public UCInicio()
        {
            InitializeComponent();
            hora.Content = DateTime.Now.ToLongTimeString();

          
        }
        /// <summary>
        /// Carga el reloj y la fecha del sistema, mediante un timer que esta continuamente comprobandolo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load(object sender,RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer(DispatcherPriority.Send);
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();

            DateTime date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            string dateString = date.ToString(@"d/M/yyyy");
            fecha.Content = dateString;
        }
        /// <summary>
        /// Aplica la hora formateada cada vez que es llamado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            hora.Content = DateTime.Now.ToString("HH:mm:ss");
        }

    
    }
}
