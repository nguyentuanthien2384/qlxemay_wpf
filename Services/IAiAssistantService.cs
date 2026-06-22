namespace QLXeMay.Services
{
    internal interface IAiAssistantService
    {
        string AnalyzeInventory();
        string AnalyzeRevenue();
        string AnalyzeCustomers();
        string AnalyzeEmployees();
        string AdviseProduct(string question);
        string SmartSearch(string keyword);
        string AnswerQuestion(string question);
    }
}
