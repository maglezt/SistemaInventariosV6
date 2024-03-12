using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaInventarios.AccesoDatos.Data;
using SistemaInventarios.AccesoDatos.Repositorio.IRepositorio;

namespace SistemaInventarios.AccesoDatos.Repositorio
{
    public class Repositorio<T> : IRepositorio<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repositorio(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task Agregar(T entidad)//Los metodos asincronos DEBEN de llevar 2 cosas, el "async" y el "await"
        {
            await dbSet.AddAsync(entidad);//se utiliza el "AddAsync" por que se esta trabajando con metodo asincrono. AddAsync es un metodo de EF que se utiliza para registrar un elemento, es lo mismo que usar "insert into table"
        }

        public async Task<T> Obtener(int id)
        {
            return await dbSet.FindAsync(id); //es como usar "select * from tabla where id=id"
        }

        public async Task<IEnumerable<T>> ObtenerTodos(Expression<Func<T, bool>> filtro = null, Func<IQueryable<T>, IOrderedQueryable<T>> ordenarPor = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
                query = query.Where(filtro); //Esto es como hacer un "select * from where ...[filtro]"

            if (!string.IsNullOrEmpty(incluirPropiedades))
                foreach (var caracter in incluirPropiedades.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(caracter); //Esto hace que se incluyan los datos a los que está relacionado la entidad, por ejemplo: la categoria, marca, etc de un producto

            if (ordenarPor != null)
                query = ordenarPor(query);

            if (!isTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<T> ObtenerPrimero(Expression<Func<T, bool>> filtro = null, string incluirPropiedades = null, bool isTracking = true)
        {
            IQueryable<T> query = dbSet;
            if (filtro != null)
                query = query.Where(filtro); //Esto es como hacer un "select * from where ...[filtro]"

            if (string.IsNullOrEmpty(incluirPropiedades))
                foreach (var caracter in incluirPropiedades.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(caracter); //Esto hace que se incluyan los datos a los que está relacionado la entidad, por ejemplo: la categoria, marca, etc de un producto

            if (!isTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        

        public void Remover(T entidad)
        {
            dbSet.Remove(entidad);
        }

        public void RemoverRango(IEnumerable<T> entidad)
        {
            dbSet.RemoveRange(entidad);
        }
    }
}
