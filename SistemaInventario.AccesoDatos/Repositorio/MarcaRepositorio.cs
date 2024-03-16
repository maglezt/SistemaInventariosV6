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
    public class MarcaRepositorio : Repositorio<Marca>, IMarcaRepositorio
    {
        private readonly ApplicationDbContext _db;

        public MarcaRepositorio(ApplicationDbContext db): base(db)
        {
            _db = db;
            
        }
        public void Actualizar(Marca marca)
        {
            var marcaBD = _db.Marcas.FirstOrDefault(b => b.Id == marca.Id); //Con esto se verifica si la bodega a actualizar existe en la base de datos, buscandolo por su ID
            if (marcaBD != null) 
            {
                marcaBD.Nombre = marca.Nombre;
                marcaBD.Descripcion = marca.Descripcion;
                marcaBD.Estado = marca.Estado;
                _db.SaveChanges();
            }
        }
    }
}
