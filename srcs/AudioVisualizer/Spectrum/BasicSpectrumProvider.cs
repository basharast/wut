using System;
using System.Collections.Generic;
using Visualizer.UI.DSP;

namespace Visualizer.UI.Spectrum
{
    /// <summary>
    ///     BasicSpectrumProvider
    /// </summary>
    public class BasicSpectrumProvider : FftProvider, ISpectrumProvider
    {
        private readonly int _sampleRate;
        private readonly List<object> _contexts = new List<object>();

        public BasicSpectrumProvider(int channels, int sampleRate, FftSize fftSize)
            : base(channels, fftSize)
        {
            try
            {
                if (sampleRate <= 0)
                    throw new ArgumentOutOfRangeException(nameof(sampleRate));
                _sampleRate = sampleRate;
            }
            catch (Exception ex)
            {

            }
        }

        public int GetFftFrequencyIndex(int frequency)
        {
            try
            {
                int fftSize = (int)FftSize;
                double f = _sampleRate / 2.0;
                // ReSharper disable once PossibleLossOfFraction
                return (int)((frequency / f) * (fftSize / 2));
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool GetFftData(float[] fftResultBuffer, object context)
        {
            try
            {
                if (_contexts.Contains(context))
                    return false;

                _contexts.Add(context);
                GetFftData(fftResultBuffer);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public override void Add(float[] samples, int count)
        {
            try
            {
                base.Add(samples, count);
                if (count > 0)
                    _contexts.Clear();
            }
            catch (Exception ex)
            {

            }
        }

        public override void Add(float left, float right)
        {
            try
            {
                base.Add(left, right);
                _contexts.Clear();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
