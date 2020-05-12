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
using TallerMecanico.informes;
using TallerMecanico.Servicios;
using CrystalDecisions.CrystalReports.Engine;

using NLog;
using System.Data;
using CrystalDecisions.Shared;

namespace TallerMecanico.Vista.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para UCFactura.xaml
    /// </summary>
    public partial class UCFactura : UserControl
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
        public UCFactura(ReportDocument rd, MVFacturacion mvfactura)
        {
            InitializeComponent();           
            this.rd = rd;
            this.mvfactura = mvfactura;
            metodoPago = mvfactura.metodoPago;
            
            cargaInforme();
        }

      
        /// <summary>
        /// Carga el informe y le pasa los parametros dependiendo del metodo de pago elegido
        /// </summary>
        private void cargaInforme()
        {
            int cantidadPiezas = 0;
            string codigoFactura = "";
            int codigoaveria = mvfactura.averiaSeleccionadas[0].CodigoAveria;
            try
            {

                cantidadPiezas = calculaCantidadPiezas();
                codigoFactura = calculaCodigoAveria(codigoaveria);
               
                rd.Load("../../informes/InformeFactura.rpt");         

                rd.SetParameterValue("fechaCobro", mvfactura.fechaSeleccionada);
                rd.SetParameterValue("NombreEmpleado",mvfactura.empleadoSeleccionado.Nombre+" "+mvfactura.empleadoSeleccionado.Apellido);                
                rd.SetParameterValue("nombreCliente",mvfactura.clienteSeleccionado.Nombre+" "+mvfactura.clienteSeleccionado.Apellidos);                
                rd.SetParameterValue("direccionCliente",mvfactura.clienteSeleccionado.Direccion);
                rd.SetParameterValue("correoCliente",mvfactura.clienteSeleccionado.Email);                
                rd.SetParameterValue("codigoFactura",codigoFactura);
                rd.SetParameterValue("descripcionAveria", mvfactura.averiaSeleccionadas[0].Descripcion);

                if (!string.IsNullOrEmpty(mvfactura.averiaSeleccionadas[0].Resolucion))
                {
                    rd.SetParameterValue("resolucionAveria", mvfactura.averiaSeleccionadas[0].Resolucion);
                }
                

                rd.SetParameterValue("cantidadPiezas",cantidadPiezas);
                rd.SetParameterValue("precioAveria",mvfactura.averiaSeleccionadas[0].Precio);
                rd.SetParameterValue("totalAveria",mvfactura.averiaSeleccionadas[0].Precio);

                if (metodoPago == tarjeta)
                {
                    string numero = mvfactura.tarjetaSeleccionada.numero;
                    
                    numero=numero.Substring(numero.Length - 4);
                   
                    rd.SetParameterValue("metodoPago","Tarjeta");
                    rd.SetParameterValue("campoPago1","Numero tarjeta: XXXX XXXX XXXX "+numero);
                    
                }else if (metodoPago==efectivo)
                {
                    rd.SetParameterValue("metodoPago","Efectivo");

                }else if (metodoPago==paypal)
                {
                    rd.SetParameterValue("metodoPago","Paypal");
                    rd.SetParameterValue("campoPago1","Correo cargo: "+mvfactura.paypalSeleccionado.correo);
                }             
                facturaReport.ViewerCore.ReportSource = rd;
            }
            catch (Exception ex)
            {
                logger.Error("Ha habido un problema al cargar y mostrar el informe de la factura del cliente: "+mvfactura.clienteSeleccionado.CodigoCliente);
            }
        }

        /// <summary>
        /// Formatea el codigo de la averia, segun la longitud que tenga
        /// </summary>
        /// <param name="codigoaveria">Devuelve el codigo de averia formateado en string</param>
        /// <returns></returns>
        private string calculaCodigoAveria(int codigoaveria)
        {
            string codigoFactura = "";
            if (codigoaveria <= 9)
            {
                codigoFactura = "000" + codigoaveria;
            }
            else if (codigoaveria > 9 && codigoaveria <= 99)
            {
                codigoFactura = "00" + codigoaveria;
            }
            else if (codigoaveria > 99 && codigoaveria <= 999)
            {
                codigoFactura = "0" + codigoaveria;
            }
            else
            {
                codigoFactura = codigoaveria + "";
            }
            return codigoFactura;
        }
        /// <summary>
        /// calcula la cantidad de piezas que se han utilizado en la averia
        /// </summary>
        /// <returns>Devuelve la cantidad de piezas</returns>
        private int calculaCantidadPiezas()
        {
            int cantidadPiezas = 0;
            foreach (pieza pz in mvfactura.averiaSeleccionadas[0].pieza)
            {
                cantidadPiezas++;
            }
            return cantidadPiezas;
        }
    }
}
