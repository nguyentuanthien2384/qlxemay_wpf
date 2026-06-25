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

        public AIAssistantViewModel(IAiAssistantService aiAssistantService, IDialogService dialogService)
        {
            this.aiAssistantService = aiAssistantService;
            this.dialogService = dialogService;

            SampleQuestions = new ObservableCollection<string>(aiAssistantService.GetSampleQuestions());

            ExecutiveSummaryCommand = new RelayCommand(_ => Result = aiAssistantService.ExecutiveSummary());
            ForecastRevenueCommand = new RelayCommand(_ => Result = aiAssistantService.ForecastRevenue());
            ReorderAdviceCommand = new RelayCommand(_ => Result = aiAssistantService.ReorderAdvice());
            RiskWarningCommand = new RelayCommand(_ => Result = aiAssistantService.RiskWarning());
            AnalyzeInventoryCommand = new RelayCommand(_ => Result = aiAssistantService.AnalyzeInventory());
            AnalyzeRevenueCommand = new RelayCommand(_ => Result = aiAssistantService.AnalyzeRevenue());
            AnalyzeCustomersCommand = new RelayCommand(_ => Result = aiAssistantService.AnalyzeCustomers());
            AnalyzeEmployeesCommand = new RelayCommand(_ => Result = aiAssistantService.AnalyzeEmployees());
            AdviseProductCommand = new RelayCommand(_ => Result = aiAssistantService.AdviseProduct(GetQuestionOrDefault("tư vấn xe bán chạy phù hợp khách phổ thông")));
            SmartSearchCommand = new RelayCommand(_ => RunWithRequiredQuestion(q => Result = aiAssistantService.SmartSearch(q), "Nhập từ khóa cần tìm."));
            AskCommand = new RelayCommand(_ => RunWithRequiredQuestion(q => Result = aiAssistantService.AnswerQuestion(q), "Nhập câu hỏi trước."));
            UseSampleQuestionCommand = new RelayCommand(parameter => UseSampleQuestion(parameter as string));
            SampleScenariosCommand = new RelayCommand(_ => Result = aiAssistantService.SampleScenarios());
            ClearCommand = new RelayCommand(_ => Clear());

            Result = aiAssistantService.SampleScenarios();
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
            Result = aiAssistantService.AnswerQuestion(sampleQuestion);
        }

        private void Clear()
        {
            Question = string.Empty;
            Result = aiAssistantService.SampleScenarios();
            SelectedSampleQuestion = null;
        }
    }
}
