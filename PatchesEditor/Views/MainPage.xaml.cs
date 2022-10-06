using PatchesEditor.Data;
using PatchesEditor.ViewModels;
using System.Windows.Controls;

namespace PatchesEditor.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        MainPageViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            DataContext = viewModel = new MainPageViewModel();
        }

        public MainPage(string fileName)
        {
            InitializeComponent();
            DataContext = viewModel = new MainPageViewModel(fileName);
        }

        public MainPage(StartupType startType)
        {
            InitializeComponent();
            DataContext = viewModel = new MainPageViewModel(startType);
        }
    }
}
