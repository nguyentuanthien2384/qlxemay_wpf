using System.Data;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface ISalesInvoiceService
    {
        DataTable LoadEmployees();
        DataTable LoadCustomers();
        DataTable LoadProducts();
        DataTable LoadInvoiceNumbers();
        string GetEmployeeName(string employeeId);
        PartyInfo GetCustomerInfo(string customerId);
        string GetProductName(string productId);
        SalesInvoiceOperationResult SaveLine(SalesInvoiceRequest request);
        SalesInvoiceOperationResult DeleteLine(string invoiceNo, string productId, int quantity);
        void CancelInvoice(string invoiceNo);
        SalesInvoiceSnapshot LoadInvoice(string invoiceNo);
    }
}
