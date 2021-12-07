using System.Windows;
using DocVault.Services;

namespace DocVault.WPF.Windows
{
    public partial class EncryptionKeyEntryWindow : Window
    {
        private readonly FileEncryptionService _fileEncryptionService;
        
        public EncryptionKeyEntryWindow(FileEncryptionService fileEncryptionService)
        {
            InitializeComponent();

            _fileEncryptionService = fileEncryptionService;
        }

        private void EnterKey_OnClick(object sender, RoutedEventArgs e)
        {
            _fileEncryptionService.SetUserEncryptionKey(encryptionKey.Text);
            
            Close();
        }
    }
}