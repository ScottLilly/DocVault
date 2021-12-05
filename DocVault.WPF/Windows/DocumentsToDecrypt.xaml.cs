using System;
using System.Windows;
using DocVault.DAL;
using DocVault.Services;
using DocVault.ViewModels;

namespace DocVault.WPF.Windows
{
    public partial class DocumentsToDecrypt : Window
    {
        private readonly IServiceProvider _serviceProvider;

        private DecryptWindowViewModel VM => DataContext as DecryptWindowViewModel;

        public DocumentsToDecrypt(IServiceProvider serviceProvider,
            DocVaultDbContext dbContext,
            FileEncryptionService fileEncryptionService)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;

            DataContext = new DecryptWindowViewModel(dbContext, fileEncryptionService);
        }

        private void FindMatchingDocuments_OnClick(object sender, RoutedEventArgs e)
        {
            VM.FindMatchingDocuments();
        }

        private void DecryptSelectedDocuments_OnClick(object sender, RoutedEventArgs e)
        {
            VM.DecryptSelectedDocuments();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}