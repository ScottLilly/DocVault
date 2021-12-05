using System.ComponentModel;
using System.Runtime.CompilerServices;
using DocVault.DAL;
using DocVault.Services;

namespace DocVault.ViewModels
{
    public class DecryptWindowViewModel : INotifyPropertyChanged
    {
        private readonly DocVaultDbContext _dbContext;
        private readonly FileEncryptionService _fileEncryptionService;

        public event PropertyChangedEventHandler PropertyChanged;

        public DecryptWindowViewModel(DocVaultDbContext dbContext,
            FileEncryptionService fileEncryptionService)
        {
            _dbContext = dbContext;
            _fileEncryptionService = fileEncryptionService;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}