namespace PrintScrn.Extensions;

public static class ViewModelsExtension
{
    public static T? FindViewModel<T>()
    {
        foreach (var vm in ViewModels.ViewModels.Instance.ViewModelsStore)
        {
            if (vm is T t)
            {
                return t;
            }
        }

        return default;
    }
}
