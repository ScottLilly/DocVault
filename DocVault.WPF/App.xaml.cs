﻿using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocVault.DAL;
using DocVault.Models;
using DocVault.Services;
using DocVault.WPF.Windows;

namespace DocVault.WPF
{
    public partial class App : Application
    {
        private const string USER_SETTINGS_FILE_NAME = "UserSettings.json";

        private IServiceProvider ServiceProvider { get; set; }
        private IConfiguration Configuration { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", false, true)
                          .AddUserSecrets<MainWindow>();

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Startup window
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.ShowDialog();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DocVaultDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("SqlDatabase")));

            UserSettings userSettings = GetUserSettings();

            services.AddSingleton(new FileEncryptionService(userSettings));

            services.AddTransient(typeof(EncryptionKeyEntryWindow));
            services.AddTransient(f => new MainWindow(userSettings, 
                f.GetRequiredService<IServiceProvider>(), 
                f.GetRequiredService<DocVaultDbContext>(), 
                f.GetRequiredService<FileEncryptionService>()));
            services.AddTransient(typeof(AboutWindow));
        }

        private UserSettings GetUserSettings()
        {
            var settingsManager = 
                new SettingsManager<UserSettings>(USER_SETTINGS_FILE_NAME);

            if (File.Exists(USER_SETTINGS_FILE_NAME))
            {
                 return settingsManager.LoadSettings();
            }

            // TODO: Choose better default locations
            var userSettings = new UserSettings
            {
                EncryptedStorageLocation = new StorageLocation
                {
                    Type = StorageLocation.LocationType.LocalDisk,
                    URI = @"\EncryptedDocuments"
                },
                DecryptedStorageLocation = new StorageLocation
                {
                    Type = StorageLocation.LocationType.LocalDisk,
                    URI = @"\DecryptedDocuments"
                }
            };

            settingsManager.SaveSettings(userSettings);

            return userSettings;
        }
    }
}