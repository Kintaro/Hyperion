
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class NumberArrayNode : AstNode
    {
        public AstNode[] _parameters;
        public double[] _numbers;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);
            
            _parameters = new AstNode[treeNode.ChildNodes.Count];
            
            for (int i = 0; i < treeNode.ChildNodes.Count; ++i)
            {
                _parameters[i] = AddChild ("#" + i, treeNode.ChildNodes[i]);
            }
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _numbers = new double[_parameters.Length];

            foreach (AstNode node in _parameters)
                node.Evaluate (context, AstMode.Read);
            for (int i = _parameters.Length - 1; i >= 0; --i)
                _numbers[i] = Convert.ToDouble (context.Data.Pop ());
        }
    }
}
