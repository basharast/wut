﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Visualizer.UI.DSP;

namespace Visualizer.UI.Spectrum
{
    public class SpectrumBase : INotifyPropertyChanged
    {
        private const int ScaleFactorLinear = 9;
        protected const int ScaleFactorSqr = 2;
        protected const double MinDbValue = -90;
        protected const double MaxDbValue = 0;
        protected const double DbScale = (MaxDbValue - MinDbValue);

        private int _fftSize;
        private bool _isXLogScale;
        private int _maxFftIndex;
        private int _maximumFrequency = 20000;
        private int _maximumFrequencyIndex;
        private int _minimumFrequency = 20; //Default spectrum from 20Hz to 20kHz
        private int _minimumFrequencyIndex;
        private ScalingStrategy _scalingStrategy;
        private int[] _spectrumIndexMax;
        private int[] _spectrumLogScaleIndexMax;
        private ISpectrumProvider _spectrumProvider;

        protected int SpectrumResolution;
        private bool _useAverage;

        public int MaximumFrequency
        {
            get { return _maximumFrequency; }
            set
            {
                try
                {
                    if (value <= MinimumFrequency)
                    {
                        throw new ArgumentOutOfRangeException("value",
                            "Value must not be less or equal the MinimumFrequency.");
                    }
                    _maximumFrequency = value;
                    UpdateFrequencyMapping();

                    RaisePropertyChanged("MaximumFrequency");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public int MinimumFrequency
        {
            get { return _minimumFrequency; }
            set
            {
                try
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException("value");
                    _minimumFrequency = value;
                    UpdateFrequencyMapping();

                    RaisePropertyChanged("MinimumFrequency");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public ISpectrumProvider SpectrumProvider
        {
            get { return _spectrumProvider; }
            set
            {
                try
                {
                    if (value == null)
                        throw new ArgumentNullException("value");
                    _spectrumProvider = value;

                    RaisePropertyChanged("SpectrumProvider");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public bool IsXLogScale
        {
            get { return _isXLogScale; }
            set
            {
                try
                {
                    _isXLogScale = value;
                    UpdateFrequencyMapping();
                    RaisePropertyChanged("IsXLogScale");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public ScalingStrategy ScalingStrategy
        {
            get { return _scalingStrategy; }
            set
            {
                try
                {
                    _scalingStrategy = value;
                    RaisePropertyChanged("ScalingStrategy");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public bool UseAverage
        {
            get { return _useAverage; }
            set
            {
                try
                {
                    _useAverage = value;
                    RaisePropertyChanged("UseAverage");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public FftSize FftSize
        {
            get { return (FftSize)_fftSize; }
            protected set
            {
                try
                {
                    if ((int)Math.Log((int)value, 2) % 1 != 0)
                        throw new ArgumentOutOfRangeException("value");

                    _fftSize = (int)value;
                    _maxFftIndex = _fftSize / 2 - 1;

                    RaisePropertyChanged("FFTSize");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void UpdateFrequencyMapping()
        {
            try
            {
                _maximumFrequencyIndex = Math.Min(_spectrumProvider.GetFftFrequencyIndex(MaximumFrequency) + 1, _maxFftIndex);
                _minimumFrequencyIndex = Math.Min(_spectrumProvider.GetFftFrequencyIndex(MinimumFrequency), _maxFftIndex);

                int actualResolution = SpectrumResolution;

                int indexCount = _maximumFrequencyIndex - _minimumFrequencyIndex;
                double linearIndexBucketSize = Math.Round(indexCount / (double)actualResolution, 3);

                _spectrumIndexMax = _spectrumIndexMax.CheckBuffer(actualResolution, true);
                _spectrumLogScaleIndexMax = _spectrumLogScaleIndexMax.CheckBuffer(actualResolution, true);

                double maxLog = Math.Log(actualResolution, actualResolution);
                for (int i = 1; i < actualResolution; i++)
                {
                    int logIndex =
                        (int)((maxLog - Math.Log((actualResolution + 1) - i, (actualResolution + 1))) * indexCount) +
                        _minimumFrequencyIndex;

                    _spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (int)(i * linearIndexBucketSize);
                    _spectrumLogScaleIndexMax[i - 1] = logIndex;
                }

                if (actualResolution > 0)
                {
                    _spectrumIndexMax[_spectrumIndexMax.Length - 1] =
                        _spectrumLogScaleIndexMax[_spectrumLogScaleIndexMax.Length - 1] = _maximumFrequencyIndex;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public virtual SpectrumPointData[] CalculateSpectrumPoints(double maxValue, float[] fftBuffer)
        {
            try
            {
                var dataPoints = new List<SpectrumPointData>();

                double value0 = 0, value = 0;
                double lastValue = 0;
                double actualMaxValue = maxValue;
                int spectrumPointIndex = 0;

                for (int i = _minimumFrequencyIndex; i <= _maximumFrequencyIndex; i++)
                {
                    switch (ScalingStrategy)
                    {
                        case ScalingStrategy.Decibel:
                            value0 = (((20 * Math.Log10(fftBuffer[i])) - MinDbValue) / DbScale) * actualMaxValue;
                            break;
                        case ScalingStrategy.Linear:
                            value0 = (fftBuffer[i] * ScaleFactorLinear) * actualMaxValue;
                            break;
                        case ScalingStrategy.Sqrt:
                            value0 = ((Math.Sqrt(fftBuffer[i])) * ScaleFactorSqr) * actualMaxValue;
                            break;
                    }

                    bool recalc = true;

                    value = Math.Max(0, Math.Max(value0, value));

                    while (spectrumPointIndex <= _spectrumIndexMax.Length - 1 &&
                           i ==
                           (IsXLogScale
                               ? _spectrumLogScaleIndexMax[spectrumPointIndex]
                               : _spectrumIndexMax[spectrumPointIndex]))
                    {
                        if (!recalc)
                            value = lastValue;

                        if (value > maxValue)
                            value = maxValue;

                        if (_useAverage && spectrumPointIndex > 0)
                            value = (lastValue + value) / 2.0;

                        dataPoints.Add(new SpectrumPointData { SpectrumPointIndex = spectrumPointIndex, Value = value });

                        lastValue = value;
                        value = 0.0;
                        spectrumPointIndex++;
                        recalc = false;
                    }

                    //value = 0;
                }

                return dataPoints.ToArray();
            }
            catch (Exception ex)
            {
                return new SpectrumPointData[] { };
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            try
            {
                if (PropertyChanged != null && !String.IsNullOrEmpty(propertyName))
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
            catch (Exception ex)
            {

            }
        }

        public struct SpectrumPointData
        {
            public int SpectrumPointIndex;
            public double Value;
        }
    }

}
