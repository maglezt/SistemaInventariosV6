using Microsoft.AspNetCore.Mvc;
using SistemaInventarios.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarios.Modelos;
using SistemaInventarios.Utilidades;

namespace SistemaInventariosV6.Areas.Admin.Controllers
{
    //Se debe indicar esta etiqueta a cada controlador para indicar a que area pertenece.
    //Si no se indica se producirá un Error Not Found.
    [Area("Admin")]
    public class CategoriaController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;

        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Este metodo es de tipo GET
        public async Task<IActionResult> Upsert(int? id)
        {
            Categoria categoria = new Categoria();

            if(!id.HasValue)
            {
                //Crear una nueva bodega
                categoria.Estado = true;
                return View(categoria);
            }
            
            //Se actualiza bodega
            categoria = await _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if (categoria == null)
                return NotFound();

            return View(categoria);
        }

        [HttpPost] //hay que indicar esta etiqueta al metodo para indicar que es de tipo POST
        [ValidateAntiForgeryToken]//esto evita las falsificaciones de solicitudes de otro sitio que está intentando cargar datos en nuestra página
        public async Task<IActionResult> Upsert(Categoria categoria)
        {
            if(ModelState.IsValid) //valida que el modelo este correcto en cada una de sus propiedades
            { 
                if(categoria.Id == 0)
                {
                    await _unidadTrabajo.Categoria.Agregar(categoria);
                    TempData[DS.Exitosa] = "Categoría creada exitosamente";
                }
                else
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
                    TempData[DS.Exitosa] = "Categoría actualizada exitosamente";
                }

                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));//redirecciona a Index
            }

            TempData[DS.Error] = "Error al grabar la categoría";
            return View(categoria);
        }

        #region API

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()//el "IActionResult" retorna una vista y objetos con formato json
        {
            var todos = await _unidadTrabajo.Categoria.ObtenerTodos();
            return Json(new { data = todos });//"data" es el nombre que se va usar en la javascript, por lo que si se cambia ese nombre aquí, se debe usar ese mismo en el javascript
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            //Se obtiene la categoria según el ID indicado
            var categoriaBD = await _unidadTrabajo.Categoria.Obtener(id);

            //Si no existe la categoria
            if(categoriaBD == null)
            {
                //Se indica por medio de javascript un mensaje indicando el error
                //y se finaliza el proceso
                return Json(new { success = false, message = "Error al borrar la categoría" });
            }

            //Si si existe la bodega, se realiza la eliminación
            _unidadTrabajo.Categoria.Remover(categoriaBD);

            //Se guardan los cambios.
            await _unidadTrabajo.Guardar();

            //Se indica un mensaje correspondiente a la eliminación
            return Json(new { success = true, message = "Categoría " + categoriaBD.Nombre + " borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]//se indica este Tag para poder referenciarlo en el javascript desde la vista upsert
        public async Task<IActionResult> ValidarNombre(string nombre, int id=0)
        {
            bool existeCategoria = false;
            var lista = await _unidadTrabajo.Categoria.ObtenerTodos();

            //Si es una categoria  nueva
            if (id == 0)
            {
                //Se indica si existe la categoria que se quiere registrar
                existeCategoria = lista.Any(b=>b.Nombre.ToLower().Trim().Equals(nombre.ToLower().Trim()));
            }
            //pero si es una bodega existente
            else
            {
                //Se indica si existe la categoria que se quiere modificar
                existeCategoria = lista.Any(b => b.Nombre.ToLower().Trim().Equals(nombre.ToLower().Trim()) && b.Id != id);
            }

            //Se retorna un json indicando si existe o no la categoria  indicada
            return Json(new { data = existeCategoria });
        }

        #endregion
    }
}
