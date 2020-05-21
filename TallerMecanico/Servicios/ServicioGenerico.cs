using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace TallerMecanico.Servicios
{
    
      /// <summary>
      /// Clase que implementa la interfaz de acceso a datos
      /// </summary>
      /// <typeparam name="T"></typeparam>
    public class ServicioGenerico<T> : IServicioGenerico<T>
        where T : class
    {
        
        /// <summary>
        /// Objeto que accede a la capa de acceso a datos creada por Entity Framework
        /// </summary>
        protected DbContext _entities;

        
        /// <summary>
        /// Objeto que nos permite acceder a las clases asociadas con las tablas de la base de datos
        /// </summary>
        protected readonly IDbSet<T> _dbset;
        
         /// <summary>
         /// Constructor del servicio generico
         /// </summary>
         /// <param name="context">Contexto de la base de datos</param>
        public ServicioGenerico(DbContext context)
        {
            _entities = context;
            _dbset = context.Set<T>();
        }

        
         /// <summary>
         /// Añade la entidad a la base de datos
         /// Necesita de un commit para hacer la transaccion persistente
         /// </summary>
         /// <param name="entity">entidad de base de datos de tipo generico</param>
         /// <returns></returns>
        public T add(T entity)
        {
            return _dbset.Add(entity);
        }
        
         /// <summary>
         /// Borra la entidad a la base de datos
         /// Necesita de un commit para hacer la transaccion persistente, borra la entidad
         /// </summary>
         /// <param name="entity">entidad de la base de datos de tipo generico</param>
         /// <returns></returns>
        public T delete(T entity)
        {
            return _dbset.Remove(entity);
        }
        
         /// <summary>
         /// Devuelve una lista con todos los objetos de una tabla de la base de datos
         /// </summary>
         /// <returns></returns>
        public IEnumerable<T> getAll()
        {
            return _dbset.AsEnumerable<T>();
        }
        
         /// <summary>
         /// Realiza un commit de la cache a la base de datos
         /// </summary>
        public void save()
        {
            _entities.SaveChanges();
        }
       
         /// <summary>
         /// Devuelve un objeto identificado por su id
         /// </summary>
         /// <param name="id">identificador</param>
         /// <returns>Devuelve el objeto que se a buscado por su id</returns>
        public T findByID(int id)
        {
            return _dbset.Find(id);
        }
        
         /// <summary>
         /// Edita la entidad en la base de datos
         /// Necesita de un comit para hacer la transaccion persistente
         /// </summary>
         /// <param name="entity">objeto del modelo de la base de datos</param>
        public void edit(T entity)
        {
            _entities.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<T> findBy(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> query = _dbset.Where(predicate).AsEnumerable();
            return query;
        }

    }
}
