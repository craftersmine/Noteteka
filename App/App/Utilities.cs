using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace App
{
    public class Utilities
    {
        public static T GetParent<T>(DependencyObject obj) => (T)GetParent(obj, typeof(T));
        public static object GetParent(DependencyObject obj, Type type)
        {
            if (obj == null)
                return null;

            var parent = VisualTreeHelper.GetParent(obj);
            if (parent == null)
                return null;

            if (type.IsAssignableFrom(parent.GetType()))
                return parent;

            return GetParent(parent, type);
        }
    }
}
