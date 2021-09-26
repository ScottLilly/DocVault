using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocVault.DAL;
using DocVault.Services;
using DocVault.WPF.Windows;

namespace DocVault.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

            services.AddSingleton(new FileEncryptionService(_configuration.GetConnectionString("BlobStorage"),
                                                            _configuration["Keys:BlobStorage"],
                                                            _configuration["AzureObjects:DocumentContainer"]));

            services.AddTransient(typeof(EncryptionKeyEntryWindow));
            services.AddTransient(typeof(MainWindow));
            services.AddTransient(typeof(AboutWindow));
        }
    }
}