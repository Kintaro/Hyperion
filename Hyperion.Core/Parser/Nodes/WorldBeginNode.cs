
using System;
using Irony.Ast;
using Hyperion.Core.Tools;

namespace Hyperion.Core.Parser.Nodes
{
    public class WorldBeginNode : AstNode
    {
        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            Api.WorldBegin ();
        }
    }
}
