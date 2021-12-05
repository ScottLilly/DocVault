using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DocVault.DAL;
using DocVault.Models;
using DocVault.Services;

namespace DocVault.ViewModels
{
    public class DecryptWindowViewModel : INotifyPropertyChanged
    {
        private readonly DocVaultDbContext _dbContext;
        private readonly FileEncryptionService _fileEncryptionService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TagSelection> TagSelections { get; } =
            new ObservableCollection<TagSelection>();

        public DecryptWindowViewModel(DocVaultDbContext dbContext,
            FileEncryptionService fileEncryptionService)
        {
            _dbContext = dbContext;
            _fileEncryptionService = fileEncryptionService;

            PopulateTagSelections();
        }

        private void PopulateTagSelections()
        {
            TagSelections.Clear();

            foreach (Tag tag in _dbContext.Tags)
            {
                TagSelections.Add(new TagSelection
                {
                    Tag = tag, IsSelected = false
                });
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}