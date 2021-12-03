using System;
using System.IO;
using System.Net.WebSockets;
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
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string USER_SETTINGS_FILE_NAME = "UserSettings.json";

        private IServiceProvider _serviceProvider { get; set; }
        private IConfiguration _configuration { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", false, true)
                          .AddUserSecrets<MainWindow>();

            _configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            _serviceProvider = serviceCollection.BuildServiceProvider();

            // Startup window
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.ShowDialog();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DocVaultDbContext>
                (options => options.UseSqlServer(_configuration.GetConnectionString("SqlDatabase")));

            var settingsManager = new SettingsManager<UserSettings>(USER_SETTINGS_FILE_NAME);
            var userSettings = settingsManager.LoadSettings();

            services.AddSingleton(new FileEncryptionService(userSettings));

            services.AddTransient(typeof(EncryptionKeyEntryWindow));
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(AboutWindow));
        }
    }
}