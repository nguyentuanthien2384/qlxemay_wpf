using System.Collections.Generic;
using System.Data;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface ISearchService
    {
        DataTable Search(SearchMode mode, IReadOnlyDictionary<string, string> criteria);
    }
}
