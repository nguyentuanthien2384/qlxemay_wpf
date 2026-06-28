using System.Collections.Generic;

namespace QLXeMay.Services
{
    internal interface IAiAssistantService
    {
        IReadOnlyList<string> GetSampleQuestions();
        string ExecutiveSummary();
        string ForecastRevenue();
        string ReorderAdvice();
        string RiskWarning();
        string AnalyzeInventory();
        string AnalyzeRevenue();
        string AnalyzeCustomers();
        string AnalyzeEmployees();
        string AdviseProduct(string question);
        string SmartSearch(string keyword);
        string AnswerQuestion(string question);
        string SampleScenarios();
        string SampleScenariosDetailed(string contextKey);
    }
}
