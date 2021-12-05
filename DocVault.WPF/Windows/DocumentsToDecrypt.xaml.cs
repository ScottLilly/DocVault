﻿using System;
using System.Windows;
using DocVault.DAL;
using DocVault.Models;
using DocVault.Services;
using DocVault.ViewModels;

namespace DocVault.WPF.Windows
{
    public partial class DocumentsToDecrypt : Window
    {
        private readonly IServiceProvider _serviceProvider;

        public DocumentsToDecrypt(UserSettings userSettings,
            IServiceProvider serviceProvider,
            DocVaultDbContext dbContext,
            FileEncryptionService fileEncryptionService)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;

            DataContext = new MainWindowViewModel(dbContext, fileEncryptionService);
        }
    }
}