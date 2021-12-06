using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using DocVault.DAL;
using DocVault.Models;
using DocVault.Services;
using DocVault.ViewModels;
using DocVault.WPF.Windows;

namespace DocVault.WPF
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;

        private DocVaultViewModel VM =>
            DataContext as DocVaultViewModel;
        
        public MainWindow(IServiceProvider serviceProvider,
            DocVaultDbContext dbContext,
            FileEncryptionService fileEncryptionService)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;

            DataContext = new DocVaultViewModel(dbContext, fileEncryptionService);
        }

        private void EnterEncryptionKey_OnClick(object sender, RoutedEventArgs e)
        {
            EncryptionKeyEntryWindow encryptionKeyEntryWindow = 
                _serviceProvider.GetRequiredService<EncryptionKeyEntryWindow>();

            encryptionKeyEntryWindow.ShowDialog();
        }

        private void StoreDocument_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;

            if(openFileDialog.ShowDialog() == true &&
               openFileDialog.FileNames.Length > 0)
            {
                VM.AddDocumentsToStore(openFileDialog.FileNames);
            }
        }

        private void RetrieveDocuments_OnClick(object sender, RoutedEventArgs e)
        {
            var decryptWindow = _serviceProvider.GetRequiredService<DocumentsToDecrypt>();
            decryptWindow.Owner = this;
            decryptWindow.ShowDialog();
        }

        private async void StoreDocuments_OnClickAsync(object sender, RoutedEventArgs e)
        {
            await VM.StoreDocumentsAsync();
        }

        private void ViewHelp_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void AboutDocVault_OnClick(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow =
                _serviceProvider.GetRequiredService<AboutWindow>();
            aboutWindow.Owner = this;

            aboutWindow.Show();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExcludeMatchingDocument_OnClick(object sender, RoutedEventArgs e)
        {
            Document matchingDocumentToRemove = ContextMenuSelectedDocument(sender);

            VM.ExcludeMatchingDocument(matchingDocumentToRemove);

            e.Handled = true;
        }

        private void DeleteMatchingDocument_OnClick(object sender, RoutedEventArgs e)
        {
            Document matchingDocumentToDelete = ContextMenuSelectedDocument(sender);

            File.Delete(matchingDocumentToDelete.FileInfo.FullName);

            VM.ExcludeMatchingDocument(matchingDocumentToDelete);

            e.Handled = true;
        }

        private static Document ContextMenuSelectedDocument(object sender)
        {
            MenuItem menuItem = (MenuItem)sender;

            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;

            DataGrid dataGrid = (DataGrid)contextMenu.PlacementTarget;

            return dataGrid.SelectedItem as Document;
        }
    }
}