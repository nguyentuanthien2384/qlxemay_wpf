using System.Data;

namespace QLXeMay.Services
{
    internal interface IGlobalSearchService
    {
        DataTable Search(string keyword);
    }
}
