using Microsoft.AspNetCore.Mvc;
using SistemaInventarios.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarios.Modelos;
using SistemaInventarios.Modelos.ViewModels;
using SistemaInventarios.Utilidades;

namespace SistemaInventariosV6.Areas.Admin.Controllers
{
    //Se debe indicar esta etiqueta a cada controlador para indicar a que area pertenece.
    //Si no se indica se producirá un Error Not Found.
    [Area("Admin")]
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        /// <summary>
        /// Permite acceder al directorio root, se usará para obtener las imagenes
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Este metodo es de tipo GET
        public async Task<IActionResult> Upsert(int? id)
        {
            ProductoVM productoVM = new ProductoVM()
            { 
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Marca"),
                PadreLista= _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Productp")
            };

            if(!id.HasValue)
            {
                //Crear nuevo producto
                productoVM.Producto.Estado = true;
                return View(productoVM);
            }
            else
            {
                //Modificar producto
                productoVM.Producto = await _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());

                //Si el producto no se encontró
                if (productoVM.Producto == null)
                    return NotFound();

                return View(productoVM);
            }
        }

        [HttpPost]//En el post se envía todo, incluso las imagenes
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductoVM productoVM)
        {
            //Si el modelo que se esta recibiendo es valido
            if(ModelState.IsValid)
            {
                //esta variable obtiene los archivos indicados en el formulario
                var files = HttpContext.Request.Form.Files;
                //Ruta donde se va a grabar la imagen
                string webRootPath = _webHostEnvironment.WebRootPath;//Primero se indica el directorio principal, "wwwroot"

                //Si es un nuevo producto
                if(productoVM.Producto.Id == 0)
                {
                    //CREAR
                    string upload = webRootPath + DS.ImagenRuta;//Ruta donde se guardan las imagenes de los productos
                    string fileName = Guid.NewGuid().ToString();//al nombre del archivo se la dá un nombre unico
                    string extension = Path.GetExtension(files[0].FileName);//se obtiene la extensión del primer archivo indicado en el formulario
                    string fullPathFile = Path.Combine(upload, fileName + extension);

                    using (var fileStream = new FileStream(fullPathFile, FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productoVM.Producto.ImagenUrl = fileName + extension;
                    await _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    //ACTUALIZAR

                    //se obtiene el modelo del producto que se va a modificar por medio del método "ObtenerPrimero" indicandole 
                    //el ID y isTracking:false para que el mismo registro, además de consultarlo, podamos modificarlo.
                    var objProducto = await _unidadTrabajo.Producto.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking: false);

                    //si hubo una nueva imagen
                    if(files.Count > 0)
                    {
                        string upload = webRootPath + DS.ImagenRuta;//Ruta donde se guardan las imagenes de los productos
                        string fileName = Guid.NewGuid().ToString();//al nombre del archivo se la dá un nombre unico
                        string extension = Path.GetExtension(files[0].FileName);//se obtiene la extensión del primer archivo indicado en el formulario
                        string fullPathFile = Path.Combine(upload, fileName + extension);

                        //BORRAR IMAGEN ANTERIOR
                        var fullPathFileAnterior = Path.Combine(upload, objProducto.ImagenUrl);
                        //Si existe el archivo anterior
                        if (System.IO.File.Exists(fullPathFileAnterior))
                            System.IO.File.Delete(fullPathFileAnterior);

                        //CREAR LA NUEVA IMAGEN
                        using (var fileStream = new FileStream(fullPathFile, FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);//se crea la imagen
                        }

                        productoVM.Producto.ImagenUrl = fileName + extension;
                    }
                    else//Si no hay nueva imagen 
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }

                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);

                }

                TempData[DS.Exitosa] = "El producto se registró exitosamente";
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            else//Si algo falla. Si el modelo no es valido
            {
                productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Categoria");
                productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Marca");
                productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropDownLista("Producto");
                return View(productoVM);
            }
        }
        
        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()//el "IActionResult" retorna una vista y objetos con formato json
        {
            var todos = await _unidadTrabajo.Producto.ObtenerTodos(incluirPropiedades: "Categoria,Marca");//En las propiedades se le indica que se "navege" por Categoria y Marca para obtener los datos de los mismo
            return Json(new { data = todos });//"data" es el nombre que se va usar en la javascript, por lo que si se cambia ese nombre aquí, se debe usar ese mismo en el javascript
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //Se obtiene la categoria según el ID indicado
            var productoDB = await _unidadTrabajo.Producto.Obtener(id);

            //Si no existe 
            if(productoDB == null)
            {
                //Se indica por medio de javascript un mensaje indicando el error
                //y se finaliza el proceso
                return Json(new { success = false, message = "Error al borrar el producto" });
            }

            //Remover imagen que esta en el directorio
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload, productoDB.ImagenUrl);

            if (System.IO.File.Exists(anteriorFile))
                System.IO.File.Delete(anteriorFile);

            //Si si existe la bodega, se realiza la eliminación
            _unidadTrabajo.Producto.Remover(productoDB);

            //Se guardan los cambios.
            await _unidadTrabajo.Guardar();

            //Se indica un mensaje correspondiente a la eliminación
            return Json(new { success = true, message = "Producto \"" + productoDB.NumeroSerie + "\" borrada exitosamente" });
        }

        [ActionName("ValidarSerie")]//se indica este Tag para poder referenciarlo en el javascript desde la vista upsert
        public async Task<IActionResult> ValidarSerie(string serie, int id=0)
        {
            bool existeProducto = false;
            var lista = await _unidadTrabajo.Producto.ObtenerTodos();

            //Si es nuevo
            if (id == 0)
            {
                //Se indica si existe la categoria que se quiere registrar
                existeProducto = lista.Any(b=>b.NumeroSerie.ToLower().Trim().Equals(serie.ToLower().Trim()));
            }
            //pero si ya existente
            else
            {
                //Se indica si existe o no el elemento que se quiere modificar
                existeProducto = lista.Any(b => b.NumeroSerie.ToLower().Trim().Equals(serie.ToLower().Trim()) && b.Id != id);
            }

            //Se retorna un json indicando si existe o no la categoria  indicada
            return Json(new { data = existeProducto });
        }

        #endregion
    }
}
