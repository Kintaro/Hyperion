
using System;
using System.Collections.Generic;
using Hyperion.Core.Interfaces;
using Hyperion.Core.Geometry;

namespace Hyperion.Core.Tools
{
    public sealed class ConstantTexture<T> : ITexture<T>
    {
        /// <summary>
        ///
        /// </summary>
        private T _value;

        /// <summary>
        ///
        /// </summary>
        public ConstantTexture (T val)
        {
            _value = val;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="geom">
        /// A <see cref="DifferentialGeometry"/>
        /// </param>
        /// <returns>
        /// A <see cref="T"/>
        /// </returns>
        public T Evaluate (DifferentialGeometry geom)
        {
            return _value;
        }
    }

    public sealed class TextureParameterSet
    {
        private ParameterSet GeometryParameters;
        private ParameterSet MaterialParameters;
        private Dictionary<string, ITexture<double>> DoubleTextures;
        private Dictionary<string, ITexture<Spectrum>> SpectrumTextures;

        public TextureParameterSet (ParameterSet geometryParameters, ParameterSet materialParameters,
            Dictionary<string, ITexture<double>> ft, Dictionary<string, ITexture<Spectrum>> st)
        {
            this.GeometryParameters = geometryParameters;
            this.MaterialParameters = materialParameters;
            this.DoubleTextures = ft;
            this.SpectrumTextures = st;
        }

        public ITexture<Spectrum> GetSpectrumTexture (string name, Spectrum def)
        {
            string n = GeometryParameters.FindTexture (name);
            if (n == "" || n == string.Empty || n == null)
                n = MaterialParameters.FindTexture (name);
            if (n != "" && n != string.Empty && n != null)
            {
                if (SpectrumTextures.ContainsKey (n))
                    return SpectrumTextures[n];
                else
                    Console.WriteLine ("Couldn't find spectrum texture named {0} for parameter {1}", n, name);
            }

            Spectrum val = GeometryParameters.FindOneSpectrum (name, MaterialParameters.FindOneSpectrum (name, def));
            return new ConstantTexture<Spectrum> (val);
        }

        public ITexture<double> GetDoubleTexture (string name, double def)
        {
            string n = GeometryParameters.FindTexture (name);
            if (n == "" || n == string.Empty || n == null)
                n = MaterialParameters.FindTexture (name);
            if (n == "" && n != string.Empty && n != null)
            {
                if (DoubleTextures.ContainsKey (n))
                    return DoubleTextures[n];
            }

            double val = GeometryParameters.FindOneDouble (name, MaterialParameters.FindOneDouble (name, def));
            return new ConstantTexture<double> (val);
        }

        public string FindString (string n)
        {
            return FindString (n, "");
        }

        public string FindString (string n, string def)
        {
            return GeometryParameters.FindOneString (n, def);
        }
    }
}
