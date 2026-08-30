using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReviMax.UI.Resources
{
    internal class TextBoxExtension
    {
            public static readonly DependencyProperty CaptionProperty =
                DependencyProperty.RegisterAttached(
                    "Caption",
                    typeof(string),
                    typeof(TextBoxExtension),
                    new PropertyMetadata(string.Empty));

            public static string GetCaption(DependencyObject obj) =>
                (string)obj.GetValue(CaptionProperty);

            public static void SetCaption(DependencyObject obj, string value) =>
                obj.SetValue(CaptionProperty, value);
        }
}
