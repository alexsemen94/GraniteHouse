using GranitHouse.Data;
using GranitHouse.Models;
using GranitHouse.Models.ViewModel;
using GranitHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GranitHouse.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _env;

        [BindProperty]
        public ProductsViewModel ProdctsVM { get; set; }

        public ProductsController(ApplicationDbContext db, HostingEnvironment env)
        {
            _db = db;
            _env = env;
            ProdctsVM = new ProductsViewModel()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTagTypes = _db.SpecialTagTypes.ToList(),
                Products = new Models.Products()
            };
        }
        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTagTypes);
            return View(await products.ToListAsync());
        }

        public IActionResult Create()
        {
            return View(ProdctsVM);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return View(ProdctsVM);
            }

            _db.Products.Add(ProdctsVM.Products);
            await _db.SaveChangesAsync();

            string webRootPath = _env.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var productsFromDb = _db.Products.Find(ProdctsVM.Products.Id);

            if (files.Count != 0)
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, ProdctsVM.Products.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProdctsVM.Products.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProdctsVM.Products.Id + ".png");
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProdctsVM.Products.Id + ".png";
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == 0)
                return NotFound();

            ProdctsVM.Products = await _db.Products.Include(m => m.SpecialTagTypes).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (ProdctsVM.Products == null)
                return NotFound();

            return View(ProdctsVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _env.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var productFromDb = _db.Products.Where(m => m.Id == ProdctsVM.Products.Id).FirstOrDefault();

                if (files.Count > 0 && files[0] != null)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(productFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, ProdctsVM.Products.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, ProdctsVM.Products.Id + extension_old));
                    }

                    using (var filestream = new FileStream(Path.Combine(uploads, ProdctsVM.Products.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    ProdctsVM.Products.Image = @"\" + SD.ImageFolder + @"\" + ProdctsVM.Products.Id + extension_new;
                }

                if (ProdctsVM.Products.Image != null)
                {
                    productFromDb.Image = ProdctsVM.Products.Image;
                }

                productFromDb.Name = ProdctsVM.Products.Name;
                productFromDb.Price = ProdctsVM.Products.Price;
                productFromDb.Available = ProdctsVM.Products.Available;
                productFromDb.ProductTypeId = ProdctsVM.Products.ProductTypeId;
                productFromDb.SpecialTagTypesId = ProdctsVM.Products.SpecialTagTypesId;
                productFromDb.ShadeColor = ProdctsVM.Products.Name;

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(ProdctsVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == 0)
                return NotFound();

            ProdctsVM.Products = await _db.Products.Include(m => m.SpecialTagTypes).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (ProdctsVM.Products == null)
                return NotFound();

            return View(ProdctsVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == 0)
                return NotFound();

            ProdctsVM.Products = await _db.Products.Include(m => m.SpecialTagTypes).Include(m => m.ProductTypes).SingleOrDefaultAsync(m => m.Id == id);

            if (ProdctsVM.Products == null)
                return NotFound();

            return View(ProdctsVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _env.WebRootPath;
            Products products = await _db.Products.FindAsync(id);

            if (products == null)
                return NotFound();
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extenstion = Path.GetExtension(products.Image);

                if (System.IO.File.Exists(Path.Combine(uploads, products.Id + extenstion)))
                    System.IO.File.Delete(Path.Combine(uploads, products.Id + extenstion));

                _db.Products.Remove(products);

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
    }
}