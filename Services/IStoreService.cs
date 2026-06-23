using System.Collections.Generic;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface IStoreService
    {
        IReadOnlyList<StoreProduct> LoadProducts(string keyword);
        StoreOrderResult PlaceOrder(string customerId, IEnumerable<CartLine> lines);
        IReadOnlyList<StoreOrderSummary> LoadOrders(string customerId);
        IReadOnlyList<StoreOrderLine> LoadOrderLines(string invoiceNo);
        PartyInfo GetCustomerInfo(string customerId);
    }
}
