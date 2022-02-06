using System.Collections.Generic;
using System.Windows.Documents;

namespace PrintScrn.ViewModels
{
    public class ViewModels
    {
        private static ViewModels _instance = new();

        public static ViewModels Instance => _instance;

        public List<BaseViewModel> ViewModelsStore { get; set; } = new List<BaseViewModel>();
    }
}
