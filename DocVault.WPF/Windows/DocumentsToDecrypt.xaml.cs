using System.Threading.Tasks;
using System.Windows;
using DocVault.ViewModels;

namespace DocVault.WPF.Windows
{
    public partial class DocumentsToDecrypt : Window
    {
        private DecryptWindowViewModel VM => DataContext as DecryptWindowViewModel;

        public DocumentsToDecrypt(DecryptWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void FindMatchingDocuments_OnClick(object sender, RoutedEventArgs e)
        {
            VM.FindMatchingDocuments();
        }

        private async Task DecryptSelectedDocuments_OnClick(object sender, RoutedEventArgs e)
        {
            await VM.DecryptSelectedDocumentsAsync();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}