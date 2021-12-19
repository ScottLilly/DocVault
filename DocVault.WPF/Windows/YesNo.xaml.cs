using System.Collections.Generic;
using System.Windows;
using DocVault.ViewModels;

namespace DocVault.WPF.Windows
{
    public partial class YesNo : Window
    {
        private YesNoViewModel VM => DataContext as YesNoViewModel;

        public bool Response => VM.Response;

        public YesNo(string title, string question)
        {
            InitializeComponent();

            DataContext = new YesNoViewModel(title, question);
        }

        public YesNo(string title, List<string> question)
        {
            InitializeComponent();

            DataContext = new YesNoViewModel(title, question);
        }

        private void No_OnClick(object sender, RoutedEventArgs e)
        {
            VM.Response = false;

            Close();
        }

        private void Yes_OnClick(object sender, RoutedEventArgs e)
        {
            VM.Response = true;

            Close();
        }
    }
}