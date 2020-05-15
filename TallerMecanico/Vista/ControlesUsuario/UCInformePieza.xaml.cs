using CrystalDecisions.CrystalReports.Engine;
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
using TallerMecanico.Servicios;
using TallerMecanico.Modelo;
using CrystalDecisions.Shared;
using NLog;

namespace TallerMecanico.Vista.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para UCInformePieza.xaml
    /// </summary>
    public partial class UCInformePieza : UserControl
    {
        
        private ServicioSQL sqlServ;
        private ReportDocument rd;
        private Logger logger;

        /// <summary>
        /// Constructor del control de usuario
        /// </summary>
        /// <param name="servicioSql">Clase de servicio sql, nos permite obtener informacion mediante sentencias sql</param>
        /// <param name="rd">Documento de informe para el reporte</param>
        public UCInformePieza(ServicioSQL servicioSql,ReportDocument rd)
        {
            InitializeComponent();            
            this.sqlServ = servicioSql;
            this.rd = rd;
            logger = LogManager.GetCurrentClassLogger();

            cargaInforme();
            
        }
        /// <summary>
        /// Carga el informe y obtiene los 
        /// datos necesarios para el informe mediante una sentencia sql
        /// </summary>
        private void cargaInforme()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\InformePieza.rpt";
                rd.Load(path);                
                rd.SetDataSource(sqlServ.getDatos("select * from tallermecanico.pieza group by Tipo"));
                informeuc.ViewerCore.ReportSource = rd;

            }
            catch (Exception ex)
            {                
                logger.Error("Ha habido un problema al cargar y mostrar el informe de la piezas",ex);
            }
        }
    }
}
