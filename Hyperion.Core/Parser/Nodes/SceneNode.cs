
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class SceneNode : AstNode
    {
        public string FileName;
        public AstNode Argument;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {

        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {

        }
        
    }
}
