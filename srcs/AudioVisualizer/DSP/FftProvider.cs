using System;

namespace Visualizer.UI.DSP
{
    /// <summary>
    /// Provides FFT calculations.
    /// </summary>
    /// <remarks>
    /// Usage: Use the <see cref="Add(float[],int)"/>-method to input samples to the <see cref="FftProvider"/>. Use the <see cref="GetFftData(float[])"/> method to 
    /// calculate the Fast Fourier Transform.
    /// </remarks>
    public class FftProvider
    {
        private readonly int _channels;
        private readonly FftSize _fftSize;
        private readonly int _fftSizeExponent;
        private readonly Complex[] _storedSamples;
        private int _currentSampleOffset;
        private volatile bool _newDataAvailable;

        /// <summary>
        /// Gets the specified fft size.
        /// </summary>
        public FftSize FftSize => _fftSize;

        /// <summary>
        /// Gets a value which indicates whether new data is available.
        /// </summary>
        public virtual bool IsNewDataAvailable => _newDataAvailable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FftProvider"/> class.
        /// </summary>
        /// <param name="channels">Number of channels of the input data.</param>
        /// <param name="fftSize">The number of bands to use.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="channels"/> is less than zero.</exception>
        public FftProvider(int channels, FftSize fftSize)
        {
            try
            {
                if (channels < 1)
                    throw new ArgumentOutOfRangeException(nameof(channels));
                var exponent = Math.Round(Math.Log((int)fftSize, 2));
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (exponent % 1 != 0 || exponent == 0)
                    // ReSharper restore CompareOfFloatsByEqualityOperator
                    throw new ArgumentOutOfRangeException(nameof(fftSize));

                _channels = channels;
                _fftSize = fftSize; //todo: add validation for the fftSize parameter.
                _fftSizeExponent = (int)exponent;
                _storedSamples = new Complex[(int)fftSize];
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Adds a <paramref name="left"/> and a <paramref name="right"/> sample to the <see cref="FftProvider"/>. The <paramref name="left"/> and the <paramref name="right"/> sample will be merged together.
        /// </summary>
        /// <param name="left">The sample of the left channel.</param>
        /// <param name="right">The sample of the right channel.</param>
        public virtual void Add(float left, float right)
        {
            try
            {
                //todo: may throw an exception... not sure
                _storedSamples[_currentSampleOffset].Imaginary = 0f;
                _storedSamples[_currentSampleOffset].Real = (left + right) / 2f;
                _currentSampleOffset++;

                if (_currentSampleOffset >= _storedSamples.Length) //override the already stored samples.
                    _currentSampleOffset = 0;

                _newDataAvailable = true;
            }catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Adds multiple samples to the <see cref="FftProvider"/>. 
        /// </summary>
        /// <param name="samples">Float Array which contains samples.</param>
        /// <param name="count">Number of samples to add to the <see cref="FftProvider"/>.</param>
        public virtual void Add(float[] samples, int count)
        {
            try
            {
                if (samples == null)
                    throw new ArgumentNullException(nameof(samples));
                count -= count % _channels; //not sure whether to throw an exception...
                if (count > samples.Length)
                    throw new ArgumentOutOfRangeException(nameof(count));

                var blocksToProcess = count / _channels;
                for (var i = 0; i < blocksToProcess; i += _channels)
                {
                    _storedSamples[_currentSampleOffset].Imaginary = 0f;
                    _storedSamples[_currentSampleOffset].Real = MergeSamples(samples, i, _channels);
                    _currentSampleOffset++;

                    if (_currentSampleOffset >= _storedSamples.Length) //override the already stored samples.
                        _currentSampleOffset = 0;
                }

                _newDataAvailable = count > 0;
            }catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Calculates the Fast Fourier Transform and stores the result in the <paramref name="fftResultBuffer"/>.
        /// </summary>
        /// <param name="fftResultBuffer">The output buffer.</param>
        /// <returns>Returns a value which indicates whether the Fast Fourier Transform got calculated. If there have not been added any new samples since the last transform, the FFT won't be calculated. True means that the Fast Fourier Transform got calculated.</returns>
        public virtual bool GetFftData(Complex[] fftResultBuffer)
        {
            try
            {
                if (fftResultBuffer == null)
                    throw new ArgumentNullException(nameof(fftResultBuffer));

                var input = fftResultBuffer;
                Array.Copy(_storedSamples, input, _storedSamples.Length);

                FastFourierTransformation.Fft(input, _fftSizeExponent);
                var result = _newDataAvailable;
                _newDataAvailable = false;

                return result;
            }catch(Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Calculates the Fast Fourier Transform and stores the result in the <paramref name="fftResultBuffer"/>.
        /// </summary>
        /// <param name="fftResultBuffer">The output buffer.</param>
        /// <returns>Returns a value which indicates whether the Fast Fourier Transform got calculated. If there have not been added any new samples since the last transform, the FFT won't be calculated. True means that the Fast Fourier Transform got calculated.</returns>
        public virtual bool GetFftData(float[] fftResultBuffer)
        {
            try
            {
                if (fftResultBuffer == null)
                    throw new ArgumentNullException(nameof(fftResultBuffer));

                if (fftResultBuffer.Length < (int)_fftSize)
                    throw new ArgumentException("Length of array must be at least as long as the specified fft size.", nameof(fftResultBuffer));
                var input = new Complex[(int)_fftSize];

                var result = _newDataAvailable;
                GetFftData(input);

                for (var i = 0; i < input.Length / 2; i++)
                {
                    fftResultBuffer[i] = (float)Math.Sqrt(input[i].Real * input[i].Real + input[i].Imaginary * input[i].Imaginary);
                }

                //no need to set _newDataAvailable to false, since it got already set by the GetFftData(Complex[]) method.
                return result;
            }catch(Exception ex)
            {
                return false;
            }
        }

        private float MergeSamples(float[] samples, int i, int channels)
        {
            try
            {
                switch (channels)
                {
                    case 1:
                        return samples[i];
                    case 2:
                        return (samples[i] + samples[i + 1]) / 2f;
                    case 3:
                        return (samples[i] + samples[i + 1] + samples[i + 2]) / 3f;
                    case 4:
                        return (samples[i] + samples[i + 1] + samples[i + 2] + samples[i + 3]) / 4f;
                    case 5:
                        return (samples[i] + samples[i + 1] + samples[i + 2] + samples[i + 3] + samples[i + 4]) / 5f;
                    case 6:
                        return (samples[i] + samples[i + 1] + samples[i + 2] + samples[i + 3] + samples[i + 4] + samples[i + 5]) / 6f;
                }

                float sample = 0;
                for (var j = i; j < channels; j++)
                {
                    sample += samples[j++];
                }
                return sample / channels;
            }catch(Exception ex)
            {
                return 0f;
            }
        }
    }

}
