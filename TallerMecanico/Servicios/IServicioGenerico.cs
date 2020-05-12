using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TallerMecanico.Servicios
{
    interface IServicioGenerico<T> where T : class
    {
        
        /// <summary>
        /// Obtiene todos los objetos de una determinada entidad
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> getAll();

        /// <summary>
        /// Busca elementos según la expresión o predicado pasado como parámetro
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> findBy(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Busca un objeto por su identificador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T findByID(int id);
      
        /// <summary>
        /// Inserta un objeto nuevo en la base de datos
        /// Luego se debe de realizar un commit para que se hagan persistentes los cambios
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T add(T entity);
        
        /// <summary>
        /// Borra un objeto de la base de datos en función de su id
        /// Luego se debe de realizar un commit para que se hagan persistentes los cambios
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T delete(T entity);
       
        /// <summary>
        /// Edita un objeto de la base de datos
        /// Luego se debe de realizar un commit para que se hagan persistentes los cambios
        /// </summary>
        /// <param name="entity"></param>
        void edit(T entity);
        
        /// <summary>
        ///  Realiza un commit para que los cambios se hagan persistentes
        /// </summary>
        void save();
    }
}
