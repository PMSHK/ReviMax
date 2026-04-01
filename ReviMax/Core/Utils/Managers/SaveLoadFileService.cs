using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using ReviMax.Config;
using ReviMax.DTO.Annotations;
using ReviMax.Models.Annotations;

namespace ReviMax.Core.Utils.Managers
{
    internal static class SaveLoadFileService
    {

        public static bool LoadFromFile<T>(string path, out (T data,string path) value)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Открыть файл",
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Multiselect = false,
                InitialDirectory = path
            };
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string filePath = openFileDialog.FileName;
                JsonConverter.Deserialize<T>(File.ReadAllText(filePath), out T? dto);
                if (dto != null)
                {
                    value = (dto, filePath);
                    ReviMaxLog.Information($"File loaded from {filePath}. Its data is: {dto.ToString()}");
                    return true;
                }
            }
            value = default;
            return false;
        }

        public static void SaveToFile<T>(T content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content.GetType));

            SaveFileDialog dialog = new SaveFileDialog()
            {
                Title = "Сохранение",
                Filter = "JSON files (*.json)|*.json|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                DefaultExt = ".json",
                AddExtension = true,
                FileName = "ReviMaxSettings"
            };

            bool? result = dialog.ShowDialog();
            string filePath = "";
            if (result == true)
            {
                filePath = dialog.FileName;
                var contentToSave = content;
                File.WriteAllText(filePath, JsonConverter.Serialize<T>(contentToSave));
                ReviMaxLog.Information($"File saved to {filePath}");
            }
        }

    }
}
