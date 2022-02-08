using System;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
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
            UpdateSelectedRectCmd = new RelayCommand(
                OnExecuted_UpdateSelectedRectCmd,
                CanExecute_UpdateSelectedRectCmd
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
            set => UpdateSelectedRectCmd.Execute(null);
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

        #region UpdateSelectedRectCmd

        public ICommand UpdateSelectedRectCmd { get; }

        private static bool CanExecute_UpdateSelectedRectCmd(object? p = null)
        {
            return true;
        }

        private void OnExecuted_UpdateSelectedRectCmd(object? p = null)
        {
            if (_selectedRectWidth <= 0 || _selectedRectHeight <= 0)
            {
                return;
            }

            if (_selectedRectXPosition < 0 || _selectedRectYPosition < 0)
            {
                return;
            }

            var cBmp = _fullscreenInitialBitmap.Crop(
                new Rectangle(
                    (int) Math.Round(_selectedRectXPosition),
                    (int) Math.Round(_selectedRectYPosition),
                    (int) Math.Round(_selectedRectWidth),
                    (int) Math.Round(_selectedRectHeight)
                )
            );

            Set(ref _selectedRectImageSource, cBmp.ToBitmapImage());
        }

        #endregion

        #endregion
    }
}
