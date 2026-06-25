using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ReviMax.Revit.Core.Services;
using ReviMax.Revit.Model;

namespace ReviMax.CircuitAnalyzeManager.Models.Graph
{
    internal static class GraphNodeClassifier
    {
        public static void ClassifyNodes (this CircuitGraph graph)
        {

        }

        public static void FindJunctionBoxes(this CircuitGraph graph,Document doc, params BuiltInCategory[] categories)
        {

        }

        private static bool IsSpecialNode(CircuitGraph graph, CircuitGraphNode node)
        {
            // Логика определения, является ли узел "особым"
            // Например, можно считать узлом "особым", если он имеет степень больше 2 или связан с определёнными типами элементов
            return node.Degree > 2;
        }

        private static bool IsFittingNode(CircuitGraph graph, CircuitGraphNode node)
        {
            // Логика определения, является ли узел "подходящим" для сжатия
            // Например, можно считать узлом "подходящим", если он имеет степень 2 и не связан с определёнными типами элементов
            return node.Degree == 2 && !IsSpecialNode(graph, node);
        }

        private static bool IsJunctionBoxNode(CircuitGraph graph, CircuitGraphNode node)
        {
            // Логика определения, является ли узел "распределительной коробкой"
            // Например, можно считать узлом "распределительной коробкой", если он связан с определёнными типами элементов или имеет определённые атрибуты
            return node.Degree > 2; // Пример: узел с более чем 2 связями может быть распределительной коробкой
        }

    }
}
