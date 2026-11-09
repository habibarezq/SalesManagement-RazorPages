using System;
using System.Collections.Generic;

namespace SalesManagement.Models;

public partial class PurchaseInvoice
{
    public int InvoiceId { get; set; }

    public int SupplierId { get; set; }

    public DateTime Date { get; set; }

    public virtual ICollection<PurchaseInvoiceHistory> PurchaseInvoiceHistories { get; set; } = new List<PurchaseInvoiceHistory>();

    public virtual ICollection<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; } = new List<PurchaseInvoiceItem>();

    public virtual ICollection<StockHistory> StockHistories { get; set; } = new List<StockHistory>();

    public virtual Supplier Supplier { get; set; } = null!;
}
