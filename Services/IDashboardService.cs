using QLXeMay.Models;

namespace QLXeMay.Services
{
    internal interface IDashboardService
    {
        DashboardSnapshot Load();
    }
}
