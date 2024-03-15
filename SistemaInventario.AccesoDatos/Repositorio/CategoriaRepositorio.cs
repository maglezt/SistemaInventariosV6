using SistemaInventarios.AccesoDatos.Data;
using SistemaInventarios.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarios.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarios.AccesoDatos.Repositorio
{
    public class CategoriaRepositorio : Repositorio<Categoria>, ICategoriaRepositorio
    {
        private readonly ApplicationDbContext _db;

        public CategoriaRepositorio(ApplicationDbContext db): base(db)
        {
            _db = db;
            
        }
        public void Actualizar(Categoria categoria)
        {
            var categoriaBD = _db.Categorias.FirstOrDefault(b => b.Id == categoria.Id); //Con esto se verifica si la bodega a actualizar existe en la base de datos, buscandolo por su ID
            if (categoriaBD != null) 
            {
                categoriaBD.Nombre = categoria.Nombre;
                categoriaBD.Descripcion = categoria.Descripcion;
                categoriaBD.Estado = categoria.Estado;
                _db.SaveChanges();
            }
        }
    }
}
