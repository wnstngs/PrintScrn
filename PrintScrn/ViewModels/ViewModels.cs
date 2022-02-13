using System.Collections.Generic;

namespace PrintScrn.ViewModels;

public class ViewModels
{
    public static ViewModels Instance { get; } = new();

    public List<BaseViewModel> ViewModelsStore { get; set; } = new();
}
