
using System;
using Irony.Ast;
using Hyperion.Core.Tools;

namespace Hyperion.Core.Parser.Nodes
{
    public class TextureNode : AstNode
    {
        public AstNode _pluginName;
        public AstNode _parameters;
        public AstNode _paramOne;
        public AstNode _paramTwo;

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);

            _pluginName = AddChild ("plugin", treeNode.ChildNodes[1]);
            _paramOne = AddChild ("param-one", treeNode.ChildNodes[2]);
            _paramTwo = AddChild ("param-two", treeNode.ChildNodes[3]);
            _parameters = AddChild ("parameters", treeNode.ChildNodes[4]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            _pluginName.Evaluate (context, AstMode.Read);
            _parameters.Evaluate (context, AstMode.Read);
            _paramOne.Evaluate (context, AstMode.Read);
            _paramTwo.Evaluate (context, AstMode.Read);
            
            ParamListContentNode node = _parameters as ParamListContentNode;
            ParameterSet parameterSet = node._parameterSet;
            string plugin = (_pluginName as Irony.Ast.LiteralValueNode).Value as string;
            string n1 = (_paramOne as Irony.Ast.LiteralValueNode).Value as string;
            string n2 = (_paramTwo as Irony.Ast.LiteralValueNode).Value as string;

            Api.Texture (plugin, n1, n2, parameterSet);
        }
    }
}
