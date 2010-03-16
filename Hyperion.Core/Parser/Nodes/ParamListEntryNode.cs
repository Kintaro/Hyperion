
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class ParamListEntryNode : AstNode
    {
        public AstNode _paramName;
        public AstNode _array;
        public string _name;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);
            
            _paramName = AddChild ("name", treeNode.ChildNodes[0]);
            _array = AddChild ("array", treeNode.ChildNodes[1]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _paramName.Evaluate (context, AstMode.Read);
            _array.Evaluate (context, AstMode.Read);
            
            _name = (_paramName as Irony.Ast.LiteralValueNode).Value as string;
        }
    }
}
