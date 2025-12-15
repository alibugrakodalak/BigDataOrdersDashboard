using BigDataOrdersDashboard.Context;
using BigDataOrdersDashboard.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BigDataOrdersDashboard.Controllers
{
    public class ProductController : Controller
    {
        private readonly BigDataOrdersDbContext _context;

        public ProductController(BigDataOrdersDbContext context)
        {
            _context = context;
        }

        public IActionResult ProductList(int page = 1)
        {
            //var values = _context.Products.ToList();
            //return View(values);

            int pageSize = 18; // her seferinde 18 kayıt
            var values = _context.Products
                                 .OrderBy(p => p.ProductId)
                                 .Skip((page -1) * pageSize)
                                 .Take(pageSize)
                                 .Include(y=>y.Category)
                                 .ToList();

            int totalCount = _context.Products.Count();
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(values);
        }

        [HttpGet]
        public IActionResult CreateProduct() 
        {
            var categoryList = _context.Categories
            .Select(x => new SelectListItem
            {
                Text = x.CategoryName,
                Value = x.CategoryId.ToString()
            })
            .ToList();

            //Dropdown için viewBag e gönder
            ViewBag.CategoryList = categoryList;
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(Product product) 
        { 
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction("ProductList");
        }

        public IActionResult DeleteProduct(int id)
        {
            var value = _context.Products.Find(id);
            _context.Products.Remove(value);
            _context.SaveChanges();
            return RedirectToAction("ProductList");
        }

        [HttpGet]
        public IActionResult UpdateProduct(int id) 
        {
            var categoryList = _context.Categories
                                .Select(x => new SelectListItem
                                {
                                    Text = x.CategoryName,
                                    Value = x.CategoryId.ToString()
                                })
                                .ToList();

            ViewBag.CategoryList = categoryList;

            var value = _context.Products.Find(id);
            return View(value);
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product product) 
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction("ProductList");
        }
    }
}
