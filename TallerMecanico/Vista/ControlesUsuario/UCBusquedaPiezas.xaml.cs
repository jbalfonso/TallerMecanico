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
using TallerMecanico.Vista.Dialogos;
using TallerMecanico.Vista.Dialogos.piezaDialogo;

namespace TallerMecanico.Vista.ControlesUsuario
{
    /// <summary>
    /// Lógica de interacción para UCBusquedaPiezas.xaml
    /// </summary>
    public partial class UCBusquedaPiezas : UserControl
    {
        private tallermecanicoEntities tEnt;
        private List<int> permisosUsuarioLogeado = new List<int>();
        private MVPieza mvpieza;
        private const int perm_anyadirPieza = 6;
        private const int perm_modificarEliminarPieza = 7;

        /// <summary>
        /// Constructor del control de usuario 
        /// </summary>
        /// <param name="ent">entidad de la base de datos taller mecanico</param>
        /// <param name="mvpieza">Clase de gestion de las piezas</param>
        /// <param name="permisos">Lista de permisos del usuario logeado</param>
        public UCBusquedaPiezas(tallermecanicoEntities ent, MVPieza mvpieza, List<int> permisos)
        {
            InitializeComponent();
            this.tEnt = ent;
            this.mvpieza = mvpieza;
            this.permisosUsuarioLogeado = permisos;

            DataContext = mvpieza;

            comprobarPermisos();
           
        }
      
        /// <summary>
        /// Compruebo los permisos del usuario logeado,
        /// dependiendo de los permisos que tenga se ocultan o se muestran elementos de la aplicacion
        /// </summary>
        private void comprobarPermisos()
        {
            if (!permisosUsuarioLogeado.Contains(perm_anyadirPieza) && !permisosUsuarioLogeado.Contains(perm_modificarEliminarPieza))
            {
                menuClickDerecho.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (!permisosUsuarioLogeado.Contains(perm_anyadirPieza))
                {
                    AnadirMenu.Visibility = Visibility.Collapsed;
                }
                if (!permisosUsuarioLogeado.Contains(perm_modificarEliminarPieza))
                {
                    editarMenu.Visibility = Visibility.Collapsed;
                    eliminarMenu.Visibility = Visibility.Collapsed;
                }
                
            }

        }

        /// <summary>
        /// Gestor del boton de filtrar, 
        /// el cual aplica los filtros seleccionados, 
        /// segun si estan vacios o no estan seleccionados algunos filtros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filtrar_Click(object sender, RoutedEventArgs e)
        {
            
            mvpieza.criteriosPieza.Clear();
            if (comboCodigoPieza.SelectedItem !=null || mvpieza.txt_filtroCodigoPieza!=0)
            {
                mvpieza.criteriosPieza.Add(new Predicate<pieza>(p => p.CodigoPieza>=(int)comboCodigoPieza.SelectedItem && comboCodigoPieza.SelectedItem!=null));
            }
            if (!string.IsNullOrEmpty(mvpieza.txt_filtroDescripcion))
            {
                mvpieza.criteriosPieza.Add(new Predicate<pieza>(p=> p.Descripcion.ToUpper().StartsWith(mvpieza.txt_filtroDescripcion.ToUpper())));
            }
            dgListaPiezas.Items.Filter = mvpieza.predicadoFiltro;
           
        }

        /// <summary>
        /// Gestor del boton de quitar el filtro, 
        /// deselecciona los filtros aplicados y
        /// quita los filtros del datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitarFiltroBtn_Click(object sender, RoutedEventArgs e)
        {
            dgListaPiezas.Items.Filter = null;
            mvpieza.criteriosPieza.Clear();
            filtroDescripcion.Text = null;
            comboCodigoPieza.SelectedItem = null;

        }

        /// <summary>
        /// Gestor para el boton de editar pieza, 
        /// muestra el dialogo, y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditarMenu_Click(object sender, RoutedEventArgs e)
        {
            if (dgListaPiezas.SelectedItem != null)
            {
                ModificarPieza diag = new ModificarPieza(mvpieza);
                diag.ShowDialog();

                if (diag.DialogResult == true)
                {
                    dgListaPiezas.Items.Refresh();
                    MessageBox.Show("Pieza editada correctamente","Informacion");
                }
            }
        }

        /// <summary>
        /// Gestor del boton de añadir una pieza, 
        /// muestra el dialogo y luego muestra un mensaje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnadirMenu_Click(object sender, RoutedEventArgs e)
        {
            AddPieza diag = new AddPieza(mvpieza);
            diag.ShowDialog();
            if (diag.DialogResult==true)
            {
                dgListaPiezas.Items.Refresh();
                MessageBox.Show("Pieza añadida correctamente","Informacion");
            }
        }

        /// <summary>
        /// Gestor del boton de eliminar una pieza, 
        /// elimina la pieza y dependiendo 
        /// de si la ha borrado correctamente o no muestra un mensaje o otro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EliminarMenu_Click(object sender, RoutedEventArgs e)
        {
            if (dgListaPiezas.SelectedItem is pieza)
            {
                mvpieza.piezaNueva = (pieza)dgListaPiezas.SelectedItem;
                MessageBoxResult result = MessageBox.Show("Esta seguro que desea eliminar la pieza seleccionada?","Confirmacion",MessageBoxButton.YesNo,MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {                
                    if (mvpieza.borra())
                    {
                        dgListaPiezas.Items.Refresh();
                        MessageBox.Show("Pieza eliminada correctamente", "Informacion",MessageBoxButton.OK,MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Ha habido un error al borrar la pieza en la base de datos", "Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    }
                }

            }
            else
            {
                MessageBox.Show("Debe seleccionar una pieza, no una averia", "Informacion",MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
        }
    }
}
