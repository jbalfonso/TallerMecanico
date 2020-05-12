using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallerMecanico.MVVM
{
    /// <summary>
    /// Objeto de cuenta paypal el cual contiene el correo de la cuenta 
    /// y la contraseña de la cuenta
    /// </summary>
   public class Paypal
    {
        /// <summary>
        /// Texto del correo de la cuenta de paypal
        /// </summary>
        public string correo { get; set; }

        /// <summary>
        /// Texto de la contraseña de la cuenta de paypal
        /// </summary>
        public string contrasenya { get; set; }


        /// <summary>
        /// Constructor de la cuenta de paypal
        /// </summary>
        /// <param name="correo">es el correo de la cuenta de paypal</param>
        /// <param name="contrasenya">es la contraseña de la cuenta de paypal</param>
        public Paypal(string correo,string contrasenya)
        {
            this.contrasenya = contrasenya;
            this.correo = correo;
        }
       
    }
}
