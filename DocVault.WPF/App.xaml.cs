using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocVault.DAL;
using DocVault.Models;
using DocVault.Services;
using DocVault.ViewModels;
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

            services.AddSingleton(userSettings);
            services.AddSingleton(typeof(FileEncryptionService));
            services.AddSingleton(typeof(DocVaultViewModel));

            services.AddTransient(typeof(UserConfiguration));
            services.AddTransient(typeof(EncryptionKeyEntryWindow));
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(DocumentsToDecrypt));
            services.AddTransient(typeof(AboutWindow));
            services.AddTransient(typeof(Help));
            services.AddTransient(typeof(DecryptWindowViewModel));
            services.AddTransient(typeof(UserConfigurationViewModel));
        }

        private UserSettings GetUserSettings()
        {
            var settingsManager =
                new SettingsManager<UserSettings>(USER_SETTINGS_FILE_NAME);

            UserSettings userSettings = settingsManager.LoadSettings();

            if (userSettings != null &&
                userSettings.EncryptedStorageLocation != null &&
                userSettings.DecryptedStorageLocation != null)
            {
                return userSettings;
            }

            // Uninitialized settings, populate with default values
            // TODO: Choose better default locations
            userSettings = new UserSettings
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