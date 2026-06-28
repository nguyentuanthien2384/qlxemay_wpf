using System.Collections.ObjectModel;
using System.Windows.Input;
using QLXeMay.Infrastructure;
using QLXeMay.Services;

namespace QLXeMay.ViewModels
{
    internal sealed class AIAssistantViewModel : ViewModelBase
    {
        private readonly IAiAssistantService aiAssistantService;
        private readonly IDialogService dialogService;
        private string question;
        private string result;
        private string selectedSampleQuestion;
        private bool isSampleScenarioPanelOpen;
        private string sampleScenarioDetails;
        private string currentContextKey = "overview";
        private string currentContextLabel = "Tổng quan điều hành";

        public AIAssistantViewModel(IAiAssistantService aiAssistantService, IDialogService dialogService)
        {
            this.aiAssistantService = aiAssistantService;
            this.dialogService = dialogService;

            SampleQuestions = new ObservableCollection<string>(aiAssistantService.GetSampleQuestions());

            ExecutiveSummaryCommand = new RelayCommand(_ => ExecuteWithContext("overview", aiAssistantService.ExecutiveSummary));
            ForecastRevenueCommand = new RelayCommand(_ => ExecuteWithContext("forecast", aiAssistantService.ForecastRevenue));
            ReorderAdviceCommand = new RelayCommand(_ => ExecuteWithContext("reorder", aiAssistantService.ReorderAdvice));
            RiskWarningCommand = new RelayCommand(_ => ExecuteWithContext("risk", aiAssistantService.RiskWarning));
            AnalyzeInventoryCommand = new RelayCommand(_ => ExecuteWithContext("inventory", aiAssistantService.AnalyzeInventory));
            AnalyzeRevenueCommand = new RelayCommand(_ => ExecuteWithContext("revenue", aiAssistantService.AnalyzeRevenue));
            AnalyzeCustomersCommand = new RelayCommand(_ => ExecuteWithContext("customers", aiAssistantService.AnalyzeCustomers));
            AnalyzeEmployeesCommand = new RelayCommand(_ => ExecuteWithContext("employees", aiAssistantService.AnalyzeEmployees));
            AdviseProductCommand = new RelayCommand(_ =>
            {
                string q = GetQuestionOrDefault("tư vấn xe bán chạy phù hợp khách phổ thông");
                ExecuteWithContext("advice", () => aiAssistantService.AdviseProduct(q));
            });
            SmartSearchCommand = new RelayCommand(_ => RunWithRequiredQuestion(q =>
            {
                ExecuteWithContext("search", () => aiAssistantService.SmartSearch(q));
            }, "Nhập từ khóa cần tìm."));
            AskCommand = new RelayCommand(_ => RunWithRequiredQuestion(q =>
            {
                ExecuteWithContext(DetectContextKeyFromQuestion(q), () => aiAssistantService.AnswerQuestion(q));
            }, "Nhập câu hỏi trước."));
            UseSampleQuestionCommand = new RelayCommand(parameter => UseSampleQuestion(parameter as string));
            SampleScenariosCommand = new RelayCommand(_ => ShowSampleScenariosInResult());
            CloseSampleScenariosCommand = new RelayCommand(_ => IsSampleScenarioPanelOpen = false);
            ClearCommand = new RelayCommand(_ => Clear());

            Result = aiAssistantService.SampleScenarios();
            SampleScenarioDetails = aiAssistantService.SampleScenariosDetailed(CurrentContextKey);
        }

        public ObservableCollection<string> SampleQuestions { get; }

        public string Question
        {
            get => question;
            set => SetProperty(ref question, value);
        }

        public string Result
        {
            get => result;
            set => SetProperty(ref result, value);
        }

        public string SelectedSampleQuestion
        {
            get => selectedSampleQuestion;
            set
            {
                if (!SetProperty(ref selectedSampleQuestion, value)) return;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Question = value;
                }
            }
        }

        public bool IsSampleScenarioPanelOpen
        {
            get => isSampleScenarioPanelOpen;
            set => SetProperty(ref isSampleScenarioPanelOpen, value);
        }

        public string SampleScenarioDetails
        {
            get => sampleScenarioDetails;
            private set => SetProperty(ref sampleScenarioDetails, value);
        }

        public string CurrentContextLabel
        {
            get => currentContextLabel;
            private set => SetProperty(ref currentContextLabel, value);
        }

        public string CurrentContextKey
        {
            get => currentContextKey;
            private set => SetProperty(ref currentContextKey, value);
        }

        public ICommand ExecutiveSummaryCommand { get; }
        public ICommand ForecastRevenueCommand { get; }
        public ICommand ReorderAdviceCommand { get; }
        public ICommand RiskWarningCommand { get; }
        public ICommand AnalyzeInventoryCommand { get; }
        public ICommand AnalyzeRevenueCommand { get; }
        public ICommand AnalyzeCustomersCommand { get; }
        public ICommand AnalyzeEmployeesCommand { get; }
        public ICommand AdviseProductCommand { get; }
        public ICommand SmartSearchCommand { get; }
        public ICommand AskCommand { get; }
        public ICommand UseSampleQuestionCommand { get; }
        public ICommand SampleScenariosCommand { get; }
        public ICommand CloseSampleScenariosCommand { get; }
        public ICommand ClearCommand { get; }

        private string GetQuestionOrDefault(string defaultValue)
        {
            return string.IsNullOrWhiteSpace(Question) ? defaultValue : Question.Trim();
        }

        private void RunWithRequiredQuestion(System.Action<string> action, string emptyMessage)
        {
            if (string.IsNullOrWhiteSpace(Question))
            {
                dialogService.ShowInformation(emptyMessage);
                return;
            }

            action(Question.Trim());
        }

        private void UseSampleQuestion(string sampleQuestion)
        {
            if (string.IsNullOrWhiteSpace(sampleQuestion)) return;
            Question = sampleQuestion;
            ExecuteWithContext(DetectContextKeyFromQuestion(sampleQuestion), () => aiAssistantService.AnswerQuestion(sampleQuestion));
        }

        private void Clear()
        {
            Question = string.Empty;
            CurrentContextKey = "overview";
            CurrentContextLabel = ContextLabelFromKey(CurrentContextKey);
            Result = aiAssistantService.SampleScenarios();
            SelectedSampleQuestion = null;
        }

        private void ShowSampleScenariosInResult()
        {
            if (string.IsNullOrWhiteSpace(Result))
            {
                SampleScenarioDetails = aiAssistantService.SampleScenariosDetailed(CurrentContextKey);
                Result = SampleScenarioDetails;
            }
            else
            {
                // Always show exactly the same content currently rendered in "Kết quả phân tích".
                SampleScenarioDetails = Result;
            }
            IsSampleScenarioPanelOpen = true;
        }

        private void ExecuteWithContext(string contextKey, System.Func<string> action)
        {
            CurrentContextKey = string.IsNullOrWhiteSpace(contextKey) ? "overview" : contextKey;
            CurrentContextLabel = ContextLabelFromKey(CurrentContextKey);
            Result = action();
        }

        private static string DetectContextKeyFromQuestion(string question)
        {
            string q = (question ?? string.Empty).ToLowerInvariant();
            if (q.Contains("dự báo") || q.Contains("du bao") || q.Contains("forecast")) return "forecast";
            if (q.Contains("nhập thêm") || q.Contains("nhap them") || q.Contains("gợi ý nhập") || q.Contains("goi y nhap")) return "reorder";
            if (q.Contains("rủi ro") || q.Contains("rui ro") || q.Contains("cảnh báo") || q.Contains("canh bao")) return "risk";
            if (q.Contains("tồn kho") || q.Contains("ton kho") || q.Contains("hết hàng") || q.Contains("het hang")) return "inventory";
            if (q.Contains("doanh thu") || q.Contains("lợi nhuận") || q.Contains("loi nhuan")) return "revenue";
            if (q.Contains("khách") || q.Contains("khach") || q.Contains("vip")) return "customers";
            if (q.Contains("nhân viên") || q.Contains("nhan vien") || q.Contains("kpi")) return "employees";
            if (q.Contains("tư vấn") || q.Contains("tu van") || q.Contains("nên mua") || q.Contains("nen mua")) return "advice";
            if (q.Contains("tìm") || q.Contains("tim") || q.Contains("tra cứu") || q.Contains("tra cuu")) return "search";
            return "overview";
        }

        private static string ContextLabelFromKey(string key)
        {
            switch (key)
            {
                case "overview": return "Tổng quan điều hành";
                case "forecast": return "Dự báo doanh thu";
                case "reorder": return "Gợi ý nhập hàng";
                case "risk": return "Cảnh báo rủi ro";
                case "inventory": return "Phân tích tồn kho";
                case "revenue": return "Phân tích doanh thu";
                case "customers": return "Phân tích khách hàng";
                case "employees": return "Phân tích nhân viên";
                case "advice": return "Tư vấn sản phẩm";
                case "search": return "Tìm kiếm dữ liệu";
                default: return "Tổng quan điều hành";
            }
        }
    }
}
