using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DocVault.ViewModels
{
    public class YesNoViewModel : INotifyPropertyChanged
    {
        public string Title { get; }
        public string Question { get; }
        public bool Response { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public YesNoViewModel(string title, string question)
        {
            Title = title;
            Question = question;
        }

        public YesNoViewModel(string title, List<string> question)
        {
            Title = title;
            Question = string.Join(Environment.NewLine, question);
        }
    }
}