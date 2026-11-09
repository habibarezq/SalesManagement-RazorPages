using System;
using System.Collections.Generic;

namespace SalesManagement.Models;

public partial class StockHistory
{
    public int StockhistoryId { get; set; }

    public int ProductId { get; set; }

    public int QuantityChange { get; set; }

    public string Reason { get; set; } = null!;

    public int? PurchaseInvoiceId { get; set; }

    public int? SalesInvoiceId { get; set; }

    public DateTime Date { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseInvoice? PurchaseInvoice { get; set; }

    public virtual SalesInvoice? SalesInvoice { get; set; }
}
