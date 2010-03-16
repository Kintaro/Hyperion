
using System;
using Irony.Ast;
using Hyperion.Core.Tools;

namespace Hyperion.Core.Parser.Nodes
{
    public class VolumeIntegratorNode : AstNode
    {
        public AstNode _pluginName;
        public AstNode _parameters;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);
            
            _pluginName = AddChild ("plugin", treeNode.ChildNodes[1]);
            _parameters = AddChild ("parameters", treeNode.ChildNodes[2]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _pluginName.Evaluate (context, AstMode.Read);
            _parameters.Evaluate (context, AstMode.Read);
            
            ParameterSet parameterSet = new Hyperion.Core.Tools.ParameterSet ();
            string plugin = (_pluginName as Irony.Ast.LiteralValueNode).Value as string;
            
            Api.VolumeIntegrator (plugin, parameterSet);
        }
    }
}
