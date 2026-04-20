using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ReviMax.NumeratorManager.Services
{
    internal class ParameterTextService
    {

        public static string BuiltText(string? text = null, string? prefix = null, string? suffix = null, string? separator = ".", int? currNum = 0, bool IsTextEntered = false) 
        {
            StringBuilder builder = new StringBuilder();
            if(string.IsNullOrEmpty(separator)) separator = ".";
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                builder.Append(prefix);
                builder.Append(separator);
            }
            if(text != null) builder.Append(text);
            if (!string.IsNullOrWhiteSpace(suffix))
            {
                builder.Append(separator);
                builder.Append(suffix);
            }
            if (currNum!=null && currNum != 0) 
            {
                if(AllIsNullOrWhiteSpaceExceptNumerator(text, prefix, suffix, currNum, IsTextEntered))
                {
                    builder.Clear();
                    builder.Append(currNum);
                }
                else 
                {
                    builder.Append(separator);
                    builder.Append(currNum);
                }
                
            }
            return builder.ToString();
        }
        private static bool AllIsNullOrWhiteSpaceExceptNumerator(string? text = null, string? prefix = null, string? suffix = null, int? currNum = 0, bool isTextEntered=false)
        {
            return string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(suffix) && !isTextEntered && currNum != null && currNum != 0;
        }
    }
}
