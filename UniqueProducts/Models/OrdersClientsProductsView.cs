using System;
using System.Collections.Generic;

namespace UniqueProducts.Models;

public partial class OrdersClientsProductsView
{
    public int OrderId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Company { get; set; }

    public string? Representative { get; set; }

    public string? ProductName { get; set; }
}
