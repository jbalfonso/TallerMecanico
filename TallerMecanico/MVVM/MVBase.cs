using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TallerMecanico.MVVM
{
    /// <summary>
    /// Contenido de la clase generica MVBase
    /// </summary>
    public class MVBase : INotifyPropertyChanged, IDataErrorInfo
    {

        #region PropertyChanged
        /// <summary>
        /// Gestiona el PropertyChangedEventHandler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T value, [CallerMemberName]string propertyName = "")
        {
            if (field == null || EqualityComparer<T>.Default.Equals(field, value)) { return false; }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        /// <summary>
        /// Gestiona el Onproperty changed de la propiedad
        /// </summary>
        /// <param name="propertyName"> nombre de la propiedad</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Error info
        /// <summary>
        /// Obtiene o establece el boton de guardar de los formularios
        /// </summary>
        public Button btnGuardar { get; set; }
        private int errorCount;

        /// check for general model error
        public string Error { get { return null; } }

        /// check for property errors
        public string this[string columnName]
        {
            get
            {
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                if (Validator.TryValidateProperty(
                        GetType().GetProperty(columnName).GetValue(this)
                        , new ValidationContext(this)
                        {
                            MemberName = columnName
                        }
                        , validationResults))
                    return null;


                return validationResults.First().ErrorMessage;
            }
        }


        
        /// <summary>
        /// Método que nos permite saber si hay errores en algún formulario
        /// En caso de que haya errores devolverá el valor de falso y en caso de que no haya devolverá verdadero
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Devuelve true si es valido</returns>
        public bool IsValid(DependencyObject obj)
        {
            // The dependency object is valid if it has no errors and all
            // of its children (that are dependency objects) are error-free.
            return !Validation.GetHasError(obj) &&
            LogicalTreeHelper.GetChildren(obj)
            .OfType<DependencyObject>()
            .All(IsValid);
        }

        
        /// <summary>
        /// Deshabilita el botón de guardar si hay errores en el formulario
        /// Si no hay errores habilitará el botón
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnErrorEvent(object sender, RoutedEventArgs e)
        {
            var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null)
                throw new Exception("Argumentos inesperados");
            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    {
                        errorCount++; break;
                    }
                case ValidationErrorEventAction.Removed:
                    {
                        errorCount--; break;
                    }
                default:
                    {
                        throw new Exception("Acción desconocida");
                    }
            }
            btnGuardar.IsEnabled = errorCount == 0;
        }

        #endregion
    }
}