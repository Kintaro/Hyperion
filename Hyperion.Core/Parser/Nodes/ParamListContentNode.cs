
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser.Nodes
{
    public class ParamListContentNode : AstNode
    {
        public AstNode[] _parameters;
        public Tools.ParameterSet _parameterSet;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);
            _parameters = new AstNode[treeNode.ChildNodes.Count];
            
            for (int i = 0; i < treeNode.ChildNodes.Count; ++i)
                _parameters[i] = AddChild ("parameter #" + i, treeNode.ChildNodes[i]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _parameterSet = new Hyperion.Core.Tools.ParameterSet ();
            
            for (int i = 0; i < _parameters.Length; ++i)
            {
                _parameters[i].Evaluate (context, AstMode.Read);
                _parameterSet.AddNode (_parameters[i]);
            }
        }
    }
}
