using System.Collections.Generic;
using PrintScrn.Infrastructure;

namespace PrintScrn.ViewModels;

public class ViewModels
{
    public static ViewModels Instance { get; } = new();

    public List<Bindable> ViewModelsStore { get; } = new();
}
