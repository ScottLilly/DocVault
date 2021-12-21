using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DocVault.ViewModels
{
    public class OKViewModel : INotifyPropertyChanged
    {
        public string Title { get; }
        public string Message { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public OKViewModel(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public OKViewModel(string title, List<string> question)
        {
            Title = title;
            Message = string.Join(Environment.NewLine, question);
        }
    }
}