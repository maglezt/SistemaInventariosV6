using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarios.AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class//De esta manera se indica que la interfaz es generica para que trabaje con cualquier tipo de objeto que le enviemos
    {
        /// <summary>
        /// Regresa el tipo de objeto que se le envía a la interfaz según el ID proporcionado
        /// </summary>
        /// <param name="id">ID del objeto a buscar</param>
        /// <returns></returns>
        Task<T> Obtener(int id);//Indicando "Task<> se hace asincrono el método"

        /// <summary>
        /// Devuelve una lista de todos los elementos del tipo de objeto que se envía a la interfaz
        /// </summary>
        /// <param name="filtro"></param>
        /// <param name="ordenarPor"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> ObtenerTodos(
            Expression<Func<T, bool>> filtro = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null,
            string incluirPropiedades = null,
            bool isTracking = true
            );

        Task<T> ObtenerPrimero(
            Expression<Func<T, bool>> filtro = null,
            string incluirPropiedades = null,
            bool isTracking = true
            );

        Task Agregar(T entidad);

        void Remover(T entidad);//No puede ser asincrono por que se trata de remover elementos

        void RemoverRango(IEnumerable<T> entidad);
    }
}
