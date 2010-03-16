
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class ArrayNode : AstNode
    {
        public AstNode _parameters;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);
            
            _parameters = AddChild ("array", treeNode.ChildNodes[1].ChildNodes[0]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _parameters.Evaluate (context, AstMode.Read);
        }
    }
}
