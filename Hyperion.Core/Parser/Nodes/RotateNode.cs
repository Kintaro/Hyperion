
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class RotateNode : AstNode
    {
        public AstNode _x;
        public AstNode _y;
        public AstNode _z;
        public AstNode _angle;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);

            _x = AddChild ("x", treeNode.ChildNodes[2]);
            _y = AddChild ("y", treeNode.ChildNodes[3]);
            _z = AddChild ("z", treeNode.ChildNodes[4]);
            _angle = AddChild ("angle", treeNode.ChildNodes[1]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _x.Evaluate (context, AstMode.Read);
            _y.Evaluate (context, AstMode.Read);
            _z.Evaluate (context, AstMode.Read);
            _angle.Evaluate (context, AstMode.Read);

            double angle = Convert.ToDouble (context.Data[0]);
            double x = Convert.ToDouble (context.Data[3]);
            double y = Convert.ToDouble (context.Data[2]);
            double z = Convert.ToDouble (context.Data[1]);

            Api.Rotate (angle, x, y, z);
        }
        
    }
}
