using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Models;

namespace SalesManagement.Pages.Products
{
    public class AddProductModel : PageModel
    {
        private readonly SalesManagementDbContext _context;
        private readonly ILogger<AddProductModel> _logger;
        public AddProductModel(SalesManagementDbContext context,ILogger<AddProductModel> logger)
        {
            _context=context;
            _logger=logger;
        }

        [BindProperty]
        public Product newProduct {get; set;}=new();
        public SelectList? Categories {get; set;}=default;
        public void OnGet()
        {
            
            LoadCategories();
        }
        private void LoadCategories()
        {
            Categories=new SelectList(_context.Categories.ToList(),"CategoryId","Name");
            
        }

        public IActionResult OnPost()
        {   
            // _logger.LogInformation("OnPost reached");
            ModelState.Remove("newProduct.Category");
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }
            // _logger.LogInformation("Adding product {Name} with price {Price}", newProduct.Name, newProduct.Price);

            _context.Products.Add(newProduct);
            _context.SaveChanges();
            
            return RedirectToPage("Index");
        }
    }
}
