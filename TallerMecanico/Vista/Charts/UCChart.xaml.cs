using LiveCharts;
using LiveCharts.Wpf;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TallerMecanico.Modelo;
using TallerMecanico.MVVM;

namespace TallerMecanico.Vista.Charts
{
    /// <summary>
    /// Lógica de interacción para UCChart.xaml
    /// </summary>
    public partial class UCChart : UserControl
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private tallermecanicoEntities ent;
        private MVAveria mvaveria;
        private List<averia> listadoAverias;     
        private ChartValues<double> columnas = new ChartValues<double>();
        private List<string> meses = new List<string>();
        private Dictionary<int, double> valoresMes = new Dictionary<int, double>();

        /// <summary>
        /// Constructor del control de usuario del chart
        /// </summary>
        /// <param name="tent">entidad de la base de datos taller mecanico</param>
        /// <param name="mvaveria">Clase gestora MVAveria</param>
        public UCChart(tallermecanicoEntities tent,MVAveria mvaveria)
        {
            InitializeComponent();
            this.ent = tent;
            this.mvaveria = mvaveria;
            
            inicializar();

        }
        /// <summary>
        /// Inicializa, los atributos de la clase, 
        /// y prepara los datos para ser mostrados en el chart,
        /// y los muestra en el chart
        /// </summary>
        private void inicializar()
        {
            try
            {
                cargaMeses();
                cargaValores();
                listadoAverias = mvaveria.listaAverias;

                foreach (averia av in listadoAverias)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        if (i == 2)
                        {
                            if (av.FechaResolucion >= new DateTime(DateTime.Now.Year, i, 1) && av.FechaResolucion <= new DateTime(DateTime.Now.Year, i, 29))
                            {
                                valoresMes[i]++;
                            }
                        }
                        else
                        {
                            if (av.FechaResolucion >= new DateTime(DateTime.Now.Year, i, 1) && av.FechaResolucion <= new DateTime(DateTime.Now.Year, i, 30))
                            {
                                valoresMes[i]++;
                            }
                        }
                    }
                }

                foreach (KeyValuePair<int, double> t in valoresMes)
                {
                    columnas.Add(t.Value);
                }

                SeriesCollection serie = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title="Reparaciones",
                        Values = columnas,
                        DataLabels=true,
                        Fill=Brushes.Blue,

                    }
                };
                Axis eje = new Axis
                {
                    Title = "Meses",
                    Labels = meses

                };
                lvReparaciones.Series = serie;
                lvReparaciones.AxisX.Add(eje);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha habido un problema al inicializar el grafico","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                logger.Error("Ha habido un problema al inicializar el grafico, error al inicializar la clase UCChart",ex);
                
            }
        }
        /// <summary>
        /// Carga los valores iniciales para el diccionario de los meses
        /// </summary>
        private void cargaValores()
        {
            valoresMes.Add(1,0);
            valoresMes.Add(2, 0);
            valoresMes.Add(3, 0);
            valoresMes.Add(4, 0);
            valoresMes.Add(5, 0);
            valoresMes.Add(6, 0);
            valoresMes.Add(7, 0);
            valoresMes.Add(8, 0);
            valoresMes.Add(9, 0);
            valoresMes.Add(10, 0);
            valoresMes.Add(11, 0);
            valoresMes.Add(12, 0);
        }      
      
       /// <summary>
       /// Carga los valores de los nombres de los meses a mostrar
       /// </summary>
        private void cargaMeses()
        {
            meses.Add("Enero");
            meses.Add("Febrero");
            meses.Add("Marzo");
            meses.Add("Abril");
            meses.Add("Mayo");
            meses.Add("Junio");
            meses.Add("Julio");
            meses.Add("Agosto");
            meses.Add("Septiembre");
            meses.Add("Octubre");
            meses.Add("Noviembre");
            meses.Add("Diciembre");
        }
    }
}
