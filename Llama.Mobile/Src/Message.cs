using Android.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Llama.Mobile.Src
{

    public enum messageType
    {
        User,
        other,
    }

    public class Message : INotifyPropertyChanged
    {
        private StringBuilder _textBuilder = new StringBuilder();
        public string Text { 
            get => _textBuilder.ToString(); 
            set{
                _textBuilder = new StringBuilder(value);
                OnPropertyChanged(nameof(Text));
            } 
        }
        public void AppendText(string text)
        {
            _textBuilder.Append(text);
            OnPropertyChanged(nameof(Text));
        }

        private bool _isPreparing;
        public bool IsPreparing
        {
            get => _isPreparing;
            set
            {
                if (_isPreparing != value)
                {
                    _isPreparing = value;
                    OnPropertyChanged(nameof(IsPreparing));
                }
            }
        }

        public required messageType Type { get; set; }



        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }




}
