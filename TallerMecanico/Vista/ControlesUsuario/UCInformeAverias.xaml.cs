using CrystalDecisions.CrystalReports.Engine;
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
using TallerMecanico.Servicios;

namespace TallerMecanico.Vista.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para UCInformeAverias.xaml
    /// </summary>
    public partial class UCInformeAverias : UserControl
    {

        private ReportDocument rd;
        private ServicioSQL sqlServ;
        private Logger logger;     

        /// <summary>
        /// Constructor del control de usuario
        /// </summary>
        /// <param name="sqlServ">Clase de servicio sql, nos permite obtener informacion mediante sentencias sql</param>
        /// <param name="rd">Documento de informe para el reporte</param>
        public UCInformeAverias(ServicioSQL sqlServ, ReportDocument rd)
        {
            InitializeComponent();
            this.sqlServ = sqlServ;
            this.rd = rd;
            logger = LogManager.GetCurrentClassLogger();
            
            cargaInforme();
           
        }

        /// <summary>
        /// Carga el informe y obtiene los datos 
        /// necesarios para el informe mediante una sentencia sql
        /// </summary>
        private void cargaInforme()
        {
            try
            {

                rd.Load("../../informes/InformeAveria.rpt");
                rd.SetDataSource(sqlServ.getDatos("select averia.CodigoAveria,averia.Descripcion,averia.Precio,averia.Estado,empleado.Nombre,averia.Resolucion," +
                    "averia.FechaRecepcion,averia.FechaResolucion,cliente.Nombre,averia.Observaciones,pieza.Descripcion from averia " +
                    "join cliente on cliente.CodigoCliente = averia.Cliente join empleado on empleado.CodigoEmpleado = averia.EmpleadoAsignado " +
                    "join tiene on tiene.Averias_CodigoAveria = averia.CodigoAveria join pieza on pieza.CodigoPieza = tiene.Piezas_CodigoPieza order by averia.CodigoAveria "));
                
                informeAveria.ViewerCore.ReportSource = rd;

            }
            catch (Exception ex)
            {
                logger.Error("Ha habido un problema al cargar y mostrar el informe de las averias", ex);
            }
        }
    }
}
