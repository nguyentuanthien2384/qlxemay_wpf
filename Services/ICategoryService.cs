using System.Collections.Generic;
using System.Data;
using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface ICategoryService
    {
        DataTable Load(DanhMucConfig config);
        bool Exists(DanhMucConfig config, string keyValue);
        void Insert(DanhMucConfig config, IReadOnlyDictionary<string, object> values);
        void Update(DanhMucConfig config, IReadOnlyDictionary<string, object> values, string keyValue);
        void Delete(DanhMucConfig config, string keyValue);
    }
}
