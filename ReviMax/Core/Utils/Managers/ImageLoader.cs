using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ReviMax.Core.Config;

namespace ReviMax.Core.Utils.Managers
{
    internal class ImageLoader : ILoad <BitmapImage>
    {
        public BitmapImage? Load(string imagePath)
        {
            try
            {
                if (File.Exists(imagePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                ReviMaxLog.Warning($"Failed to load image: {imagePath}");
                return null;
            }
            return null;
        }
    }
}
