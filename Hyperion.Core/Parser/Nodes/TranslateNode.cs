
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class TranslateNode : AstNode
    {
        public AstNode _x;
        public AstNode _y;
        public AstNode _z;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);
            
            _x = AddChild ("x", treeNode.ChildNodes[1]);
            _y = AddChild ("y", treeNode.ChildNodes[2]);
            _z = AddChild ("z", treeNode.ChildNodes[3]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _x.Evaluate (context, AstMode.Read);
            _y.Evaluate (context, AstMode.Read);
            _z.Evaluate (context, AstMode.Read);
            
            double x = Convert.ToDouble (context.Data[2]);
            double y = Convert.ToDouble (context.Data[1]);
            double z = Convert.ToDouble (context.Data[0]);
            
            Api.Translate (x, y, z);
        }
        
    }
}
