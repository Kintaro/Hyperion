
using System;
using Irony.Ast;
using Hyperion.Core.Tools;

namespace Hyperion.Core.Parser.Nodes
{
    public class CoordSysTransformNode : AstNode
    {
        public AstNode _name;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);

            _name = AddChild ("plugin", treeNode.ChildNodes[1]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _name.Evaluate (context, AstMode.Read);
            
            string name = (_name as Irony.Ast.LiteralValueNode).Value as string;
            
            Api.CoordSysTransform (name);
        }
    }
}
