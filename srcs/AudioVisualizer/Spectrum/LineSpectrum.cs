using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Visualizer.UI.DSP;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI.Xaml;
using WinUniversalTool;

namespace Visualizer.UI.Spectrum
{
    public class LineSpectrum : SpectrumBase
    {
        private int _barCount;
        private double _barSpacing;
        private double _barWidth;
        private Size _currentSize;
        public LineSpectrum(FftSize fftSize)
        {
            FftSize = fftSize;
        }

        public double BarWidth => _barWidth;

        public double BarSpacing
        {
            get => _barSpacing;
            set
            {
                try
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException(nameof(value));
                    _barSpacing = value;
                    UpdateFrequencyMapping();

                    RaisePropertyChanged("BarSpacing");
                    RaisePropertyChanged("BarWidth");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public int BarCount
        {
            get => _barCount;
            set
            {
                try
                {
                    if (value <= 0)
                        throw new ArgumentOutOfRangeException(nameof(value));
                    _barCount = value;
                    SpectrumResolution = value;
                    UpdateFrequencyMapping();

                    RaisePropertyChanged("BarCount");
                    RaisePropertyChanged("BarWidth");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public Size CurrentSize
        {
            get => _currentSize;
            protected set
            {
                try
                {
                    _currentSize = value;
                    RaisePropertyChanged("CurrentSize");
                }
                catch (Exception ex)
                {

                }
            }
        }

        public bool isPlaying = false;
        public bool isActive = false;
        public void CreateSpectrumLine(ICanvasAnimatedControl canvas, CanvasDrawingSession ds)
        {
            try
            {
                if (!isPlaying || !isActive)
                {
                    return;
                }
                var size = canvas.Size;
                if (!UpdateFrequencyMappingIfNessesary(size))
                    return;

                var fftBuffer = new float[(int)FftSize];

                //get the fft result from the spectrum provider
                if (SpectrumProvider.GetFftData(fftBuffer))
                {
                    CreateSpectrumLineInternal(canvas, ds, fftBuffer, size);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void CreateSpectrumLineInternal(ICanvasAnimatedControl canvas, CanvasDrawingSession ds, float[] fftBuffer, Size size)
        {
            try
            {
                var height = (float)size.Height;
                var width = (float)size.Width;
                //prepare the fft result for rendering 
                SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(height / 2, fftBuffer);
                var c1 = Colors.Green;
                var c2 = Colors.Red;

                try
                {
                    c1.A = (byte)Math.Round((255 * (MainPage.BarOpacity / 100.0)));
                    c2.A = (byte)Math.Round((255 * (MainPage.BarOpacity / 100.0)));
                }
                catch (Exception ex)
                {
                    c1.A = 200;
                    c2.A = 200;
                }
                using (var brush = new CanvasLinearGradientBrush(canvas, c1, c2))
                {
                    //connect the calculated points with lines
                    for (int i = 0; i < spectrumPoints.Length; i++)
                    {
                        SpectrumPointData p = spectrumPoints[i];
                        int barIndex = p.SpectrumPointIndex;

                        var xCoord = (float)(BarSpacing * (barIndex + 1) + (_barWidth * barIndex) + _barWidth / 2);

                        var point0 = new Vector2(xCoord + (MainPage.AutoScaleDown ? 10: 0), height);
                        var point1 = new Vector2(xCoord + (MainPage.AutoScaleDown ? 10 : 0), height - (float)p.Value);

                        brush.StartPoint = point0;
                        brush.EndPoint = new Vector2(xCoord, 0);

                        ds.DrawLine(point0, point1, brush, (float)_barWidth);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public override void UpdateFrequencyMapping()
        {
            try
            {
                _barWidth = Math.Max(((_currentSize.Width / (MainPage.AutoScaleDown ? (BarCount == 1 ? 12 : (BarCount == 2 ? 8 : 6)) : 1) - (BarSpacing * (BarCount + 1))) / BarCount), 0.00001);
                base.UpdateFrequencyMapping();
            }
            catch (Exception ex)
            {

            }
        }

        public bool UpdateFrequencyMappingIfNessesary(Size newSize)
        {
            try
            {
                if (newSize != CurrentSize)
                {
                    CurrentSize = newSize;
                    UpdateFrequencyMapping();
                }

                return newSize.Width > 0 && newSize.Height > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }

}
