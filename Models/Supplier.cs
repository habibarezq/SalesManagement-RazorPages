using System;
using System.Collections.Generic;

namespace SalesManagement.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
}
