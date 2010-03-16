
using System;
using Irony.Ast;

namespace Hyperion.Core.Parser
{


    public sealed class PrintVisitor : IAstVisitor
    {
        int indentation = 0;
        public void BeginVisit (AstNode node)
        {
            for (int i = 0; i < indentation; i++)
            {
                Console.Write ("\t");
            }
            Console.WriteLine (node.ToString ());
            indentation++;
        }

        public void EndVisit (AstNode node)
        {
            indentation--;
        }
    }
}
