using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaInventarios.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarios.AccesoDatos.Configuracion
{
    //En este archivo se indican, de otra manera, las propiedades de los modelos en vez de usar anotaciones
    public class ProductoConfiguracion : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.Property(x=> x.Id).IsRequired();
            builder.Property(x=> x.NumeroSerie).IsRequired().HasMaxLength(60);
            builder.Property(x=> x.Descripcion).IsRequired().HasMaxLength(100);
            builder.Property(x=> x.Estado).IsRequired();
            builder.Property(x=> x.Precio).IsRequired();
            builder.Property(x=> x.Costo).IsRequired();
            builder.Property(x=> x.CategoriaId).IsRequired();
            builder.Property(x=> x.MarcaId).IsRequired();
            builder.Property(x => x.ImagenUrl).IsRequired(false);
            builder.Property(x => x.PadreId).IsRequired(false);

            /*RELACIONES*/

            builder.HasOne(x=> x.Categoria).WithMany()//HasOne...WithMany es relacion 1 a Muchos
                .HasForeignKey(x=>x.CategoriaId)
                .OnDelete(DeleteBehavior.NoAction);//esto es para evitar la eliminacion en Cascada

            builder.HasOne(x => x.Marca).WithMany()
                .HasForeignKey(x => x.MarcaId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Padre).WithMany()
                .HasForeignKey(x => x.PadreId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
