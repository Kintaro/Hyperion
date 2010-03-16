
using System.Collections.Generic;
using Irony.Ast;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Tools
{
    /// <summary>
    ///
    /// </summary>
    public sealed class ParameterSet
    {
        /// <summary>
        ///
        /// </summary>
        private List<ParameterSetItem<int>> integers = new List<ParameterSetItem<int>> ();
        /// <summary>
        ///
        /// </summary>
        private List<ParameterSetItem<bool>> booleans = new List<ParameterSetItem<bool>> ();
        /// <summary>
        ///
        /// </summary>
        private List<ParameterSetItem<double>> doubles = new List<ParameterSetItem<double>> ();
        /// <summary>
        ///
        /// </summary>
        private List<ParameterSetItem<Vector>> vectors = new List<ParameterSetItem<Vector>> ();
        /// <summary>
        ///
        /// </summary>
        private List<ParameterSetItem<Normal>> normals = new List<ParameterSetItem<Normal>> ();
        /// <summary>
        ///
        /// </summary>
        private List<ParameterSetItem<Spectrum>> spectra = new List<ParameterSetItem<Spectrum>> ();
        /// <summary>
        ///
        /// </summary>
        private List<ParameterSetItem<Point>> points = new List<ParameterSetItem<Point>> ();
        /// <summary>
        ///
        /// </summary>
        private List<ParameterSetItem<string>> strings = new List<ParameterSetItem<string>> ();
        /// <summary>
        ///
        /// </summary>
        private List<ParameterSetItem<string>> textures = new List<ParameterSetItem<string>> ();

        /// <summary>
        ///
        /// </summary>
        /// <param name="node">
        /// A <see cref="AstNode"/>
        /// </param>
        public void AddNode (AstNode node)
        {
            Parser.Nodes.ParamListEntryNode paramNode = node as Parser.Nodes.ParamListEntryNode;
            Parser.Nodes.ArrayNode arrayNode = paramNode._array as Parser.Nodes.ArrayNode;
            Parser.Nodes.NumberArrayNode numberNode = arrayNode._parameters as Parser.Nodes.NumberArrayNode;
            
            string name = paramNode._name;
            if (numberNode != null)
                AddNumberNode (name, numberNode);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="node">
        /// A <see cref="Parser.Nodes.NumberArrayNode"/>
        /// </param>
        private void AddNumberNode (string name, Parser.Nodes.NumberArrayNode node)
        {
            string type = name.Split (' ')[0];
            string parameterName = name.Split(' ')[1].Trim ();
            
            switch (type)
            {
            case "integer":
                int[] numbers = ConvertToInt (node._numbers);
                AddInt (parameterName, numbers);
                break;
            case "float":
                AddDouble (parameterName, node._numbers);
                break;
            case "double":
                goto case "float";
            case "color":
                    Spectrum[] colors = new Spectrum[node._numbers.Length / 3];
                    for (int i = 0; i < node._numbers.Length / 3; ++i)
                        colors[i] = new Spectrum (node._numbers[3 * i], node._numbers[3 * i + 1], node._numbers[3 * i + 2]);
                    AddSpectrum (parameterName, colors);
                    break;
            case "point":
                Point[] p = new Point[node._numbers.Length / 3];
                for (int i = 0; i < node._numbers.Length / 3; ++i)
                    p[i] = new Point (node._numbers[3 * i], node._numbers[3 * i + 1], node._numbers[3 * i + 2]);
                AddPoint (parameterName, p);
                break;
            case "vector":
                Vector[] v = new Vector[node._numbers.Length / 3];
                for (int i = 0; i < node._numbers.Length / 3; ++i)
                    v[i] = new Vector (node._numbers[3 * i], node._numbers[3 * i + 1], node._numbers[3 * i + 2]);
                AddVector (parameterName, v);
                break;
            case "normal":
                Normal[] n = new Normal[node._numbers.Length / 3];
                for (int i = 0; i < node._numbers.Length / 3; ++i)
                    n[i] = new Normal (node._numbers[3 * i], node._numbers[3 * i + 1], node._numbers[3 * i + 2]);
                AddNormal (parameterName, n);
                break;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="node">
        /// A <see cref="Parser.Nodes.StringArrayNode"/>
        /// </param>
        private void AddStringNode (string name, Parser.Nodes.StringArrayNode node)
        {
            AddString (name, node._strings);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="numbers">
        /// A <see cref="System.Double[]"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32[]"/>
        /// </returns>
        private int[] ConvertToInt (double[] numbers)
        {
            int[] result = new int[numbers.Length];
            for (int i = 0; i < numbers.Length; ++i)
                result[i] = System.Convert.ToInt32 (numbers[i]);
            return result;
        }

        /// <summary>
        ///     Adds a new parameter to the list
        /// </summary>
        /// <param name="name">
        ///     The parameter's name
        /// </param>
        /// <param name="data">
        ///     The parameter's content
        /// </param>
        public void AddDouble (string name, double[] data)
        {
            EraseDouble (name);
            AddParamType<double> (name, doubles, data);
        }

        /// <summary>
        ///     Adds a new parameter to the list
        /// </summary>
        /// <param name="name">
        ///     The parameter's name
        /// </param>
        /// <param name="data">
        ///     The parameter's content
        /// </param>
        public void AddInt (string name, int[] data)
        {
            EraseInt (name);
            AddParamType<int> (name, integers, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="data">
        /// A <see cref="System.String[]"/>
        /// </param>
        public void AddString (string name, string[] data)
        {
            EraseString (name);
            AddParamType<string> (name, strings, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="data">
        /// A <see cref="Spectrum[]"/>
        /// </param>
        public void AddSpectrum (string name, Spectrum[] data)
        {
            EraseSpectrum (name);
            AddParamType<Spectrum> (name, spectra, data);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="data">
        /// A <see cref="Point[]"/>
        /// </param>
        public void AddPoint (string name, Point[] data)
        {
            EraseSpectrum (name);
            AddParamType<Point> (name, points, data);
        }

        public void AddVector (string name, Vector[] data)
        {
            AddParamType<Vector> (name, vectors, data);
        }

        public void AddNormal (string name, Normal[] data)
        {
            AddParamType<Normal> (name, normals, data);
        }

        /// <summary>
        ///     Erase the named parameter
        /// </summary>
        /// <param name="name">
        ///     The parameter's name
        /// </param>
        public void EraseDouble (string name)
        {
            foreach (ParameterSetItem<double> parameter in doubles)
            {
                if (parameter.Name == name)
                {
                    doubles.Remove (parameter);
                    return;
                }
            }
        }

        /// <summary>
        ///     Erase the named parameter
        /// </summary>
        /// <param name="name">
        ///     The parameter's name
        /// </param>
        public void EraseInt (string name)
        {
            foreach (ParameterSetItem<int> parameter in integers)
            {
                if (parameter.Name == name)
                {
                    integers.Remove (parameter);
                    return;
                }
            }
        }

        /// <summary>
        ///     Erase the named parameter
        /// </summary>
        /// <param name="name">
        ///     The parameter's name
        /// </param>
        public void EraseString (string name)
        {
            foreach (ParameterSetItem<string> parameter in strings)
            {
                if (parameter.Name == name)
                {
                    strings.Remove (parameter);
                    return;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        public void EraseSpectrum (string name)
        {
            foreach (ParameterSetItem<Spectrum> parameter in spectra)
            {
                if (parameter.Name == name)
                {
                    spectra.Remove (parameter);
                    return;
                }
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="numberOfItems">
        /// A <see cref="System.Int32"/>
        /// </param>
        public double[] FindDouble (string name, ref int numberOfItems)
        {
            return LookupPtr<double> (name, doubles, ref numberOfItems);
        }

        public int[] FindInt (string name, ref int numberOfItems)
        {
            return LookupPtr<int> (name, integers, ref numberOfItems);
        }

        public Vector[] FindVector (string name, ref int numberOfItems)
        {
            return LookupPtr<Vector> (name, vectors, ref numberOfItems);
        }

        public Point[] FindPoint (string name, ref int numberOfItems)
        {
            return LookupPtr<Point> (name, points, ref numberOfItems);
        }

        public Normal[] FindNormal (string name, ref int numberOfItems)
        {
            return LookupPtr<Normal> (name, normals, ref numberOfItems);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/>
        /// </returns>
        public string FindTexture (string name)
        {
            return LookupOne<string> (name, textures, "");
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="def">
        /// A <see cref="System.Double"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Double"/>
        /// </returns>
        public double FindOneDouble (string name, double def)
        {
            return LookupOne<double> (name, doubles, def);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="def">
        /// A <see cref="Point"/>
        /// </param>
        /// <returns>
        /// A <see cref="Point"/>
        /// </returns>
        public Point FindOnePoint (string name, Point def)
        {
            return LookupOne<Point> (name, points, def);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="def">
        /// A <see cref="Spectrum"/>
        /// </param>
        /// <returns>
        /// A <see cref="Spectrum"/>
        /// </returns>
        public Spectrum FindOneSpectrum (string name, Spectrum def)
        {
            return LookupOne<Spectrum> (name, spectra, def);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="def">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Double"/>
        /// </returns>
        public int FindOneInt (string name, int def)
        {
            return LookupOne<int> (name, integers, def);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="def">
        /// A <see cref="System.String"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public string FindOneString (string name, string def)
        {
            return LookupOne<string> (name, strings, def);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="def">
        /// A <see cref="System.Boolean"/>
        /// </param>
        /// <returns>
        /// A <see cref="System.Int32"/>
        /// </returns>
        public bool FindOneBool (string name, bool def)
        {
            return LookupOne<bool> (name, booleans, def);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="list">
        /// A <see cref="List<ParameterSetItem<System.Double>>"/>
        /// </param>
        /// <param name="data">
        /// A <see cref="T[]"/>
        /// </param>
        private void AddParamType<T> (string name, List<ParameterSetItem<T>> list, T[] data)
        {
            list.Add (new ParameterSetItem<T> (name, data));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="list">
        /// A <see cref="List<ParameterSetItem<T>>"/>
        /// </param>
        /// <param name="numberOfItems">
        /// A <see cref="System.Int32[]"/>
        /// </param>
        /// <returns>
        /// A <see cref="T[]"/>
        /// </returns>
        private T[] LookupPtr<T> (string name, List<ParameterSetItem<T>> list, ref int numberOfItems)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                ParameterSetItem<T> parameter = list[i];
                if (parameter.Name == name)
                {
                    numberOfItems = parameter.NumberOfItems;
                    parameter.LookedUp = true;
                    return parameter.Data;
                }
            }
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/>
        /// </param>
        /// <param name="list">
        /// A <see cref="List<ParameterSetItem<T>>"/>
        /// </param>
        /// <returns>
        /// A <see cref="T"/>
        /// </returns>
        private T LookupOne<T> (string name, List<ParameterSetItem<T>> list, T def)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                ParameterSetItem<T> parameter = list[i];
                if (parameter.Name == name && parameter.NumberOfItems == 1)
                {
                    parameter.LookedUp = true;
                    return parameter.Data[0];
                }
            }
            return def;
        }
    }
}
