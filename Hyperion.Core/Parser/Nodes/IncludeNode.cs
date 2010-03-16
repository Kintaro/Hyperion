
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class IncludeNode : AstNode
    {
        public AstNode Argument;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);

            Argument = AddChild ("IncludeFile", treeNode.ChildNodes[1]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            Argument.Evaluate (context, AstMode.Read);

            Console.WriteLine ("Including file {0}", context.Data[0]);
        }

    }
}
