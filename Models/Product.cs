using System;
using System.Collections.Generic;

namespace SalesManagement.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public int Quantity { get; set; }

    public int CategoryId { get; set; }

    public decimal Price { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; } = new List<PurchaseInvoiceItem>();

    public virtual ICollection<SalesItem> SalesItems { get; set; } = new List<SalesItem>();

    public virtual ICollection<StockHistory> StockHistories { get; set; } = new List<StockHistory>();
}
