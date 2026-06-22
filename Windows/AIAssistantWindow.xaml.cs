using System.Windows;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class AIAssistantWindow : Window
    {
        public AIAssistantWindow()
        {
            InitializeComponent();
            DataContext = new AIAssistantViewModel(new AiAssistantService(), new DialogService());
        }
    }
}
