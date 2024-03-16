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
    public class MarcaConfiguracion : IEntityTypeConfiguration<Marca>
    {
        public void Configure(EntityTypeBuilder<Marca> builder)
        {
            builder.Property(x=> x.Id).IsRequired();
            builder.Property(x=> x.Nombre).IsRequired().HasMaxLength(60);
            builder.Property(x=> x.Descripcion).IsRequired().HasMaxLength(100);
            builder.Property(x=> x.Estado).IsRequired();
        }
    }
}
