
using System;
using Irony.Ast;
using Hyperion.Core.Tools;

namespace Hyperion.Core.Parser.Nodes
{
    public class ConcatTransformNode : AstNode
    {
        public AstNode[] _numberNodes = new AstNode[16];
        public double[] _numbers = new double[16];

        public override void Init (Irony.Parsing.ParsingContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init (context, treeNode);

            for (int i = 0; i < 16; ++i)
                _numberNodes[i] = AddChild ("p" + i, treeNode.ChildNodes[1 + i]);
        }

        public override void Evaluate (Irony.Interpreter.EvaluationContext context, AstMode mode)
        {
            for (int i = 0; i < 16; ++i)
            {
                _numberNodes[i].Evaluate (context, AstMode.Read);
                _numbers[i] = Convert.ToDouble((_numberNodes[i] as LiteralValueNode).Value);
            }

            Api.ConcatTransform (_numbers);
        }
    }
}
