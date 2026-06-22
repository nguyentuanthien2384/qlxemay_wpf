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

        public AIAssistantViewModel(IAiAssistantService aiAssistantService, IDialogService dialogService)
        {
            this.aiAssistantService = aiAssistantService;
            this.dialogService = dialogService;

            AnalyzeInventoryCommand = new RelayCommand(_ => Result = aiAssistantService.AnalyzeInventory());
            AnalyzeRevenueCommand = new RelayCommand(_ => Result = aiAssistantService.AnalyzeRevenue());
            AnalyzeCustomersCommand = new RelayCommand(_ => Result = aiAssistantService.AnalyzeCustomers());
            AnalyzeEmployeesCommand = new RelayCommand(_ => Result = aiAssistantService.AnalyzeEmployees());
            AdviseProductCommand = new RelayCommand(_ => Result = aiAssistantService.AdviseProduct(GetQuestionOrDefault("sản phẩm bán chạy")));
            SmartSearchCommand = new RelayCommand(_ => RunWithRequiredQuestion(q => Result = aiAssistantService.SmartSearch(q), "Nhập từ khóa cần tìm."));
            AskCommand = new RelayCommand(_ => RunWithRequiredQuestion(q => Result = aiAssistantService.AnswerQuestion(q), "Nhập câu hỏi trước."));
        }

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

        public ICommand AnalyzeInventoryCommand { get; }
        public ICommand AnalyzeRevenueCommand { get; }
        public ICommand AnalyzeCustomersCommand { get; }
        public ICommand AnalyzeEmployeesCommand { get; }
        public ICommand AdviseProductCommand { get; }
        public ICommand SmartSearchCommand { get; }
        public ICommand AskCommand { get; }

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
    }
}
