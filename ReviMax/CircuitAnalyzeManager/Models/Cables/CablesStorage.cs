using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Config;

namespace ReviMax.CircuitAnalyzeManager.Models.Cables
{
    internal class CablesStorage
    {
        public Dictionary<string, Cable> Cables { get; set; } = new ();

        public void AddCable(Cable cable)
        {
            if (cable == null) return;
            if (!Cables.ContainsKey(cable.Id))
            {
                Cables[cable.Id] = cable;
                ReviMaxLog.Information($"Cable with ID {cable.Id} added to storage.");
            }
        }
        public Cable? GetCableById(string id)
        {
            if (Cables.TryGetValue(id, out var cable))
            {
                ReviMaxLog.Information($"Got cable with ID {id} from storage.");
                return cable;
            }
            return null;
        }

        public void RemoveCableById(string id) 
        {
            if (Cables.TryGetValue(id, out var cable) && cable != null)
            {
                {
                    Cables.Remove(id);
                    ReviMaxLog.Information($"Removed cable with ID {id} from storage.");
                }
            }
        }
    }
}
