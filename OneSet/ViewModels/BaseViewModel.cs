using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace OneSet.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
		public ICommand SaveCommand { get; set; }
        public ICommand LoadCommand { get; set; }

        private string _title;
        public string Title
        {
            set
            {
                if (_title == value) return;
                _title = value;
                OnPropertyChanged("Title");
            }
            get
            {
                return _title;
            }
        }

        protected BaseViewModel()
        {
			SaveCommand = new Command (async () => await OnSave());
            LoadCommand = new Command(async (parameter) => await OnLoad(parameter));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract Task OnLoad(object parameter = null);
        public abstract Task OnSave();
    }
}