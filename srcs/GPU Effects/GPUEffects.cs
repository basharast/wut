/**

  Copyright (c) 2021-2024  Bashar Astifan

*/
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace WinUniversalTool.Models
{
    public class GPUEffects : BindableBase
    {
        //Effects System
        public Visibility compatibiltyTag = Visibility.Collapsed;
        public bool EffectsVisible = false;
        public List<RenderEffect> RenderEffectsList { get; set; } = new List<RenderEffect>();

        public async Task<List<byte[]>> getOverlay()
        {
            try
            {
                var filePicker = new FileOpenPicker();
                filePicker.SuggestedStartLocation = PickerLocationId.Downloads;
                filePicker.FileTypeFilter.Add(".png");
                filePicker.FileTypeFilter.Add(".jpg");
                filePicker.FileTypeFilter.Add(".jpeg");
                filePicker.FileTypeFilter.Add(".gif");
                var TargetFiles = await filePicker.PickMultipleFilesAsync();
                if (TargetFiles != null && TargetFiles.Count > 0)
                {
                    List<byte[]> byteArrays = new List<byte[]>();
                    foreach (var fItem in TargetFiles)
                    {
                        byte[] resultInBytes = (await FileIO.ReadBufferAsync(fItem)).ToArray();
                        byteArrays.Add(resultInBytes);
                    }
                    if (byteArrays.Count > 0)
                    {
                        return byteArrays;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public bool addOverlays = false;
        public bool addOverlaysInitial = false;
        public bool AddOverlays
        {
            get
            {
                return addOverlays;
            }
            set
            {
                addOverlays = value;
                if (!addOverlaysInitial)
                {
                    if (value)
                    {
                        GetOverlay();
                    }
                    else
                    {
                        isBlendModeSet = false;
                        UpdateEffect("OverlayEffect", false, null);
                    }
                    RaisePropertyChanged(nameof(AddOverlays));
                }
            }
        }
        bool overlayInProgress = false;

        private async void GetOverlay()
        {
            if (overlayInProgress)
            {
                return;
            }
            overlayInProgress = true;
            try
            {
                var overlay = await getOverlay();
                if (overlay != null)
                {
                    isBlendModeSet = true;
                    UpdateEffect("OverlayEffect", true, overlay);
                }
                else
                {
                    AddOverlays = false;
                }
            }
            catch (Exception ex)
            {
                AddOverlays = false;
            }
            overlayInProgress = false;
        }

        public async void ReloadEffects()
        {
            if (targetFile != null && targetImage != null)
            {
                try
                {
                    cancellationTokenSource.Cancel();
                }
                catch (Exception ex)
                {

                }
                cancellationTokenSource = new CancellationTokenSource();
                Interlocked.Increment(ref requestsAmount);
                await UpdateEffects(targetImage, targetFile).AsAsyncAction().AsTask(cancellationTokenSource.Token);
            }
        }

        //BrightnessEffect
        #region Brightness Effect
        public double brightnessLevel = 0.0;
        public double BrightnessLevel
        {
            get
            {
                return brightnessLevel;
            }
            set
            {
                brightnessLevel = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("BrightnessLevel", brightnessLevel);
                RaisePropertyChanged("BrightnessLevel");
                UpdateEffect("BrightnessEffect", BrightnessEffect, brightnessLevel);
            }
        }
        public bool brightnessEffect = false;
        public bool BrightnessEffect
        {
            set
            {
                brightnessEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("BrightnessEffect", brightnessEffect);
                RaisePropertyChanged("BrightnessEffect");
                UpdateEffect("BrightnessEffect", BrightnessEffect, brightnessLevel);
            }
            get
            {
                return brightnessEffect;
            }
        }
        #endregion

        //Transform3DEffect
        #region Transform 3D Effect
        public double rotate = 0;
        public double Rotate
        {
            get
            {
                return rotate;
            }
            set
            {
                rotate = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Rotate", rotate);
                RaisePropertyChanged("Rotate");
                UpdateEffect("Transform3DEffect", Transform3DEffect, rotate, RotateX, RotateY);
            }
        }
        public double rotateX = 0;
        public double RotateX
        {
            get
            {
                return rotateX;
            }
            set
            {
                rotateX = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("RotateX", rotateX);
                RaisePropertyChanged("RotateX");
                UpdateEffect("Transform3DEffect", Transform3DEffect, Rotate, rotateX, RotateY);
            }
        }
        public double rotateY = 0;
        public double RotateY
        {
            get
            {
                return rotateY;
            }
            set
            {
                rotateY = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("RotateY", rotateY);
                RaisePropertyChanged("RotateY");
                UpdateEffect("Transform3DEffect", Transform3DEffect, Rotate, RotateX, rotateY);
            }
        }
        public bool transform3DEffect = false;
        public bool Transform3DEffect
        {
            set
            {
                transform3DEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Transform3DEffect", transform3DEffect);
                RaisePropertyChanged("Transform3DEffect");
                UpdateEffect("Transform3DEffect", Transform3DEffect, Rotate, RotateX, RotateY);
            }
            get
            {
                return transform3DEffect;
            }
        }
        #endregion

        //ContrastEffect
        #region Contrast Effect
        public double contrastLevel = 0.0;
        public double ContrastLevel
        {
            get
            {
                return contrastLevel;
            }
            set
            {
                contrastLevel = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("ContrastLevel", contrastLevel);
                RaisePropertyChanged("ContrastLevel");
                UpdateEffect("ContrastEffect", ContrastEffect, contrastLevel);
            }
        }
        public bool contrastEffect = false;
        public bool ContrastEffect
        {
            set
            {
                contrastEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("ContrastEffect", contrastEffect);
                RaisePropertyChanged("ContrastEffect");
                UpdateEffect("ContrastEffect", contrastEffect, ContrastLevel);
            }
            get
            {
                return contrastEffect;
            }
        }
        #endregion

        //ExposureEffect
        #region Exposure Effect
        public double exposure = 0.0;
        public double Exposure
        {
            get
            {
                return exposure;
            }
            set
            {
                exposure = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Exposure", exposure);
                RaisePropertyChanged("Exposure");
                UpdateEffect("ExposureEffect", ExposureEffect, exposure);
            }
        }
        public bool exposureEffect = false;
        public bool ExposureEffect
        {
            set
            {
                exposureEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("ExposureEffect", exposureEffect);
                RaisePropertyChanged("ExposureEffect");
                UpdateEffect("ExposureEffect", ExposureEffect, Exposure);
            }
            get
            {
                return exposureEffect;
            }
        }
        #endregion

        //SepiaEffect
        #region SepiaEffect
        public double intensity = 0.5;
        public double Intensity
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Intensity", intensity);
                RaisePropertyChanged("Intensity");
                UpdateEffect("SepiaEffect", SepiaEffect, intensity);
            }
        }
        public bool sepiaEffect = false;
        public bool SepiaEffect
        {
            set
            {
                sepiaEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("SepiaEffect", sepiaEffect);
                RaisePropertyChanged("SepiaEffect");
                UpdateEffect("SepiaEffect", SepiaEffect, Intensity);
            }
            get
            {
                return sepiaEffect;
            }
        }
        #endregion

        //SharpenEffect
        #region SharpenEffect
        public double amountSharpen = 0.0;
        public double AmountSharpen
        {
            get
            {
                return amountSharpen;
            }
            set
            {
                amountSharpen = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("AmountSharpen", amountSharpen);
                RaisePropertyChanged("AmountSharpen");
                UpdateEffect("SharpenEffect", SharpenEffect, amountSharpen);
            }
        }
        public bool sharpenEffect = false;
        public bool SharpenEffect
        {
            set
            {
                sharpenEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("SharpenEffect", sharpenEffect);
                RaisePropertyChanged("SharpenEffect");
                UpdateEffect("SharpenEffect", SharpenEffect, AmountSharpen);
            }
            get
            {
                return sharpenEffect;
            }
        }
        #endregion

        //StraightenEffect
        #region Straighten Effect
        public double angleStraighten = 0.0;
        public double AngleStraighten
        {
            get
            {
                return angleStraighten;
            }
            set
            {
                angleStraighten = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("AngleStraighten", angleStraighten);
                RaisePropertyChanged("AngleStraighten");
                UpdateEffect("StraightenEffect", StraightenEffect, angleStraighten);
            }
        }
        public bool straightenEffect = false;
        public bool StraightenEffect
        {
            set
            {
                straightenEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("StraightenEffect", straightenEffect);
                RaisePropertyChanged("StraightenEffect");
                UpdateEffect("StraightenEffect", StraightenEffect, AngleStraighten);
            }
            get
            {
                return straightenEffect;
            }
        }
        #endregion

        //VignetteEffect
        #region Vignette Effect
        public double amountVignette = 0.50;
        public double AmountVignette
        {
            get
            {
                return amountVignette;
            }
            set
            {
                amountVignette = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("AmountVignette", amountVignette);
                RaisePropertyChanged("AmountVignette");
                UpdateEffect("VignetteEffect", VignetteEffect, amountVignette, Curve);
            }
        }
        public double curve = 0.0;
        public double Curve
        {
            get
            {
                return curve;
            }
            set
            {
                curve = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Curve", curve);
                RaisePropertyChanged("Curve");
                UpdateEffect("VignetteEffect", VignetteEffect, AmountVignette, curve);
            }
        }
        public bool vignetteEffect = false;
        public bool VignetteEffect
        {
            set
            {
                vignetteEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("VignetteEffect", vignetteEffect);
                RaisePropertyChanged("VignetteEffect");
                UpdateEffect("VignetteEffect", VignetteEffect, AmountVignette, Curve);
            }
            get
            {
                return vignetteEffect;
            }
        }
        #endregion

        //TileEffect
        #region Tile Effect
        public double left = 0.0;
        public double Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Left", left);
                RaisePropertyChanged("Left");
                UpdateEffect("TileEffect", TileEffect, left, Top, Right, Bottom);
            }
        }
        public double top = 0.0;
        public double Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Top", top);
                RaisePropertyChanged("Top");
                UpdateEffect("TileEffect", TileEffect, Left, top, Right, Bottom);
            }
        }
        public double right = 256;
        public double Right
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Right", right);
                RaisePropertyChanged("Right");
                UpdateEffect("TileEffect", TileEffect, Left, Top, right, Bottom);
            }
        }
        public double bottom = 256;
        public double Bottom
        {
            get
            {
                return bottom;
            }
            set
            {
                bottom = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Bottom", bottom);
                RaisePropertyChanged("Bottom");
                UpdateEffect("TileEffect", TileEffect, Left, Top, Right, bottom);
            }
        }
        public bool tileEffect = false;
        public bool TileEffect
        {
            set
            {
                tileEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("TileEffect", tileEffect);
                RaisePropertyChanged("TileEffect");
                UpdateEffect("TileEffect", TileEffect, Left, Top, Right, Bottom);
            }
            get
            {
                return tileEffect;
            }
        }
        #endregion

        //CropEffect
        #region Crop Effect
        public double leftCrop = 0.0;
        public double LeftCrop
        {
            get
            {
                return leftCrop;
            }
            set
            {
                leftCrop = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("LeftCrop", leftCrop);
                RaisePropertyChanged("LeftCrop");
                UpdateEffect("CropEffect", CropEffect, leftCrop, TopCrop, RightCrop, BottomCrop);
            }
        }
        public double topCrop = 0.0;
        public double TopCrop
        {
            get
            {
                return topCrop;
            }
            set
            {
                topCrop = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("TopCrop", topCrop);
                RaisePropertyChanged("TopCrop");
                UpdateEffect("CropEffect", CropEffect, LeftCrop, topCrop, RightCrop, BottomCrop);
            }
        }
        public double rightCrop = 256;
        public double RightCropMax = 256;
        public double RightCrop
        {
            get
            {
                return rightCrop;
            }
            set
            {
                rightCrop = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("RightCrop", rightCrop);
                RaisePropertyChanged("RightCrop");
                UpdateEffect("CropEffect", CropEffect, LeftCrop, TopCrop, rightCrop, BottomCrop);
            }
        }
        public double bottomCrop = 256;
        public double BottomCropMax = 256;
        public double BottomCrop
        {
            get
            {
                return bottomCrop;
            }
            set
            {
                bottomCrop = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("BottomCrop", bottomCrop);
                RaisePropertyChanged("BottomCrop");
                UpdateEffect("CropEffect", CropEffect, LeftCrop, TopCrop, RightCrop, bottomCrop);
            }
        }
        public bool cropEffect = false;
        public bool CropEffect
        {
            set
            {
                cropEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("CropEffect", cropEffect);
                RaisePropertyChanged("CropEffect");
                UpdateEffect("CropEffect", CropEffect, LeftCrop, TopCrop, RightCrop, BottomCrop);
            }
            get
            {
                return cropEffect;
            }
        }
        #endregion

        //TemperatureAndTintEffect
        #region Temperature And Tint Effect
        public double temperature = 0.0;
        public double Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                temperature = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Temperature", temperature);
                RaisePropertyChanged("Temperature");
                UpdateEffect("TemperatureAndTintEffect", TemperatureAndTintEffect, temperature, Tint);
            }
        }
        public double tint = 0.0;
        public double Tint
        {
            get
            {
                return tint;
            }
            set
            {
                tint = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Tint", tint);
                RaisePropertyChanged("Tint");
                UpdateEffect("TemperatureAndTintEffect", TemperatureAndTintEffect, Temperature, tint);
            }
        }
        public bool temperatureAndTintEffect = false;
        public bool TemperatureAndTintEffect
        {
            set
            {
                temperatureAndTintEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("TemperatureAndTintEffect", temperatureAndTintEffect);
                RaisePropertyChanged("TemperatureAndTintEffect");
                UpdateEffect("TemperatureAndTintEffect", TemperatureAndTintEffect, Temperature, Tint);
            }
            get
            {
                return temperatureAndTintEffect;
            }
        }
        #endregion

        //MorphologyEffect
        #region Morphology Effect
        public double heightMorphology = 1.0;
        public double HeightMorphology
        {
            get
            {
                return heightMorphology;
            }
            set
            {
                heightMorphology = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("HeightMorphology", heightMorphology);
                RaisePropertyChanged("HeightMorphology");
                UpdateEffect("MorphologyEffect", MorphologyEffect, heightMorphology);
            }
        }
        public bool morphologyEffect = false;
        public bool MorphologyEffect
        {
            set
            {
                morphologyEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("MorphologyEffect", morphologyEffect);
                RaisePropertyChanged("MorphologyEffect");
                UpdateEffect("MorphologyEffect", MorphologyEffect, HeightMorphology);
            }
            get
            {
                return morphologyEffect;
            }
        }
        #endregion

        //SaturationEffect
        #region Saturation Effect
        public double saturation = 1.0;
        public double Saturation
        {
            get
            {
                return saturation;
            }
            set
            {
                saturation = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Saturation", saturation);
                RaisePropertyChanged("Saturation");
                UpdateEffect("SaturationEffect", SaturationEffect, saturation);
            }
        }
        public bool saturationEffect = false;
        public bool SaturationEffect
        {
            set
            {
                saturationEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("SaturationEffect", saturationEffect);
                RaisePropertyChanged("SaturationEffect");
                UpdateEffect("SaturationEffect", SaturationEffect, Saturation);
            }
            get
            {
                return saturationEffect;
            }
        }
        #endregion

        //ScaleEffect
        #region Scale Effect
        public double widthScale = 1.0;
        public double WidthScale
        {
            get
            {
                return widthScale;
            }
            set
            {
                widthScale = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("WidthScale", widthScale);
                RaisePropertyChanged("WidthScale");
                UpdateEffect("ScaleEffect", ScaleEffect, widthScale, HeightScale, SharpnessScale);
            }
        }
        public double heightScale = 1.0;
        public double HeightScale
        {
            get
            {
                return heightScale;
            }
            set
            {
                heightScale = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("HeightScale", heightScale);
                RaisePropertyChanged("HeightScale");
                UpdateEffect("ScaleEffect", ScaleEffect, WidthScale, heightScale, SharpnessScale);
            }
        }
        public double sharpnessScale = 0.0;
        public double SharpnessScale
        {
            get
            {
                return sharpnessScale;
            }
            set
            {
                sharpnessScale = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("SharpnessScale", sharpnessScale);
                RaisePropertyChanged("SharpnessScale");
                UpdateEffect("ScaleEffect", ScaleEffect, WidthScale, HeightScale, sharpnessScale);
            }
        }
        public bool scaleEffect = false;
        public bool ScaleEffect
        {
            set
            {
                scaleEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("ScaleEffect", scaleEffect);
                RaisePropertyChanged("ScaleEffect");
                UpdateEffect("ScaleEffect", ScaleEffect, WidthScale, HeightScale, SharpnessScale);
            }
            get
            {
                return scaleEffect;
            }
        }
        #endregion

        //PosterizeEffect
        #region Posterize Effect
        public double red = 4.0;
        public double Red
        {
            get
            {
                return red;
            }
            set
            {
                red = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Red", red);
                RaisePropertyChanged("Red");
                UpdateEffect("PosterizeEffect", PosterizeEffect, red, Green, Blue);
            }
        }
        public double green = 4.0;
        public double Green
        {
            get
            {
                return green;
            }
            set
            {
                green = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Green", green);
                RaisePropertyChanged("Green");
                UpdateEffect("PosterizeEffect", PosterizeEffect, Red, green, Blue);
            }
        }
        public double blue = 4.0;
        public double Blue
        {
            get
            {
                return blue;
            }
            set
            {
                blue = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Blue", blue);
                RaisePropertyChanged("Blue");
                UpdateEffect("PosterizeEffect", PosterizeEffect, Red, Green, blue);
            }
        }
        public bool posterizeEffect = false;
        public bool PosterizeEffect
        {
            set
            {
                posterizeEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("PosterizeEffect", posterizeEffect);
                RaisePropertyChanged("PosterizeEffect");
                UpdateEffect("PosterizeEffect", PosterizeEffect, Red, Green, Blue);
            }
            get
            {
                return posterizeEffect;
            }
        }
        #endregion

        //HighlightsAndShadowsEffect
        #region Highlights AndShadows Effect
        public double clarity = 0.0;
        public double Clarity
        {
            get
            {
                return clarity;
            }
            set
            {
                clarity = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Clarity", clarity);
                RaisePropertyChanged("Clarity");
                UpdateEffect("HighlightsAndShadowsEffect", HighlightsAndShadowsEffect, clarity, Highlights, Shadows);
            }
        }
        public double highlights = 0.0;
        public double Highlights
        {
            get
            {
                return highlights;
            }
            set
            {
                highlights = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Highlights", highlights);
                RaisePropertyChanged("Highlights");
                UpdateEffect("HighlightsAndShadowsEffect", HighlightsAndShadowsEffect, Clarity, highlights, Shadows);
            }
        }
        public double shadows = 0.0;
        public double Shadows
        {
            get
            {
                return shadows;
            }
            set
            {
                shadows = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Shadows", shadows);
                RaisePropertyChanged("Shadows");
                UpdateEffect("HighlightsAndShadowsEffect", HighlightsAndShadowsEffect, Clarity, Highlights, shadows);
            }
        }
        public bool highlightsAndShadowsEffect = false;
        public bool HighlightsAndShadowsEffect
        {
            set
            {
                highlightsAndShadowsEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("HighlightsAndShadowsEffect", highlightsAndShadowsEffect);
                RaisePropertyChanged("HighlightsAndShadowsEffect");
                UpdateEffect("HighlightsAndShadowsEffect", HighlightsAndShadowsEffect, Clarity, Highlights, Shadows);
            }
            get
            {
                return highlightsAndShadowsEffect;
            }
        }
        #endregion

        //GaussianBlurEffect
        #region Gaussian Blur Effect
        public double blurAmountGaussianBlur = 1.0;
        public double BlurAmountGaussianBlur
        {
            get
            {
                return blurAmountGaussianBlur;
            }
            set
            {
                blurAmountGaussianBlur = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("BlurAmountGaussianBlur", blurAmountGaussianBlur);
                RaisePropertyChanged("BlurAmountGaussianBlur");
                UpdateEffect("GaussianBlurEffect", GaussianBlurEffect, blurAmountGaussianBlur);
            }
        }
        public bool gaussianBlurEffect = false;
        public bool GaussianBlurEffect
        {
            set
            {
                gaussianBlurEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("GaussianBlurEffect", gaussianBlurEffect);
                RaisePropertyChanged("GaussianBlurEffect");
                UpdateEffect("GaussianBlurEffect", GaussianBlurEffect, BlurAmountGaussianBlur);
            }
            get
            {
                return gaussianBlurEffect;
            }
        }
        #endregion

        //GrayscaleEffect
        #region Grayscale Effect
        public bool grayscaleEffect = false;
        public bool GrayscaleEffect
        {
            set
            {
                grayscaleEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("GrayscaleEffect", grayscaleEffect);
                RaisePropertyChanged("GaussianBlurEffect");
                UpdateEffect("GrayscaleEffect", GrayscaleEffect);
            }
            get
            {
                return grayscaleEffect;
            }
        }
        #endregion

        //RgbToHueEffect
        #region Rgb To Hue Effect
        public bool rgbToHueEffect = false;
        public bool RgbToHueEffect
        {
            set
            {
                rgbToHueEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("RgbToHueEffect", rgbToHueEffect);
                RaisePropertyChanged("RgbToHueEffect");
                UpdateEffect("RgbToHueEffect", RgbToHueEffect);
            }
            get
            {
                return rgbToHueEffect;
            }
        }
        #endregion

        //InvertEffect
        #region Invert Effect
        public bool invertEffect = false;
        public bool InvertEffect
        {
            set
            {
                invertEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("InvertEffect", invertEffect);
                RaisePropertyChanged("InvertEffect");
                UpdateEffect("InvertEffect", InvertEffect);
            }
            get
            {
                return invertEffect;
            }
        }
        #endregion

        //HueToRgbEffect
        #region Hue To Rgb Effect
        public bool hueToRgbEffect = false;
        public bool HueToRgbEffect
        {
            set
            {
                hueToRgbEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("HueToRgbEffect", hueToRgbEffect);
                RaisePropertyChanged("HueToRgbEffect");
                UpdateEffect("HueToRgbEffect", HueToRgbEffect);
            }
            get
            {
                return hueToRgbEffect;
            }
        }
        #endregion

        //DirectionalBlurEffect
        #region Directional Blur Effect
        public double blurAmount = 3;
        public double BlurAmount
        {
            get
            {
                return blurAmount;
            }
            set
            {
                blurAmount = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("BlurAmount", BlurAmount);
                RaisePropertyChanged("BlurAmount");
                UpdateEffect("DirectionalBlurEffect", DirectionalBlurEffect, blurAmount, Angle);
            }
        }
        public double angle = 0;
        public double Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("Angle", angle);
                RaisePropertyChanged("Angle");
                UpdateEffect("DirectionalBlurEffect", DirectionalBlurEffect, BlurAmount, angle);
            }
        }
        public bool directionalBlurEffect = false;
        public bool DirectionalBlurEffect
        {
            set
            {
                directionalBlurEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("DirectionalBlurEffect", directionalBlurEffect);
                RaisePropertyChanged("DirectionalBlurEffect");
                UpdateEffect("DirectionalBlurEffect", DirectionalBlurEffect, BlurAmount, Angle);
            }
            get
            {
                return directionalBlurEffect;
            }
        }
        #endregion

        //EmbossEffect
        #region Emboss Effect
        public double amountEmboss = 1;
        public double AmountEmboss
        {
            get
            {
                return amountEmboss;
            }
            set
            {
                amountEmboss = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("AmountEmboss", AmountEmboss);
                RaisePropertyChanged("EmbossEffect");
                UpdateEffect("EmbossEffect", EmbossEffect, amountEmboss, AngleEmboss);
            }
        }
        public double angleEmboss = 0;
        public double AngleEmboss
        {
            get
            {
                return angleEmboss;
            }
            set
            {
                angleEmboss = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("AngleEmboss", angleEmboss);
                RaisePropertyChanged("AngleEmboss");
                UpdateEffect("EmbossEffect", EmbossEffect, AmountEmboss, angleEmboss);
            }
        }
        public bool embossEffect = false;
        public bool EmbossEffect
        {
            set
            {
                embossEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("EmbossEffect", embossEffect);
                RaisePropertyChanged("EmbossEffect");
                UpdateEffect("EmbossEffect", EmbossEffect, AmountEmboss, AngleEmboss);
            }
            get
            {
                return embossEffect;
            }
        }
        #endregion

        //EdgeDetectionEffect
        #region Edge Detection Effect 
        public double blurAmountEdge = 0;
        public double BlurAmountEdge
        {
            get
            {
                return blurAmountEdge;
            }
            set
            {
                blurAmountEdge = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("BlurAmountEdge", BlurAmountEdge);
                RaisePropertyChanged("BlurAmountEdge");
                UpdateEffect("EdgeDetectionEffect", EdgeDetectionEffect, AmountEdge, blurAmountEdge);
            }
        }
        public double amountEdge = 0.5;
        public double AmountEdge
        {
            get
            {
                return amountEdge;
            }
            set
            {
                amountEdge = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("AmountEdge", amountEdge);
                RaisePropertyChanged("AmountEdge");
                UpdateEffect("EdgeDetectionEffect", EdgeDetectionEffect, AmountEdge, blurAmountEdge);
            }
        }
        public bool edgeDetectionEffect = false;
        public bool EdgeDetectionEffect
        {
            set
            {
                edgeDetectionEffect = value;
                Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("EdgeDetectionEffect", edgeDetectionEffect);
                RaisePropertyChanged("EdgeDetectionEffect");
                UpdateEffect("EdgeDetectionEffect", edgeDetectionEffect, AmountEdge, blurAmountEdge);
            }
            get
            {
                return edgeDetectionEffect;
            }
        }
        #endregion

        public void SyncEffectsSettings()
        {
            try
            {

                var effectsOrderString = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("effectsOrderHistory", "");
                if (effectsOrderString.Length > 0)
                {
                    effectsOrderHistory = JsonConvert.DeserializeObject<Dictionary<string, int>>(effectsOrderString);
                }
            }
            catch (Exception ex)
            {

            }
            isEffectsInitial = true;
            try
            {
                //BrightnessEffect
                BrightnessEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("BrightnessEffect", false);
                BrightnessLevel = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("BrightnessLevel", brightnessLevel);

                //ContrastEffect
                ContrastEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("ContrastEffect", false);
                ContrastLevel = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("ContrastLevel", contrastLevel);

                //DirectionalBlurEffect
                DirectionalBlurEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("DirectionalBlurEffect", false);
                BlurAmount = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("BlurAmount", blurAmount);
                Angle = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Angle", angle);

                //EdgeDetectionEffect
                EdgeDetectionEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("EdgeDetectionEffect", false);
                AmountEdge = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("AmountEdge", amountEdge);
                EdgeDetectionEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("EdgeDetectionEffect", edgeDetectionEffect);

                //EmbossEffect
                EmbossEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("EmbossEffect", false);
                AmountEmboss = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("AmountEmboss", amountEmboss);
                AngleEmboss = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("AngleEmboss", angleEmboss);

                //ExposureEffect
                ExposureEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("ExposureEffect", false);
                Exposure = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Exposure", exposure);

                //GaussianBlurEffect
                GaussianBlurEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("GaussianBlurEffect", false);
                BlurAmountGaussianBlur = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("BlurAmountGaussianBlur", blurAmountGaussianBlur);

                //SaturationEffect
                SaturationEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("SaturationEffect", false);
                Saturation = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Saturation", saturation);

                //SepiaEffect
                SepiaEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("SepiaEffect", false);
                Intensity = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Intensity", intensity);

                //Transform3DEffect
                Transform3DEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Transform3DEffect", false);
                Rotate = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Rotate", rotate);
                RotateX = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("RotateX", rotateX);
                RotateY = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("RotateY", rotateY);

                //SharpenEffect
                SharpenEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("SharpenEffect", false);
                AmountSharpen = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("AmountSharpen", amountSharpen);

                //StraightenEffect
                StraightenEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("StraightenEffect", false);
                AngleStraighten = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("AngleStraighten", angleStraighten);

                //VignetteEffect
                VignetteEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("VignetteEffect", false);
                AmountVignette = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("AmountVignette", amountVignette);
                Curve = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Curve", curve);

                //GrayscaleEffect
                GrayscaleEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("GrayscaleEffect", false);

                //HueToRgbEffect 
                HueToRgbEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("HueToRgbEffect", false);

                //InvertEffect 
                InvertEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("InvertEffect", false);

                //RgbToHueEffect 
                RgbToHueEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("RgbToHueEffect", false);

                //HighlightsAndShadowsEffect 
                HighlightsAndShadowsEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("HighlightsAndShadowsEffect", false);
                Clarity = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Clarity", clarity);
                Highlights = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Highlights", highlights);
                Shadows = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Shadows", shadows);

                //PosterizeEffect 
                PosterizeEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("PosterizeEffect", false);
                Red = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Red", red);
                Green = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Green", green);
                Blue = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Blue", blue);

                //ScaleEffect 
                ScaleEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("ScaleEffect", false);
                WidthScale = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("WidthScale", widthScale);
                HeightScale = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("HeightScale", heightScale);
                SharpnessScale = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("SharpnessScale", sharpnessScale);

                //TemperatureAndTintEffect 
                TemperatureAndTintEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("TemperatureAndTintEffect", false);
                Temperature = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Temperature", temperature);
                Tint = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Tint", tint);

                //TileEffect 
                TileEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("TileEffect", false);
                Left = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Left", left);
                Top = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Top", top);
                Right = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Right", right);
                Bottom = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("Bottom", bottom);

                //CropEffect 
                CropEffect = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("CropEffect", false);
                LeftCrop = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("LeftCrop", leftCrop);
                TopCrop = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("TopCrop", topCrop);
                RightCrop = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("RightCrop", rightCrop);
                BottomCrop = Plugin.Settings.CrossSettings.Current.GetValueOrDefault("BottomCrop", bottomCrop);

            }
            catch (Exception ex)
            {

            }
            isEffectsInitial = false;
        }
        public void ClearAllEffectsCall()
        {
            BrightnessEffect = false;
            ContrastEffect = false;
            DirectionalBlurEffect = false;
            EmbossEffect = false;
            ExposureEffect = false;
            GaussianBlurEffect = false;
            GrayscaleEffect = false;
            HueToRgbEffect = false;
            InvertEffect = false;
            HighlightsAndShadowsEffect = false;
            PosterizeEffect = false;
            RgbToHueEffect = false;
            SaturationEffect = false;
            ScaleEffect = false;
            SepiaEffect = false;
            SharpenEffect = false;
            StraightenEffect = false;
            TemperatureAndTintEffect = false;
            TileEffect = false;
            CropEffect = false;
            VignetteEffect = false;
            Transform3DEffect = false;

            //Helpers.ShowToastNotification("Clear Effects","All effects cleared");
        }

        bool isEffectsInitial = false;
        Dictionary<string, int> effectsOrderHistory = new Dictionary<string, int>();
        bool effectsInProgress = true;
        private async void UpdateEffect(string EffectName, bool EffectState, double EffectValue1 = 0, double EffectValue2 = 0, double EffectValue3 = 0, double EffectValue4 = 0)
        {
            effectsInProgress = true;
            var ForceOrder = -1;
            if (!EffectState)
            {
                var testOrder = -1;
                if (effectsOrderHistory.TryGetValue(EffectName, out testOrder))
                {
                    effectsOrderHistory.Remove(EffectName);
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("effectsOrderHistory", JsonConvert.SerializeObject(effectsOrderHistory));
                }
            }
            else if (isEffectsInitial)
            {
                var testOrder = -1;
                if (effectsOrderHistory.TryGetValue(EffectName, out testOrder))
                {
                    ForceOrder = testOrder;
                }
            }
            var effectOrder = SetEffect(EffectName, EffectState, EffectValue1, EffectValue2, EffectValue3, EffectValue4, ForceOrder);
            if (effectOrder != -1)
            {
                var testOrder = -1;
                if (!effectsOrderHistory.TryGetValue(EffectName, out testOrder))
                {
                    effectsOrderHistory.Add(EffectName, effectOrder);
                    Plugin.Settings.CrossSettings.Current.AddOrUpdateValue("effectsOrderHistory", JsonConvert.SerializeObject(effectsOrderHistory));
                }
            }

            if (targetImage != null && targetFile != null)
            {
                try
                {
                    cancellationTokenSource.Cancel();
                }
                catch (Exception ex)
                {

                }
                cancellationTokenSource = new CancellationTokenSource();
                Interlocked.Increment(ref requestsAmount);
                await UpdateEffects(targetImage, targetFile).AsAsyncAction().AsTask(cancellationTokenSource.Token);
            }
            effectsInProgress = false;
        }

        private async void UpdateEffect(string EffectName, bool EffectState, List<byte[]> EffectValue1)
        {
            effectsInProgress = true;
            SetEffect(EffectName, EffectState, EffectValue1);
            if (targetImage != null && targetFile != null)
            {
                try
                {
                    cancellationTokenSource.Cancel();
                }
                catch (Exception ex)
                {

                }
                cancellationTokenSource = new CancellationTokenSource();
                Interlocked.Increment(ref requestsAmount);
                await UpdateEffects(targetImage, targetFile).AsAsyncAction().AsTask(cancellationTokenSource.Token);
            }
            effectsInProgress = false;
        }


        public int SetEffect(string EffectName, bool EffectState, double EffectValue1 = 0, double EffectValue2 = 0, double EffectValue3 = 0, double EffectValue4 = 0, int ForceOrder = -1)
        {
            int effectOrder = -1;
            if (!EffectState)
            {
                RemoveEffect(EffectName);
            }
            else
            {
                RenderEffect RenderEffect = new RenderEffect(EffectName, EffectValue1, EffectValue2, EffectValue3, EffectValue4);
                effectOrder = UpdateOrAddEffect(RenderEffect, ForceOrder);
            }
            return effectOrder;
        }

        public int SetEffect(string EffectName, bool EffectState, List<byte[]> EffectValue1, int ForceOrder = -1)
        {
            int effectOrder = -1;
            if (!EffectState)
            {
                RemoveEffect(EffectName);
            }
            else
            {
                RenderEffect RenderEffect = new RenderEffect(EffectName, EffectValue1);
                effectOrder = UpdateOrAddEffect(RenderEffect, ForceOrder);
            }
            return effectOrder;
        }
        public void RemoveEffect(string Name)
        {
            try
            {
                for (int i = 0; i < RenderEffectsList.Count; i++)
                {
                    if (RenderEffectsList[i].Name.Equals(Name))
                    {
                        RenderEffectsList.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public int UpdateOrAddEffect(RenderEffect effect, int ForceOrder = -1)
        {
            int effectOrder = -1;
            try
            {
                bool EffectFound = false;

                for (int i = 0; i < RenderEffectsList.Count; i++)
                {
                    if (RenderEffectsList[i].Name.Equals(effect.Name))
                    {
                        RenderEffectsList[i].Value1 = effect.Value1;
                        RenderEffectsList[i].Value2 = effect.Value2;
                        RenderEffectsList[i].Value3 = effect.Value3;
                        RenderEffectsList[i].Value4 = effect.Value4;
                        RenderEffectsList[i].Values1 = effect.Values1;
                        EffectFound = true;
                        break;
                    }
                }
                if (!EffectFound)
                {
                    if (ForceOrder > -1)
                    {
                        effect.Order = ForceOrder;
                    }
                    else
                    {
                        effect.Order = RenderEffectsList.Count;
                    }
                    effectOrder = effect.Order;
                    RenderEffectsList.Add(effect);
                }
            }
            catch (Exception ex)
            {

            }
            return effectOrder;
        }

        //Call on render
        //RenderTargetViewport will be updated based on source size
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public Image targetImage = null;
        public StorageFile targetFile = null;
        public bool isBlendModeSet = false;
        public bool isEffectsInProgress = false;
        public bool FitOverlay = false;
        public BlendEffectMode BlendEffectModeGlobal = BlendEffectMode.Overlay;
        public Visibility isProcessing = Visibility.Collapsed;
        int requestsAmount = 0;
        public async Task syncCropSliders()
        {
            try
            {
                if (targetFile != null)
                {
                    var device = new CanvasDevice();
                    using (var stream = await targetFile.OpenReadAsync())
                    {
                        var RenderTarget = await CanvasBitmap.LoadAsync(device, stream);
                        RightCrop = RenderTarget.SizeInPixels.Width;
                        BottomCrop = RenderTarget.SizeInPixels.Height;
                        RightCropMax = RenderTarget.SizeInPixels.Width;
                        BottomCropMax = RenderTarget.SizeInPixels.Height;
                        RaisePropertyChanged(nameof(RightCropMax));
                        RaisePropertyChanged(nameof(BottomCropMax));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public async Task<StorageFile> UpdateEffects(Image ContentImage, StorageFile imageFile, bool saveToTemp = false)
        {
            StorageFile returnedFile = null;
            try
            {
                if (isEffectsInProgress)
                {
                    return returnedFile;
                }
                isEffectsInProgress = true;
                isProcessing = Visibility.Visible;
                RaisePropertyChanged(nameof(isProcessing));
                TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                _ = Task.Run(async () =>
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.TryRunAsync(CoreDispatcherPriority.High, async () =>
                    {
                        try
                        {
                            while (requestsAmount > 0)
                            {
                                Interlocked.Decrement(ref requestsAmount);
                            }

                            using (var stream = await imageFile.OpenReadAsync())
                            {
                                CanvasImageInterpolation interpolation = CanvasImageInterpolation.Linear;

                                var device = new CanvasDevice();
                                var RenderTarget = await CanvasBitmap.LoadAsync(device, stream);

                                var renderer = new CanvasRenderTarget(device,
                                                                      RenderTarget.SizeInPixels.Width,
                                                                      RenderTarget.SizeInPixels.Height, RenderTarget.Dpi);

                                ICanvasImage outputResult = RenderTarget;

                                double viewportWidth = (double)RenderTarget.SizeInPixels.Width;
                                double viewportHeight = (double)RenderTarget.Size.Height;

                                List<PixelShaderEffect> outputShaders = new List<PixelShaderEffect>();
                                List<CanvasBitmap> outputOverlays = new List<CanvasBitmap>();

                                using (var drawingSession = renderer.CreateDrawingSession())
                                {
                                    foreach (var effect in RenderEffectsList.OrderBy(item => item.Order))
                                    {
                                        try
                                        {
                                            switch (effect.Name)
                                            {
                                                case "PixelShaderEffect":
                                                    foreach (var eValue in effect.Values1)
                                                    {
                                                        PixelShaderEffect pixelShaderEffect = new PixelShaderEffect(eValue);
                                                        pixelShaderEffect.CacheOutput = true;
                                                        pixelShaderEffect.Source1Interpolation = interpolation;
                                                        pixelShaderEffect.Source1Mapping = SamplerCoordinateMapping.OneToOne;
                                                        outputShaders.Add(pixelShaderEffect);
                                                    }
                                                    break;

                                                case "OverlayEffect":
                                                    foreach (var eValue in effect.Values1)
                                                    {
                                                        using (var ms = new System.IO.MemoryStream(eValue))
                                                        {
                                                            var bitmap = CanvasBitmap.LoadAsync(drawingSession.Device, ms.AsRandomAccessStream()).AsTask().Result;
                                                            effect.tempResult = bitmap;
                                                            outputOverlays.Add(bitmap);
                                                        }
                                                    }
                                                    break;

                                                case "BrightnessEffect":
                                                    BrightnessEffect brightnessEffect = new BrightnessEffect();
                                                    brightnessEffect.Source = outputResult;
                                                    brightnessEffect.CacheOutput = true;
                                                    brightnessEffect.BlackPoint = new Vector2(0f, (float)effect.Value1);
                                                    outputResult = brightnessEffect;
                                                    break;

                                                case "ContrastEffect":
                                                    ContrastEffect contrastEffect = new ContrastEffect();
                                                    contrastEffect.Source = outputResult;
                                                    contrastEffect.CacheOutput = true;
                                                    contrastEffect.Contrast = (float)effect.Value1;
                                                    outputResult = contrastEffect;
                                                    break;

                                                case "DirectionalBlurEffect":
                                                    DirectionalBlurEffect directionalBlurEffect = new DirectionalBlurEffect();
                                                    directionalBlurEffect.Source = outputResult;
                                                    directionalBlurEffect.CacheOutput = true;
                                                    directionalBlurEffect.BlurAmount = (float)effect.Value1;
                                                    directionalBlurEffect.Angle = (float)effect.Value2;
                                                    outputResult = directionalBlurEffect;
                                                    break;

                                                case "EdgeDetectionEffect":
                                                    EdgeDetectionEffect edgeDetectionEffect = new EdgeDetectionEffect();
                                                    edgeDetectionEffect.Source = outputResult;
                                                    edgeDetectionEffect.CacheOutput = true;
                                                    edgeDetectionEffect.Amount = (float)effect.Value1;
                                                    edgeDetectionEffect.BlurAmount = (float)effect.Value2;
                                                    outputResult = edgeDetectionEffect;
                                                    break;

                                                case "EmbossEffect":
                                                    EmbossEffect embossEffect = new EmbossEffect();
                                                    embossEffect.Source = outputResult;
                                                    embossEffect.CacheOutput = true;
                                                    embossEffect.Amount = (float)effect.Value1;
                                                    embossEffect.Angle = (float)effect.Value2;
                                                    outputResult = embossEffect;
                                                    break;

                                                case "ExposureEffect":
                                                    ExposureEffect exposureEffect = new ExposureEffect();
                                                    exposureEffect.Source = outputResult;
                                                    exposureEffect.CacheOutput = true;
                                                    exposureEffect.Exposure = (float)effect.Value1;
                                                    outputResult = exposureEffect;
                                                    break;

                                                case "GaussianBlurEffect":
                                                    GaussianBlurEffect gaussianBlurEffect = new GaussianBlurEffect();
                                                    gaussianBlurEffect.Source = outputResult;
                                                    gaussianBlurEffect.CacheOutput = true;
                                                    gaussianBlurEffect.BlurAmount = (float)effect.Value1;
                                                    outputResult = gaussianBlurEffect;
                                                    break;

                                                case "GrayscaleEffect":
                                                    GrayscaleEffect grayscaleEffect = new GrayscaleEffect();
                                                    grayscaleEffect.Source = outputResult;
                                                    grayscaleEffect.CacheOutput = true;
                                                    outputResult = grayscaleEffect;
                                                    break;

                                                case "InvertEffect":
                                                    InvertEffect invertEffect = new InvertEffect();
                                                    invertEffect.Source = outputResult;
                                                    invertEffect.CacheOutput = true;
                                                    outputResult = invertEffect;
                                                    break;

                                                case "HueToRgbEffect":
                                                    HueToRgbEffect hueToRgbEffect = new HueToRgbEffect();
                                                    hueToRgbEffect.Source = outputResult;
                                                    hueToRgbEffect.CacheOutput = true;
                                                    outputResult = hueToRgbEffect;
                                                    break;

                                                case "RgbToHueEffect":
                                                    RgbToHueEffect rgbToHueEffect = new RgbToHueEffect();
                                                    rgbToHueEffect.Source = outputResult;
                                                    rgbToHueEffect.CacheOutput = true;
                                                    outputResult = rgbToHueEffect;
                                                    break;

                                                case "HighlightsAndShadowsEffect":
                                                    HighlightsAndShadowsEffect highlightsAndShadowsEffect = new HighlightsAndShadowsEffect();
                                                    highlightsAndShadowsEffect.Source = outputResult;
                                                    highlightsAndShadowsEffect.CacheOutput = true;
                                                    highlightsAndShadowsEffect.Clarity = (float)effect.Value1;
                                                    highlightsAndShadowsEffect.Highlights = (float)effect.Value2;
                                                    highlightsAndShadowsEffect.Shadows = (float)effect.Value3;
                                                    outputResult = highlightsAndShadowsEffect;
                                                    break;

                                                case "PosterizeEffect":
                                                    PosterizeEffect posterizeEffect = new PosterizeEffect();
                                                    posterizeEffect.Source = outputResult;
                                                    posterizeEffect.CacheOutput = true;
                                                    posterizeEffect.RedValueCount = (int)effect.Value1;
                                                    posterizeEffect.GreenValueCount = (int)effect.Value2;
                                                    posterizeEffect.BlueValueCount = (int)effect.Value3;
                                                    outputResult = posterizeEffect;
                                                    break;

                                                case "MorphologyEffect":
                                                    MorphologyEffect morphologyEffect = new MorphologyEffect();
                                                    morphologyEffect.Source = outputResult;
                                                    morphologyEffect.CacheOutput = true;
                                                    morphologyEffect.Height = (int)effect.Value1;
                                                    outputResult = morphologyEffect;
                                                    break;

                                                case "SaturationEffect":
                                                    SaturationEffect saturationEffect = new SaturationEffect();
                                                    saturationEffect.Source = outputResult;
                                                    saturationEffect.CacheOutput = true;
                                                    saturationEffect.Saturation = (float)effect.Value1;
                                                    outputResult = saturationEffect;
                                                    break;

                                                case "ScaleEffect":
                                                    ScaleEffect scaleEffect = new ScaleEffect();
                                                    scaleEffect.Source = outputResult;
                                                    scaleEffect.CacheOutput = true;
                                                    scaleEffect.Scale = new Vector2((float)effect.Value1, (float)effect.Value2);
                                                    scaleEffect.CenterPoint = new Vector2((float)viewportWidth / 2, (float)viewportHeight / 2);
                                                    scaleEffect.Sharpness = (float)effect.Value3;
                                                    outputResult = scaleEffect;
                                                    break;

                                                case "SepiaEffect":
                                                    SepiaEffect sepiaEffect = new SepiaEffect();
                                                    sepiaEffect.Source = outputResult;
                                                    sepiaEffect.CacheOutput = true;
                                                    sepiaEffect.Intensity = (float)effect.Value1;
                                                    outputResult = sepiaEffect;
                                                    break;

                                                case "SharpenEffect":
                                                    SharpenEffect sharpenEffect = new SharpenEffect();
                                                    sharpenEffect.Source = outputResult;
                                                    sharpenEffect.CacheOutput = true;
                                                    sharpenEffect.Amount = (float)effect.Value1;
                                                    outputResult = sharpenEffect;
                                                    break;

                                                case "StraightenEffect":
                                                    StraightenEffect straightenEffect = new StraightenEffect();
                                                    straightenEffect.Source = outputResult;
                                                    straightenEffect.CacheOutput = true;
                                                    straightenEffect.Angle = (float)effect.Value1;
                                                    straightenEffect.MaintainSize = true;
                                                    straightenEffect.InterpolationMode = interpolation;
                                                    outputResult = straightenEffect;
                                                    break;

                                                case "TemperatureAndTintEffect":
                                                    TemperatureAndTintEffect temperatureAndTintEffect = new TemperatureAndTintEffect();
                                                    temperatureAndTintEffect.Source = outputResult;
                                                    temperatureAndTintEffect.CacheOutput = true;
                                                    temperatureAndTintEffect.Temperature = (float)effect.Value1;
                                                    temperatureAndTintEffect.Tint = (float)effect.Value2;
                                                    outputResult = temperatureAndTintEffect;
                                                    break;

                                                case "TileEffect":
                                                    TileEffect tileEffect = new TileEffect();
                                                    tileEffect.Source = outputResult;
                                                    tileEffect.CacheOutput = true;
                                                    tileEffect.SourceRectangle = new Rect(effect.Value1, effect.Value2, effect.Value3, effect.Value4);
                                                    outputResult = tileEffect;
                                                    break;

                                                case "CropEffect":
                                                    CropEffect cropEffect = new CropEffect();
                                                    cropEffect.Source = outputResult;
                                                    cropEffect.CacheOutput = true;
                                                    cropEffect.SourceRectangle = new Rect(effect.Value1, effect.Value2, effect.Value3, effect.Value4);
                                                    outputResult = cropEffect;
                                                    break;

                                                case "VignetteEffect":
                                                    VignetteEffect vignetteEffect = new VignetteEffect();
                                                    vignetteEffect.Source = outputResult;
                                                    vignetteEffect.CacheOutput = true;
                                                    vignetteEffect.Amount = (float)effect.Value1;
                                                    vignetteEffect.Curve = (float)effect.Value2;
                                                    outputResult = vignetteEffect;
                                                    break;

                                                case "Transform3DEffect":
                                                    Transform3DEffect transform3DEffect = new Transform3DEffect();
                                                    transform3DEffect.Source = outputResult;
                                                    transform3DEffect.CacheOutput = true;
                                                    transform3DEffect.TransformMatrix = Matrix4x4.CreateRotationZ((float)effect.Value1, new Vector3((float)viewportWidth / 2, (float)viewportHeight / 2, 0)) * Matrix4x4.CreateRotationX((float)effect.Value2, new Vector3((float)viewportWidth / 2, (float)viewportHeight / 2, 0)) * Matrix4x4.CreateRotationY((float)effect.Value3, new Vector3((float)viewportWidth / 2, (float)viewportHeight / 2, 0));
                                                    outputResult = transform3DEffect;
                                                    break;
                                            }
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }

                                    if (!isBlendModeSet || outputOverlays.Count == 0)
                                    {
                                        drawingSession.DrawImage(outputResult);
                                    }
                                    if (outputOverlays.Count > 0)
                                    {
                                        foreach (var overlayItem in outputOverlays)
                                        {
                                            if (isBlendModeSet)
                                            {
                                                if (FitOverlay)
                                                {
                                                    ScaleEffect scaleEffect = new ScaleEffect();
                                                    scaleEffect.Source = overlayItem;
                                                    var scaleFactorWidth = viewportWidth / overlayItem.SizeInPixels.Width;
                                                    var scaleFactorHeight = viewportWidth / overlayItem.SizeInPixels.Height;
                                                    scaleEffect.Scale = new Vector2((float)scaleFactorWidth, (float)scaleFactorHeight);

                                                    BlendEffect blendEffect = new BlendEffect();
                                                    blendEffect.Foreground = scaleEffect;
                                                    blendEffect.Background = outputResult;
                                                    blendEffect.Mode = BlendEffectModeGlobal;
                                                    blendEffect.CacheOutput = true;
                                                    drawingSession.DrawImage(blendEffect);
                                                }
                                                else
                                                {
                                                    BlendEffect blendEffect = new BlendEffect();
                                                    blendEffect.Foreground = overlayItem;
                                                    blendEffect.Background = outputResult;
                                                    blendEffect.Mode = BlendEffectModeGlobal;
                                                    blendEffect.CacheOutput = true;
                                                    drawingSession.DrawImage(blendEffect);
                                                }
                                            }
                                            else
                                            {
                                                drawingSession.DrawImage(overlayItem);
                                            }
                                        }
                                    }
                                    if (outputShaders.Count > 0)
                                    {
                                        foreach (var shaderItem in outputShaders)
                                        {
                                            drawingSession.DrawImage(shaderItem);
                                        }
                                    }
                                }
                                if (saveToTemp)
                                {
                                    StorageFile tempFile = null;
                                    try
                                    {
                                        tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("EffectsTemp.png", CreationCollisionOption.ReplaceExisting);
                                    }
                                    catch (Exception ex)
                                    {
                                        tempFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("EffectsTemp.png", CreationCollisionOption.GenerateUniqueName);
                                    }
                                    using (var saveStream = (await tempFile.OpenStreamForWriteAsync()).AsRandomAccessStream())
                                    {
                                        await renderer.SaveAsync(saveStream, CanvasBitmapFileFormat.Png);
                                    }
                                    returnedFile = tempFile;
                                }
                                else
                                {
                                    BitmapImage image = new BitmapImage();

                                    using (InMemoryRandomAccessStream streamOut = new InMemoryRandomAccessStream())
                                    {
                                        await renderer.SaveAsync(streamOut, CanvasBitmapFileFormat.Png);
                                        await image.SetSourceAsync(streamOut);
                                    }
                                    ContentImage.Source = image;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _ = Helpers.ShowError(e);
                        }
                        try
                        {
                            taskCompletionSource.SetResult(true);
                        }
                        catch (Exception ex)
                        {

                        }
                    });
                });
                await taskCompletionSource.Task;
            }
            catch (Exception e)
            {
                _ = Helpers.ShowError(e);
            }

            isProcessing = Visibility.Collapsed;
            RaisePropertyChanged(nameof(isProcessing));
            isEffectsInProgress = false;
            return returnedFile;
        }
    }
}
