using System.Windows.Controls;
using QLXeMay.Services;
using QLXeMay.ViewModels;

namespace QLXeMay.Windows
{
    public partial class AIAssistantWindow : UserControl
    {
        public AIAssistantWindow()
        {
            InitializeComponent();
            DataContext = new AIAssistantViewModel(new AiAssistantService(), new DialogService());
        }
    }
}
