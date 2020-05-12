using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TallerMecanico.MVVM
{
    /// <summary>
    /// Clase de objeto de la tarjeta de credito
    /// </summary>
    public class Tarjeta
    {
        /// <summary>
        /// texto del numero de la tarjeta de credito
        /// </summary>
        public string numero { get; set; }

        /// <summary>
        /// Texto de la fecha de caducidad de la tarjeta de credito
        /// </summary>
        public string caducidad { get; set; }

        /// <summary>
        /// Texto del codigo de seguridad de la tarjeta de credito
        /// </summary>
        public string ccv { get; set; }


        /// <summary>
        /// Constructor de la tarjeta
        /// </summary>
        /// <param name="numeroTarjeta">Texto con el numero de tarjeta</param>
        /// <param name="caducidad">Texto con la fecha de caducidad</param>
        /// <param name="ccv">Texto con el codigo de seguridad</param>
        public Tarjeta(string numeroTarjeta,string caducidad,string ccv)
        {
            this.numero = numeroTarjeta;
            this.caducidad = caducidad;
            this.ccv = ccv;
        }

    }
}
