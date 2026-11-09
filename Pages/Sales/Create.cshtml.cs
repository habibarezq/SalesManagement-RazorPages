using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Models;

namespace SalesManagement.Pages.Sales;

public class CreateModel : PageModel
{
    private readonly SalesManagementDbContext _context;

    public CreateModel(SalesManagementDbContext context) => _context = context;

    [BindProperty]
    [Required(ErrorMessage = "Please choose a customer.")]
    public int? CustomerId { get; set; }

    public SelectList Customers { get; set; } = null!;

    public void OnGet() => LoadCustomers();

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            LoadCustomers();
            return Page();
        }

        var invoice = new SalesInvoice
        {
            CustomerId = CustomerId!.Value,
            Date = DateTime.Now
        };
        invoice.SalesInvoiceHistories.Add(new SalesInvoiceHistory
        {
            Status = "Pending",
            Date = DateTime.Now
        });

        _context.SalesInvoices.Add(invoice);
        _context.SaveChanges();

        return RedirectToPage("Details", new { id = invoice.InvoiceId });
    }

    private void LoadCustomers() =>
        Customers = new SelectList(_context.Customers.OrderBy(c => c.Name), "Id", "Name");
}