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
using TallerMecanico.MVVM;
using TallerMecanico.Modelo;
using NLog;
using CrystalDecisions.CrystalReports.Engine;

namespace TallerMecanico.Vista.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para UCDevolucion.xaml
    /// </summary>
    public partial class UCDevolucion : UserControl
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private ReportDocument rd;
        private MVFacturacion mvfactura;
        private int metodoPago;

        private const int tarjeta = 1;
        private const int efectivo = 2;
        private const int paypal = 3;

        /// <summary>
        /// Constructor del control de usuario
        /// </summary>
        /// <param name="rd">Report Document, documento del informe</param>
        /// <param name="mvfactura">Clase gestora de la facturacion</param>
        public UCDevolucion(ReportDocument rd, MVFacturacion mvfactura)
        {
            InitializeComponent();
            this.rd = rd;
            this.mvfactura = mvfactura;
            metodoPago = mvfactura.metodoPago;


            cargaInforme();
        }

        /// <summary>
        /// Gestor que carga el informe de la devolucion y le pasa los paramentros al informe,
        /// dependiendo del metodo de pago, pasa unos datos o otros
        /// </summary>
        private void cargaInforme()
        {
            int codigoaveria = mvfactura.averiaSeleccionada.CodigoAveria;
            try
            {
                
                rd.Load("../../informes/InformeDevolucion.rpt");
                rd.SetParameterValue("fechaDevolucion", mvfactura.fechaSeleccionada);
                rd.SetParameterValue("NombreEmpleado", mvfactura.empleadoSeleccionado.Nombre + " " + mvfactura.empleadoSeleccionado.Apellido);
                
                rd.SetParameterValue("nombreCliente", mvfactura.averiaSeleccionada.cliente1.Nombre + " " + mvfactura.clienteSeleccionado.Apellidos);
                
                rd.SetParameterValue("direccionCliente", mvfactura.averiaSeleccionada.cliente1.Direccion);
                rd.SetParameterValue("correoCliente", mvfactura.averiaSeleccionada.cliente1.Email);

                rd.SetParameterValue("codigoFactura", calculaCodigoAveria(codigoaveria));
                rd.SetParameterValue("descripcionAveria", mvfactura.averiaSeleccionada.Descripcion);

                if (!string.IsNullOrEmpty(mvfactura.averiaSeleccionada.Resolucion))
                {
                    rd.SetParameterValue("resolucionAveria", mvfactura.averiaSeleccionada.Resolucion);
                }


                rd.SetParameterValue("cantidadPiezas", calculaCantidadPiezas());
                rd.SetParameterValue("precioAveria", mvfactura.averiaSeleccionada.Precio);
                rd.SetParameterValue("totalAveria", mvfactura.averiaSeleccionada.Precio);

                if (metodoPago == tarjeta)
                {
                    string numero = mvfactura.tarjetaSeleccionada.numero;

                    numero = numero.Substring(numero.Length - 4);

                    rd.SetParameterValue("metodoDevolucion", "Tarjeta");
                    rd.SetParameterValue("campoDevolucion", "Numero tarjeta: XXXX XXXX XXXX " + numero);

                }
                else if (metodoPago == efectivo)
                {
                    rd.SetParameterValue("metodoDevolucion", "Efectivo");

                }
                else if (metodoPago == paypal)
                {
                    rd.SetParameterValue("metodoDevolucion", "Paypal");
                    rd.SetParameterValue("campoDevolucion", "Correo cargo: " + mvfactura.paypalSeleccionado.correo);
                }

                devolucionReport.ViewerCore.ReportSource = rd;
            }
            catch (Exception ex)
            {
                logger.Error("Ha habido un problema al cargar y mostrar el informe de la devolucion del cliente: " + mvfactura.averiaSeleccionada.cliente1.CodigoCliente);
            }
        }
        /// <summary>
        /// Gestor que formatea el codigo de la averia,
        /// que es el codigo del informe dependiendo, 
        /// de la longitud del codigo
        /// </summary>
        /// <param name="codigoaveria">El codigo de la averia a devolver</param>
        /// <returns>Devuelve el codigo de la averia formateado en string</returns>
        private string calculaCodigoAveria(int codigoaveria)
        {
            string codigoDevolucion = "";
            if (codigoaveria <= 9)
            {
                codigoDevolucion = "000" + codigoaveria;
            }
            else if (codigoaveria > 9 && codigoaveria <= 99)
            {
                codigoDevolucion = "00" + codigoaveria;
            }
            else if (codigoaveria > 99 && codigoaveria <= 999)
            {
                codigoDevolucion = "0" + codigoaveria;
            }
            else
            {
                codigoDevolucion = codigoaveria + "";
            }
            return codigoDevolucion;
        }
        /// <summary>
        /// Calcula la cantidad de piezas utilizadas en la averia,
        /// </summary>
        /// <returns>Devuelve la cantidad de piezas que hay en la averia</returns>
        private int calculaCantidadPiezas()
        {
            int cantidadPiezas = 0;
            foreach (pieza pz in mvfactura.averiaSeleccionada.pieza)
            {
                cantidadPiezas++;
            }
            return cantidadPiezas;
        }

    }
}
