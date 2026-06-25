using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Autodesk.Revit.DB;
using ReviMax.CircuitAnalyzeManager.Models;
using ReviMax.CircuitAnalyzeManager.Models.Graph;
using ReviMax.CircuitAnalyzeManager.Services;
using ReviMax.Core.Config;
using ReviMax.Core.Enums;
using ReviMax.Core.Filters;
using ReviMax.Core.Utils.Converter;
using ReviMax.GostSymbolManager.UI.Commands;
using ReviMax.Revit.Core.Bridge;
using ReviMax.Revit.Core.Services;
using ReviMax.Revit.Model;

namespace ReviMax.CircuitAnalyzeManager.UI.ViewModel
{
    class CableSystemAnalyzeViewModel
    {
        private Document _doc;
        public Document Doc { get => _doc; }
        public RevitDispatcherService dispatcher = new();

        public ICommand Graph { get; }
        public ICommand Marks { get; }
        public ICommand CableJournal { get; }
        public ICommand CableLoad { get; }
        public CableSystemAnalyzeViewModel(Document doc) { 
            _doc = doc;

            Graph = new ReviMaxCommand(BuildGraph, () => true);
            Marks = new ReviMaxCommand(PlaceMarksOnCableSystems, () => true);
            CableJournal = new ReviMaxCommand(CreateCableJournal, () => true);
            CableLoad = new ReviMaxCommand(CreateCableLoad, () => true);
        }

        public void BuildGraph()
        {
            var cableSystemService = new CableSystemsService(Doc);
            var result = cableSystemService.GetStraightCableSystems();
            if (result.IsSuccess)
            {
                Dictionary<FamilyMode,IList<Element>> cableSystems = result.Value;
                ReviMaxLog.Information($"Successfully retrieved cable systems: {cableSystems.Count} types found.");
                var graphBuilder = new CircuitGraphBuilder();
                
                List<RouteElementData> routeElements = cableSystems
                    .SelectMany(el => el.Value)
                    .Select(el =>
                    {
                        var curve = (el.Location as LocationCurve)?.Curve;
                        return new RouteElementData
                        {
                            Element = new RouteElementInfo { Id = el.Id, Name = el.Name, UniqueId = el.UniqueId },
                            Start = curve!.GetEndPoint(0),
                            End = curve!.GetEndPoint(1),
                            Type = el.GetType().Name
                        };
                    })
                    .ToList();

                double tolerance = 50.0;

                
                dispatcher.Request(
                    request: app =>
                    {
                        var graph = graphBuilder.BuildGraph(routeElements, tolerance.MillimetersToFeet());
                        if (graph.IsSuccess)
                        {
                            var compressedGraph = graph.Value.CompressGraph();
                            ReviMaxLog.Information($"Graph built successfully with {string.Join(", ", compressedGraph.Edges.Select(e => e.Value.RunElements.Select(h => h.Id)))}");
                            ReviMaxLog.Information($"Graph built successfully with {string.Join(", ", compressedGraph.Nodes.Select(n => n.Value.Point))}");

                            var filter = new RevitFilterManager(Doc);
                            var elements = filter.GetElectricalElementsAll(new AllElectricalEquipmentFilter(), compressedGraph, tolerance.MillimetersToFeet());
                        }
                    });

            }
            ReviMaxLog.Information($"Failed to retrieve cable systems: {result.ErrorMessage}");
        }

        public void PlaceMarksOnCableSystems()
        {
            // Implementation for placing marks on cable systems in the Revit model
        }

        public void CreateCableJournal()
        {
            // Implementation for creating a journal of cable systems, possibly exporting to Excel or another format
        }

        public void CreateCableLoad()
        {
            // Implementation for creating a cable load analysis
        }
    }
}
