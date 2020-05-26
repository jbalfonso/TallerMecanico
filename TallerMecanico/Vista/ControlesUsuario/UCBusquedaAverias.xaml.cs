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
using TallerMecanico.Vista.Dialogos.averiaDialogo;
using TallerMecanico.Vista.Dialogos.piezaDialogo;



namespace TallerMecanico.Vista.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para UCBusquedaAverias.xaml
    /// </summary>
    public partial class UCBusquedaAverias : UserControl
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private tallermecanicoEntities tEnt;
        private MVAveria mvaveria;
        private List<int> permisosUsuarioLogeado;
        private const int perm_anyadirAveria = 1;
        private const int perm_resolverAveria = 2;
        private const int perm_anularAveria = 3;
        private Action<averia> gestionaStockAveriaModificar;

        /// <summary>
        /// Constructor del control de usuario
        /// </summary>
        /// <param name="tEnt">entidad de la base de datos del taller mecanico</param>
        /// <param name="mvaveria">Clase de gestion MVAveria</param>
        /// <param name="permisosUsuario">Lista de codigos de permisos del usuario logeado</param>
        /// <param name="gestionaStockAveriaModificar">Metodo de la clase MainWindow que gestiona el stock de las piezas</param>
        public UCBusquedaAverias(tallermecanicoEntities tEnt,MVAveria mvaveria, List<int> permisosUsuario,Action<averia> gestionaStockAveriaModificar)
        {
            InitializeComponent();
            this.tEnt = tEnt;
            this.mvaveria = mvaveria;
            this.gestionaStockAveriaModificar = gestionaStockAveriaModificar;
            this.permisosUsuarioLogeado = permisosUsuario;

            DataContext = mvaveria;

            comprobarPermisos();
          
        }
       
        /// <summary>
        /// Comprueba los permisos del usuario logeado,
        /// dependiendo de los permisos que tenga, 
        /// se ocultan o se muestran elementos de la aplicacion
        /// </summary>
        private void comprobarPermisos()
        {
            if(!permisosUsuarioLogeado.Contains(perm_resolverAveria) && !permisosUsuarioLogeado.Contains(perm_anularAveria) && !permisosUsuarioLogeado.Contains(perm_anyadirAveria))
            {
                menuClickDerecho.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (!permisosUsuarioLogeado.Contains(perm_anyadirAveria))
                {
                    AnadirMenu.Visibility = Visibility.Collapsed;
                }
                if (!permisosUsuarioLogeado.Contains(perm_resolverAveria))
                {
                    resolucionMenu.Visibility = Visibility.Collapsed;
                }
                if (!permisosUsuarioLogeado.Contains(perm_anularAveria))
                {
                    eliminarMenu.Visibility = Visibility.Collapsed;
                }
                if(permisosUsuarioLogeado.Contains(perm_anyadirAveria) && permisosUsuarioLogeado.Contains(perm_anularAveria))
                {
                    editarMenu.Visibility = Visibility.Visible;
                }
            }
            
        }

        
        /// <summary>
        /// Gestor para el boton de filtrar, filtra segun los filtros introducidos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filtrar_Click(object sender, RoutedEventArgs e)
        {
            try { 
            bool enc = true;
            mvaveria.criteriosAveria.Clear();
            if(mvaveria.fechaInicial!=null && mvaveria.fechaFinal != null)
            {
                if (mvaveria.fechaInicial > mvaveria.fechaFinal)
                {
                    MessageBox.Show("Seleccione la fecha inicial que sea menor que la fecha final", "Informacion");
                    enc = false;
                }
                else
                {
                    mvaveria.criteriosAveria.Add(new Predicate<averia>(d => d.FechaRecepcion > fechaInicial.SelectedDate && d.FechaRecepcion < fechaFinal.SelectedDate));
                }
            }
            if (!string.IsNullOrEmpty(mvaveria.txt_filtroEstado))
            {
                mvaveria.criteriosAveria.Add(new Predicate<averia>(d => d.Estado.ToUpper().StartsWith(mvaveria.txt_filtroEstado.ToUpper())));
            }
            if (!string.IsNullOrEmpty(mvaveria.txt_filtroTipo))
            {
                mvaveria.criteriosAveria.Add(new Predicate<averia>(d => (!string.IsNullOrEmpty(d.Tipo)) && d.Tipo.ToUpper().StartsWith(mvaveria.txt_filtroTipo.ToUpper())));

            }
            if (enc)
            {
                dgListaAverias.Items.Filter = mvaveria.predicadoFiltro;
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error","Ha habido un problema al filtrar las averias",MessageBoxButton.OK,MessageBoxImage.Error);
                logger.Error("Ha habido un error desconocido al filtrar las averias obtenidas de la base de datos",ex);
            }
        }
        /// <summary>
        /// Gestiona el boton quitar filtro, 
        /// quita el filtro del datagrid, 
        /// y limpia los filtros introducidos, y los inicializa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitarFiltroBtn_Click(object sender, RoutedEventArgs e)
        {
            dgListaAverias.Items.Filter = null;
            mvaveria.criteriosAveria.Clear();
            text_filtroTipo.Text = null;
            comboEstado.SelectedItem = null;
            fechaFinal.SelectedDate = mvaveria.fechaMaxima;
            fechaInicial.SelectedDate = mvaveria.fechaMinima;
        }

        /// <summary>
        /// Gestiona el dialogo de editar averia, muestra el dialogo con los datos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditarMenu_Click(object sender, RoutedEventArgs e)
        {
            if (dgListaAverias.SelectedItem != null)
            {
                ModificarAveria diag = new ModificarAveria(mvaveria,this.gestionaStockAveriaModificar);
                diag.ShowDialog();

                if (diag.DialogResult == true)
                {
                    dgListaAverias.Items.Refresh();
                    MessageBox.Show("Averia editada correctamente","Informacion");                   
                }
                
            }
        }
        /// <summary>
        /// Gestiona el menu de añadir una averia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnadirMenu_Click(object sender, RoutedEventArgs e)
        {
            AddAveria diag = new AddAveria(mvaveria);
            mvaveria.averiaNueva = new averia();
            diag.ShowDialog();
            if (diag.DialogResult == true)
            {
                dgListaAverias.Items.Refresh();
                MessageBox.Show("Averia añadida correctamente","Informacion");              
            }
        }

        /// <summary>
        /// Gestiona el boton de eliminar, elimina la averia seleccionada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarMenu_Click(object sender, RoutedEventArgs e)
        {
            if (dgListaAverias.SelectedItem is averia)
            {
                mvaveria.averiaNueva = (averia)dgListaAverias.SelectedItem;
                MessageBoxResult result = MessageBox.Show("Este seguro que desea eliminar la averia", "Confirmacion", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (mvaveria.reduceStock())
                    {
                        if (mvaveria.anula())
                        {
                            dgListaAverias.Items.Refresh();
                            MessageBox.Show("Averia anulada", "Informacion", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Ha habido un error al anular la averia en la base de datos", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("ha habido un error al reducir el stock de la averia", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }                
            }
            else
            {
                MessageBox.Show("Debe seleccionar una averia, no una pieza", "Informacion");
            }
           
        }

        private void ResolucionMenu_Click(object sender, RoutedEventArgs e)
        {
            if (dgListaAverias.SelectedItem != null)
            {
                InterfazResolucion diag = new InterfazResolucion(mvaveria);
                diag.ShowDialog();

                if (diag.DialogResult == true)
                {
                    dgListaAverias.Items.Refresh();
                    MessageBox.Show("Resolucion añadida correctamente", "Informacion",MessageBoxButton.OK,MessageBoxImage.Information);
                }

            }
        }
    }
}
