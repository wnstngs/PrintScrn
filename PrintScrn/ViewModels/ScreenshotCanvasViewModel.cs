using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PrintScrn.Capture;
using PrintScrn.Commands;
using PrintScrn.Image;

namespace PrintScrn.ViewModels
{
    public class ScreenshotCanvasViewModel : BaseViewModel
    {
        public ScreenshotCanvasViewModel()
        {
            ViewModels.Instance.ViewModelsStore.Add(this);

            OnInitCmd = new RelayCommand(
                OnExecuted_OnInitCmd,
                CanExecute_OnInitCmd
            );
        }
        
        private Bitmap? _fullscreenInitialBitmap;

        #region Properties

        #region ScreenImageSource

        private ImageSource? _screenshotCanvasImageSource;

        public ImageSource? ScreenshotCanvasImageSource
        {
            get => _screenshotCanvasImageSource;
            set => Set(ref _screenshotCanvasImageSource, value);
        }

        #endregion

        #region SelectedRectImageSource

        private ImageSource? _selectedRectImageSource;

        public ImageSource? SelectedRectImageSource
        {
            get => _selectedRectImageSource;
            set
            {
                if (_selectedRectWidthScreenCoords <= 0 || _selectedRectHeightScreenCoords <= 0)
                {
                    return;
                }

                if (_selectedRectXPositionScreenCoords < 0 || _selectedRectYPositionScreenCoords < 0)
                {
                    return;
                }

                var cBmp = _fullscreenInitialBitmap.Crop(
                    new Rectangle(
                        (int) Math.Round(_selectedRectXPositionScreenCoords),
                        (int) Math.Round(_selectedRectYPositionScreenCoords),
                        (int) Math.Round(_selectedRectWidthScreenCoords),
                        (int) Math.Round(_selectedRectHeightScreenCoords)
                    )
                );

                Set(ref _selectedRectImageSource, cBmp.ToBitmapImage());
            }
        }

        #endregion

        #region SelectedRectXPosition

        private double _selectedRectXPosition;

        public double SelectedRectXPosition
        {
            get => _selectedRectXPosition;
            set => Set(ref _selectedRectXPosition, value);
        }

        #endregion

        #region SelectedRectYPosition

        private double _selectedRectYPosition;

        public double SelectedRectYPosition
        {
            get => _selectedRectYPosition;
            set => Set(ref _selectedRectYPosition, value);
        }

        #endregion

        #region SelectedRectWidth

        private double _selectedRectWidth;

        public double SelectedRectWidth
        {
            get => _selectedRectWidth;
            set => Set(ref _selectedRectWidth, value);
        }

        #endregion

        #region SelectedRectHeight

        private double _selectedRectHeight;

        public double SelectedRectHeight
        {
            get => _selectedRectHeight;
            set => Set(ref _selectedRectHeight, value);
        }

        #endregion

        #region SelectedRectXPositionScreenCoords

        private double _selectedRectXPositionScreenCoords;

        public double SelectedRectXPositionScreenCoords
        {
            get => _selectedRectXPositionScreenCoords;
            set => Set(ref _selectedRectXPositionScreenCoords, value);
        }

        #endregion

        #region SelectedRectYPositionScreenCoords

        private double _selectedRectYPositionScreenCoords;

        public double SelectedRectYPositionScreenCoords
        {
            get => _selectedRectYPositionScreenCoords;
            set => Set(ref _selectedRectYPositionScreenCoords, value);
        }

        #endregion

        #region SelectedRectWidthScreenCoords

        private double _selectedRectWidthScreenCoords;

        public double SelectedRectWidthScreenCoords
        {
            get => _selectedRectWidthScreenCoords;
            set => Set(ref _selectedRectWidthScreenCoords, value);
        }

        #endregion

        #region SelectedRectHeightScreenCoords

        private double _selectedRectHeightScreenCoords;

        public double SelectedRectHeightScreenCoords
        {
            get => _selectedRectHeightScreenCoords;
            set => Set(ref _selectedRectHeightScreenCoords, value);
        }

        #endregion

        #endregion

        #region Commands

        #region OnInitCmd

        public ICommand OnInitCmd { get; }

        private static bool CanExecute_OnInitCmd(object p)
        {
            return true;
        }

        private void OnExecuted_OnInitCmd(object p)
        {
            _fullscreenInitialBitmap = Screenshot.Fullscreen();
            if (_fullscreenInitialBitmap != null)
            {
                ScreenshotCanvasImageSource = _fullscreenInitialBitmap.ToBitmapImage();
            }
        }

        #endregion

        #endregion
    }
}
