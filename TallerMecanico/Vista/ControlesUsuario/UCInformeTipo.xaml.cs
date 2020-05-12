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
using TallerMecanico.Modelo;
using TallerMecanico.Servicios;

namespace TallerMecanico.Vista.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para UCInformeTipo.xaml
    /// </summary>
    public partial class UCInformeTipo : UserControl
    {
        private ServicioSQL serviciosql;

        public UCInformeTipo(ServicioSQL serviciosql)
        {
            InitializeComponent();
            this.serviciosql = serviciosql;
            cargaInforme();
        }
        private void cargaInforme()
        {

        }
        
    }
}
