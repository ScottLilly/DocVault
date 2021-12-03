﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DocVault.DAL;
using DocVault.Models;
using DocVault.Services;
using Microsoft.EntityFrameworkCore;

namespace DocVault.ViewModels
{
    public sealed class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly UserSettings _userSettings;
        private readonly DocVaultDbContext _dbContext;
        private readonly FileEncryptionService _fileEncryptionService;

        private string _tags;

        public ObservableCollection<Document> DocumentsToStore { get; set; } = new();
        public ObservableCollection<Document> DocumentsThatMatch { get; set; } = new();
        public ObservableCollection<Document> DocumentsAlreadyInStorage { get; set; } = new();
        public string Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged();
            }
        }

        public bool HasEncryptionKey =>
            _fileEncryptionService?.EncryptionKeyIsSet ?? false;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(UserSettings userSettings,
                                    DocVaultDbContext dbContext,
                                   FileEncryptionService fileEncryptionService)
        {
            _userSettings = userSettings;
            _dbContext = dbContext;
            _fileEncryptionService = fileEncryptionService;

            _fileEncryptionService.PropertyChanged += FileEncryptionService_OnPropertyChanged;
        }

        public void AddDocumentsToStore(IEnumerable<string> fileNames)
        {
            ClearUIControls();

            // Create Document objects for each selected file
            List<Document> documentsToStore =
                fileNames.Select(DocumentService.CreateDocumentFromFile).ToList();

            foreach (Document document in documentsToStore)
            {
                // See if any already-stored documents have the same metadata
                List<Document> matchingStoredDocuments =
                    GetMatchingStoredDocuments(document).ToList();

                if (matchingStoredDocuments.Any())
                {
                    // Matching document(s) exist, so populate their Tags property
                    foreach (Tag tag in matchingStoredDocuments.SelectMany(msd => msd.Tags))
                    {
                        document.Tags.Add(tag);
                    }

                    DocumentsAlreadyInStorage.Add(document);
                }
                else if (documentsToStore.Any(d => d.FileSize.Equals(document.FileSize) &&
                                                   d.Checksum.SequenceEqual(document.Checksum) &&
                                                   d != document))
                {
                    // Document matches another in selected files to store list
                    DocumentsThatMatch.Add(document);
                }
                else
                {
                    DocumentsToStore.Add(document);
                }
            }
        }

        public void ExcludeMatchingDocument(Document documentToRemove)
        {
            DocumentsThatMatch.Remove(documentToRemove);

            // Check if that cleared any documents to move from
            // DocumentsThatMatch into DocumentsToStore
            List<Document> documentsToMove = new List<Document>();

            foreach (Document document in DocumentsThatMatch)
            {
                if (DocumentsToStore.None(d => d.FileSize.Equals(document.FileSize) &&
                                               d.Checksum.SequenceEqual(document.Checksum)) &&
                   DocumentsThatMatch.None(d => d.FileSize.Equals(document.FileSize) &&
                                                d.Checksum.SequenceEqual(document.Checksum) &&
                                                d != document))
                {
                    documentsToMove.Add(document);
                }
            }

            foreach (Document document in documentsToMove)
            {
                DocumentsThatMatch.Remove(document);
                DocumentsToStore.Add(document);
            }
        }

        public async Task StoreDocumentsAsync()
        {
            foreach (Document document in DocumentsToStore)
            {
                AddInputTagsToDocument(document);

                await _dbContext.Documents.AddAsync(document);

                await _fileEncryptionService.EncryptDocumentAsync(document);

                //_fileEncryptionService.DecryptDocument(document);
            }

            await _dbContext.SaveChangesAsync();

            ClearUIControls();
        }

        #region Private functions

        private IEnumerable<Document> GetMatchingStoredDocuments(Document document)
        {
            return _dbContext.Documents
                             .Include(d => d.Tags)
                             .Where(d => d.FileSize.Equals(document.FileSize) &&
                                         d.Checksum.SequenceEqual(document.Checksum));
        }

        private void AddInputTagsToDocument(Document document)
        {
            List<Tag> tags = GetDistinctTagsFromInput();

            tags.ForEach(document.Tags.Add);
        }

        private List<Tag> GetDistinctTagsFromInput()
        {
            List<Tag> distinctTags = new List<Tag>();

            foreach (string tagPhrase in GetDistinctTagPhrasesFromInput())
            {
                // Add the tag phrase to the Tag table, if needed
                if (_dbContext.Tags.None(tag => tag.Value.Equals(tagPhrase, StringComparison.CurrentCultureIgnoreCase)))
                {
                    _dbContext.Tags.Add(new Tag { Value = tagPhrase });
                    _dbContext.SaveChanges();
                }

                // Add the Tag object to return results
                distinctTags.Add(_dbContext.Tags.First(tag => tag.Value.Equals(tagPhrase)));
            }

            return distinctTags;
        }

        private IEnumerable<string> GetDistinctTagPhrasesFromInput()
        {
            HashSet<string> distinctTags = new HashSet<string>();

            foreach (string tagPhrase in Tags.SplitAndCleaned())
            {
                distinctTags.Add(tagPhrase);
            }

            return distinctTags;
        }

        private void ClearUIControls()
        {
            DocumentsToStore.Clear();
            DocumentsThatMatch.Clear();
            DocumentsAlreadyInStorage.Clear();
            Tags = "";
        }

        private void FileEncryptionService_OnPropertyChanged(object sender,
                                                             PropertyChangedEventArgs e)
        {
            if (e.PropertyName?.Equals(nameof(FileEncryptionService.EncryptionKeyIsSet)) == true)
            {
                OnPropertyChanged(nameof(HasEncryptionKey));
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}