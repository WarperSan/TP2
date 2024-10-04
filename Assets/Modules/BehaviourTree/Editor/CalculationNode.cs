using BehaviourTree.Nodes;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class CalculationNode
    {
        public float x;
        public float y;
        public Node node;
        public readonly List<CalculationNode> children = new();
        public bool hideChildren;

        public static CalculationNode Create(Node node)
        {
            var calcNode = new CalculationNode { node = node };

            // Add children
            foreach (Node child in node)
                calcNode.children.Add(Create(child));

            return calcNode;
        }
    }
}