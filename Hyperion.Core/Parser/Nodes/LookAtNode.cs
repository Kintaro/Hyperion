
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class LookAtNode : AstNode
    {
        public AstNode _x;
        public AstNode _y;
        public AstNode _z;

        public AstNode _ux;
        public AstNode _uy;
        public AstNode _uz;

        public AstNode _dx;
        public AstNode _dy;
        public AstNode _dz;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);
            
            _x = AddChild ("x", treeNode.ChildNodes[1]);
            _y = AddChild ("y", treeNode.ChildNodes[2]);
            _z = AddChild ("z", treeNode.ChildNodes[3]);

            _ux = AddChild ("ux", treeNode.ChildNodes[4]);
            _uy = AddChild ("uy", treeNode.ChildNodes[5]);
            _uz = AddChild ("uz", treeNode.ChildNodes[6]);

            _dx = AddChild ("dx", treeNode.ChildNodes[7]);
            _dy = AddChild ("dy", treeNode.ChildNodes[8]);
            _dz = AddChild ("dz", treeNode.ChildNodes[9]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _x.Evaluate (context, AstMode.Read);
            _y.Evaluate (context, AstMode.Read);
            _z.Evaluate (context, AstMode.Read);
            _dx.Evaluate (context, AstMode.Read);
            _dy.Evaluate (context, AstMode.Read);
            _dz.Evaluate (context, AstMode.Read);
            _ux.Evaluate (context, AstMode.Read);
            _uy.Evaluate (context, AstMode.Read);
            _uz.Evaluate (context, AstMode.Read);

            double x = Convert.ToDouble (context.Data[8]);
            double y = Convert.ToDouble (context.Data[7]);
            double z = Convert.ToDouble (context.Data[6]);

            double ux = Convert.ToDouble (context.Data[2]);
            double uy = Convert.ToDouble (context.Data[1]);
            double uz = Convert.ToDouble (context.Data[0]);

            double dx = Convert.ToDouble (context.Data[5]);
            double dy = Convert.ToDouble (context.Data[4]);
            double dz = Convert.ToDouble (context.Data[3]);

            Api.LookAt (x, y, z, ux, uy, uz, dx, dy, dz);
        }
        
    }
}
