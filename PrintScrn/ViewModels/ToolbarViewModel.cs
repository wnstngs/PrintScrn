using System.Windows;

namespace PrintScrn.ViewModels
{
    public class ToolbarViewModel : BaseViewModel
    {
        public ToolbarViewModel()
        {
            ViewModels.Instance.ViewModelsStore.Add(this);
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

        #endregion
    }
}
