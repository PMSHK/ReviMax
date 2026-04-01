using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.GostSymbolManager.UI.ViewModel
{
    public class FamilyVM : INotifyPropertyChanged
    {
        private bool _isLoaded;         
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title { get; set; } = "";
        public bool IsLoaded 
        { 
            get => _isLoaded; 
            set {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    OnPropertyChanged(nameof(IsLoaded));
                }
            } 
        }

        protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public FamilyVM() { }
        public FamilyVM(string title)
        {
            Title = title;
            
        }
        }
}
