using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using DocVault.DAL;
using DocVault.Models;
using DocVault.Services;
using Microsoft.EntityFrameworkCore;

namespace DocVault.ViewModels
{
    public class DecryptWindowViewModel : INotifyPropertyChanged
    {
        private readonly DocVaultDbContext _dbContext;
        private readonly FileEncryptionService _fileEncryptionService;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TagSelection> TagSelections { get; } =
            new ObservableCollection<TagSelection>();
        public ObservableCollection<DocumentSelection> DocumentSelections { get; } =
            new ObservableCollection<DocumentSelection>();

        public DecryptWindowViewModel(DocVaultDbContext dbContext,
            FileEncryptionService fileEncryptionService)
        {
            _dbContext = dbContext;
            _fileEncryptionService = fileEncryptionService;

            PopulateTagSelections();
        }

        public void FindMatchingDocuments()
        {
            DocumentSelections.Clear();

            foreach (var tagSelection in TagSelections.Where(ts => ts.IsSelected))
            {
                var matchingDocs = 
                    _dbContext.Documents.Include(d => d.Tags)
                        .ToList().Where(d => d.Tags.Contains(tagSelection.Tag));

                foreach (Document document in matchingDocs)
                {
                    if (!DocumentSelections.Any(ds => ds.Document.Id.Equals(document.Id)))
                    {
                        DocumentSelections.Add(new DocumentSelection
                        {
                            Document = document, IsSelected = false
                        });
                    }
                }
            }
        }

        public void DecryptSelectedDocuments()
        {
            foreach (DocumentSelection documentSelection in DocumentSelections.Where(ds => ds.IsSelected))
            {
                _fileEncryptionService.DecryptDocument(documentSelection.Document);
            }

            DocumentSelections.Clear();
        }

        private void PopulateTagSelections()
        {
            TagSelections.Clear();

            foreach (Tag tag in _dbContext.Tags)
            {
                TagSelections.Add(new TagSelection
                {
                    Tag = tag,
                    IsSelected = false
                });
            }
        }
    }
}