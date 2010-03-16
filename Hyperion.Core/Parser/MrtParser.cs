
using System;
using Irony.Ast;
using Irony.Parsing;
using Irony.Interpreter;

namespace Hyperion.Core.Parser
{
    /// <summary>
    ///
    /// </summary>
    public sealed class MrtParser
    {
        /// <summary>
        ///
        /// </summary>
        private MrtGrammar _grammar = new MrtGrammar ();

        /// <summary>
        ///
        /// </summary>
        /// <param name="sceneFile">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Boolean"/>
        /// </returns>
        public bool Parse (string sceneFile)
        {
            Console.WriteLine ("Reading file {0}", sceneFile);
            Irony.Parsing.LanguageData languageData = new Irony.Parsing.LanguageData (_grammar);
            Irony.Parsing.Parser parser = new Irony.Parsing.Parser (languageData);
            LanguageRuntime runtime = new LanguageRuntime (languageData);

            System.Text.StringBuilder builder = new System.Text.StringBuilder ();
            System.IO.StreamReader reader = System.IO.File.OpenText (sceneFile);
            builder.Append (reader.ReadToEnd ());

            Console.WriteLine ("Parsing contents");
            ParseTree tree = parser.Parse (builder.ToString ());
            Console.WriteLine ("Status: {0}", tree.Status);
            var node = tree.Root.AstNode as AstNode;
            node.Evaluate (new EvaluationContext (runtime), AstMode.Read);
            
            return true;
        }
    }
}
