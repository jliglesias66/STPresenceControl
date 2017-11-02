using SugaarSoft.MVVM.Base;
using System.Collections.Generic;
using System.Windows.Input;

namespace STPresenceControl.ViewModels
{
    public class MainViewModel : NotificationObject
    {
        #region Binding

        private readonly List<ICommand> _sections = new List<ICommand>(); //WON'T CHANGE DURING APP LIFETIME
        public List<ICommand> Sections { get { return _sections; } }

        private ICommand _selectedSection;
        public ICommand SelectedSection
        {
            get { return _selectedSection; }
            set { _selectedSection = value; OnPropertyChanged(); }
        }

        #endregion
    }
}
