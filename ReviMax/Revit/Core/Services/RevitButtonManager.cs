using Autodesk.Revit.UI;
using ReviMax.Core.Config;
using ReviMax.Core.Utils.Config;
using ReviMax.Core.Utils.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace ReviMax.Revit.Core.Services
{
    internal class RevitButtonManager
    {

        public static PushButtonData CreateSmallPushButton(string buttonId, string buttonName, string commandPath, string? buttonTip = null, string? imageName = null )
        {
            var buttonData = CreateButton(buttonId, buttonName, commandPath, buttonTip, imageName);
            BitmapImage? image = LoadImage(imageName);
            SetSmallImageToButton(buttonData, image);
            return buttonData;
        }

        public static PushButtonData CreateLargePushButton(string buttonId, string buttonName, string commandPath, string? buttonTip = null, string? imageName = null)
        {
            var buttonData = CreateButton(buttonId, buttonName, commandPath, buttonTip, imageName);
            BitmapImage? image = LoadImage(imageName);
            SetLargeImageToButton(buttonData,image);
            return buttonData;
        }


        private static PushButtonData CreateButton(string buttonId, string buttonName, string commandPath, string? buttonTip = null, string? imageName = null)
        {
            string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var buttonData = new PushButtonData(buttonId, buttonName, assemblyName, commandPath);
            ReviMaxLog.Information($"buttonTip is: {buttonTip}");
            if (buttonTip != null) buttonData.ToolTip = buttonTip;
            return buttonData;
        }

        private static void SetSmallImageToButton(PushButtonData button, BitmapImage? image)
        {
            if (image != null)
            {
                button.Image = image;
                ReviMaxLog.Information($"Loaded image for button {image != null}");
            }
        }

        private static void SetLargeImageToButton(PushButtonData button, BitmapImage? image)
        {
            if (image != null)
            {
                button.LargeImage = image;
                ReviMaxLog.Information($"Loaded image for button {image != null}");
            }
        }

        private static BitmapImage? LoadImage(string? imageName = null)
        {
            ImageLoader imageLoader = new ImageLoader();

            BitmapImage? image = null;
            if (imageName != null && !string.IsNullOrEmpty(imageName))
            {
                var imagePath = PathManager.GetFilePathInDirectory(PathManager.GetIconsPath(), imageName);
                image = imageLoader.Load(imagePath);
            }
            return image;
        }
    }
}
