using System.Windows;
using System.Windows.Input;
using PrintScrn.Commands;

namespace PrintScrn.ViewModels
{
    public class ToolbarViewModel : BaseViewModel
    {
        public ToolbarViewModel()
        {
            ViewModels.Instance.ViewModelsStore.Add(this);

            QuitAppCmd = new RelayCommand(
                OnExecuted_QuitAppCmd,
                CanExecute_QuitAppCmd
            );
        }

        #region Properties

        #region ToolbarVisibility

        private Visibility _toolbarVisibility = Visibility.Visible;

        public Visibility ToolbarVisibility
        {
            get => _toolbarVisibility;
            set => Set(ref _toolbarVisibility, value);
        }

        #endregion

        #endregion

        #region Commands

        #region QuitAppCmd

        public ICommand QuitAppCmd { get; }

        private static bool CanExecute_QuitAppCmd(object p)
        {
            return true;
        }

        private void OnExecuted_QuitAppCmd(object p)
        {
            Application.Current.Shutdown(0);
        }

        #endregion

        #endregion
    }
}
