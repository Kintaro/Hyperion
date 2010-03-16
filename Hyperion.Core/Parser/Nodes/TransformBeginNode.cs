
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class TransformBeginNode : AstNode
    {
        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            Api.TransformBegin ();
        }
    }
}
