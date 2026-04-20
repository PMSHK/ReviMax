using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviMax.UI.ViewModel
{
    class SelectedParameter
    {
        public string Name { get; set; } = string.Empty;
        public SelectedParameter(string name)
        {
            Name = name;
        }
    }
}
