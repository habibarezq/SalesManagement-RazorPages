using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesManagement.Models;

namespace SalesManagement.Pages.Customers
{
    public class AddCustomerModel : PageModel
    {
        private readonly SalesManagementDbContext _context;

        public AddCustomerModel(SalesManagementDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer NewCustomer { get; set; } = new();

        public void OnGet() { }

        public IActionResult OnPost()
        {
            ModelState.Remove("NewCustomer.SalesInvoices");   // only if the save complains about it

            if (!ModelState.IsValid)
                return Page();

            _context.Customers.Add(NewCustomer);
            _context.SaveChanges();

            TempData["Success"] = "Customer added.";
            return RedirectToPage("Index");
        }
    }
}