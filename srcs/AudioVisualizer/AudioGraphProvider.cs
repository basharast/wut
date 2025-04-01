using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Media;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Visualizer.UI.DSP;
using Visualizer.UI.Spectrum;
using WinUniversalTool;

namespace Visualizer.UI
{
    public class AudioGraphProvider : BindableBase, IAudioProvider
    {
        #region Fields
        private BasicSpectrumProvider _spectrumProvider;
        #endregion


        #region Properties

        public BasicSpectrumProvider SpectrumProvider
        {
            get { return _spectrumProvider; }
        }

        #endregion


        #region ctor

        AudioGraph graph;
        public bool isPlaying = false;
        public bool isActive = false;
        public AudioGraphProvider(AudioGraph g, bool a, bool p)
        {
            try
            {
                isActive = a;
                isPlaying = p;
                graph = g;
                var channelCount = graph != null ? (int)graph.EncodingProperties.ChannelCount : 1;
                var sampleRate = graph != null ? (int)graph.EncodingProperties.SampleRate : 1;
                _spectrumProvider = new BasicSpectrumProvider(channelCount, sampleRate, (FftSize)MainPage.FFTSizeArray[MainPage.FftSizeValue]);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion


        #region Methods
        public void ProcessFrameOutput(float l, float r)
        {
            if (SpectrumProvider != null && isPlaying && isActive)
            {
                try
                {
                    SpectrumProvider.Add(MainPage.LeftChannelState ? l : 0, MainPage.RightChannelState ? r: 0);
                }
                catch (Exception ex)
                {

                }
            }
        }
        #endregion


        #region Event handlers

        public bool GetFftData(float[] fftDataBuffer)
        {
            try
            {
                if (SpectrumProvider == null) return false;
                return SpectrumProvider.GetFftData(fftDataBuffer);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetFftFrequencyIndex(int frequency)
        {
            try
            {
                if (SpectrumProvider == null || graph == null) return 0;

                var fftSize = (int)SpectrumProvider.FftSize;
                var f = graph.EncodingProperties.SampleRate / 2.0;
                return (int)(frequency / f * (fftSize / 2.0));
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion
    }

}
