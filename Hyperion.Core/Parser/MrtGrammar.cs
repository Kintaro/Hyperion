
using System;
using Irony;
using Irony.Ast;
using Irony.Parsing;

namespace Hyperion.Core.Parser
{

    /// <summary>
    ///
    /// </summary>
    [Language("MRT Scene File Language", "1.0", "MRT Scene File Language")]
    public sealed class MrtGrammar : Grammar
    {
        /// <summary>
        ///
        /// </summary>
        public MrtGrammar () : base(true)
        {
            NonGrammarTerminals.Add (NewLine);
            
            #region Numbers Strings Identifiers
            var comment = new CommentTerminal ("comment", "#", "\n", "\r");
            var number = new NumberLiteral ("number", NumberOptions.AllowSign | NumberOptions.AllowStartEndDot);
            var str = new StringLiteral ("path", "\"");
            #endregion
            
            #region Transformations
            var lookAt = new NonTerminal ("look-at", typeof (Nodes.LookAtNode));
            var scale = new NonTerminal ("scale", typeof(Nodes.ScaleNode));
            var translate = new NonTerminal ("translate", typeof(Nodes.TranslateNode));
            var rotate = new NonTerminal ("rotate", typeof(Nodes.RotateNode));
            var reverseOrientation = new NonTerminal ("reverse-orientation", typeof(Nodes.ReverseOrientationNode));
            var coordSysTransform = new NonTerminal ("coord-sys-transform", typeof(Nodes.CoordSysTransformNode));
            var concatTransform = new NonTerminal ("concat-transform", typeof(Nodes.ConcatTransformNode));
            var transformations = new NonTerminal ("translate", typeof(StatementListNode));
            #endregion
            
            var include = new NonTerminal ("include", typeof(Nodes.IncludeNode));
            
            var attributeBegin = new NonTerminal ("attribute-begin", typeof(Nodes.AttributeBeginNode));
            var attributeEnd = new NonTerminal ("attribute-end", typeof(Nodes.AttributeEndNode));

            var worldBegin = new NonTerminal ("world-begin", typeof (Nodes.WorldBeginNode));
            var worldEnd = new NonTerminal ("world-end", typeof(Nodes.WorldEndNode));
            
            var transformBegin = new NonTerminal ("transform-begin", typeof(Nodes.TransformBeginNode));
            var transformEnd = new NonTerminal ("transform-end", typeof(Nodes.TransformEndNode));
            
            var paramList = new NonTerminal ("param-list");
            var paramListContents = new NonTerminal ("param-list-contents", typeof(Nodes.ParamListContentNode));
            var paramListEntry = new NonTerminal ("param-list-entry", typeof(Nodes.ParamListEntryNode));
            
            var array = new NonTerminal ("array", typeof(Nodes.ArrayNode));
            var stringArray = new NonTerminal ("string-array", typeof(Nodes.StringArrayNode));
            var numArray = new NonTerminal ("num-array", typeof(Nodes.NumberArrayNode));
            
            var camera = new NonTerminal ("camera", typeof(Nodes.CameraNode));
            var film = new NonTerminal ("film", typeof(Nodes.FilmNode));
            var sampler = new NonTerminal ("sampler", typeof(Nodes.SamplerNode));
            var surfaceIntegrator = new NonTerminal ("surface-integrator", typeof(Nodes.SurfaceIntegratorNode));
            var volumeIntegrator = new NonTerminal ("volume-integrator", typeof(Nodes.VolumeIntegratorNode));
            var texture = new NonTerminal ("texture", typeof(Nodes.TextureNode));
            var shape = new NonTerminal ("shape", typeof(Nodes.ShapeNode));
            var pixelFilter = new NonTerminal ("pixel-filter", typeof(Nodes.PixelFilterNode));
            var material = new NonTerminal ("material", typeof(Nodes.MaterialNode));
            var lightSource = new NonTerminal ("light", typeof(Nodes.LightNode));
            var areaLightSource = new NonTerminal ("area-light", typeof(Nodes.AreaLightSourceNode));
            var plugin = new NonTerminal ("plugin", typeof(StatementListNode));
            
            var statements = new NonTerminal ("statements", typeof(StatementListNode));
            var scene = new NonTerminal ("scene", typeof(StatementListNode));

            NonGrammarTerminals.Add (comment);
            
            #region Rules
            lookAt.Rule = "LookAt" + number + number + number + number + number + number + number + number + number;
            scale.Rule = "Scale" + number + number + number;
            translate.Rule = "Translate" + number + number + number;
            rotate.Rule = "Rotate" + number + number + number + number;
            reverseOrientation.Rule = "ReverseOrientation";
            coordSysTransform.Rule = "CoordSysTransform" + str;
            concatTransform.Rule = "ConcatTransform" + number + number + number + number + number + number + number + number + number + number + number + number + number + number + number + number;
            transformations.Rule = scale | translate | rotate | lookAt | coordSysTransform | concatTransform;
            include.Rule = "Include" + str;
            
            attributeBegin.Rule = "AttributeBegin";
            attributeEnd.Rule = "AttributeEnd";
            worldBegin.Rule = "WorldBegin";
            worldEnd.Rule = "WorldEnd";
            
            paramList.Rule = paramListContents;
            paramListContents.Rule = MakeStarRule (paramListContents, paramListEntry);
            paramListEntry.Rule = str + array;
            
            stringArray.Rule = MakeStarRule (stringArray, str);
            numArray.Rule = MakeStarRule (numArray, number);
            array.Rule = "[" + (stringArray | numArray) + "]";
            
            transformBegin.Rule = "TransformBegin";
            transformEnd.Rule = "TransformEnd";
            
            camera.Rule = "Camera" + str + paramListContents;
            film.Rule = "Film" + str + paramListContents;
            sampler.Rule = "Sampler" + str + paramListContents;
            surfaceIntegrator.Rule = "SurfaceIntegrator" + str + paramListContents;
            volumeIntegrator.Rule = "VolumeIntegrator" + str + paramListContents;
            shape.Rule = "Shape" + str + paramListContents;
            pixelFilter.Rule = "PixelFilter" + str + paramListContents;
            texture.Rule = "Texture" + str + str + str + paramListContents;
            material.Rule = "Material" + str + paramListContents;
            lightSource.Rule = "LightSource" + str + paramListContents;
            areaLightSource.Rule = "AreaLightSource" + str + paramListContents;
            plugin.Rule = camera | film | sampler | surfaceIntegrator | volumeIntegrator | shape | pixelFilter | texture | material | lightSource | areaLightSource;
            
            statements.Rule = transformations | include | attributeBegin | attributeEnd | paramListContents | plugin | statements | worldBegin | worldEnd | comment;
            scene.Rule = MakePlusRule (scene, statements);
            #endregion
            
            #region Config
            this.Root = scene;
            this.LanguageFlags = LanguageFlags.CreateAst;
            #endregion
        }
    }
}
