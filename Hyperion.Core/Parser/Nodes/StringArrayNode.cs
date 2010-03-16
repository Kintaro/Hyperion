
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class StringArrayNode : AstNode
    {
        public AstNode[] _parameters;
        public string[] _strings;

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
            _strings = new string[_parameters.Length];
            int i = _parameters.Length - 1;

            foreach (AstNode node in _parameters)
                node.Evaluate (context, AstMode.Read);

            while (context.Data.Count > 1)
                _strings[i] = Convert.ToString (context.Data.Pop ());
        }
    }
}
