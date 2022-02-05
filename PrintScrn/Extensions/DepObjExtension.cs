using System.Windows;
using System.Windows.Media;

namespace PrintScrn.Extensions
{
    public static class DepObjExtension
    {
        public static DependencyObject FindVisualRoot(this DependencyObject o)
        {
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(o);
                if (parent is null)
                {
                    return o;
                }
                o = parent;
            }
        }
    }
}
