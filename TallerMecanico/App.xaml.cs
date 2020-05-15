using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TallerMecanico
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Metodo que se ejecuta al inicio de la aplicacion, antes de cargar cualquier dialogo
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!AplicacionEjecutandose()) {              
            base.OnStartup(e);
            }
            else
            {
                MessageBox.Show("Ya hay una instancia de la aplicacion ejecutandose");
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Comprueba cuantas instancias de la aplicacion hay ejecutandose, 
        /// si hay mas de una devuelve true si no devuelve false
        /// </summary>
        /// <returns>Devuelve true si ya hay mas de 1 aplicacion, devuelve false si no hay mas de 1</returns>
        private bool AplicacionEjecutandose()
        {
            Boolean Flag = false;
            try
            {
                Process[] ProcessObj = null; ;
                String ModualName = Process.GetCurrentProcess().MainModule.ModuleName;
                String ProcessName =Path.GetFileNameWithoutExtension(ModualName);              

                // Obtiene todas las intancias del proceso del programa, ejecutándose en el equipo
                ProcessObj = Process.GetProcessesByName(ProcessName);

                if (ProcessObj.Length > 1) // si ya hay una aplicación, devolvemos true
                {
                    Flag = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Flag;
        }
        
    }
}
