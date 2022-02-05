using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PrintScrn.Capture;
using PrintScrn.Commands;
using PrintScrn.Image;
using PrintScrn.Native;

namespace PrintScrn.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        private Bitmap? _fullscreenInitialBitmap;

        public MainViewModel()
        {
            // TODO: Get width and height of the monitor where window is.
            // We can't use PrintScrn.Native.Win32Fn.GetMonitorRectFromWindow() as it
            // is using GetMonitorFromWindow, but window isn't initialized yet.
            MonitorWidth = 2560;
            MonitorHeight = 1440;

            OnInitCmd = new CommandImpl(
                OnExecuted_OnInitCmd,
                CanExecute_OnInitCmd
            );
            QuitAppCmd = new CommandImpl(
                OnExecuted_QuitAppCmd,
                CanExecute_QuitAppCmd
            );
        }


        #region Properties

        #region MonitorWidth

        private int _monitorWidth;

        public int MonitorWidth
        {
            get => _monitorWidth;
            private set => Set(ref _monitorWidth, value);
        }

        #endregion

        #region MonitorHeight

        private int _monitorHeight;

        public int MonitorHeight
        {
            get => _monitorHeight;
            private set => Set(ref _monitorHeight, value);
        }

        #endregion

        #region WindowOpacity

        private double _windowOpacity = 1.0;

        public double WindowOpacity
        {
            get => _windowOpacity;
            private set => Set(ref _windowOpacity, value);
        }

        #endregion

        #region ScreenImageSource

        private ImageSource? _screenImageSource;

        public ImageSource? ScreenImageSource
        {
            get => _screenImageSource;
            private set => Set(ref _screenImageSource, value);
        }

        #endregion

        #region SelectedRectImageSource

        private ImageSource? _selectedRectImageSource;

        public ImageSource? SelectedRectImageSource
        {
            get => _selectedRectImageSource;
            set
            {
                if (value is not BitmapSource bmpSrc)
                {
                    return;
                }

                if (_selectedRectWidth <= 0 || _selectedRectHeight <= 0)
                {
                    return;
                }

                if (_selectedRectXPosition < 0 || _selectedRectYPosition < 0)
                {
                    return;
                }

                var croppedBitmap = new CroppedBitmap(
                    bmpSrc,
                    new Int32Rect
                    {
                        X = (int) Math.Round(_selectedRectXPosition),
                        Y = (int) Math.Round(_selectedRectYPosition),
                        Width = (int) Math.Round(_selectedRectWidth),
                        Height = (int) Math.Round(_selectedRectHeight)
                    }
                );
                Set(ref _selectedRectImageSource, croppedBitmap);
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
            var hwnd = new WindowInteropHelper(Application.Current.MainWindow!).Handle;
            Win32Fn.GetClientRect(hwnd, out var clientRect);
            Win32Fn.ClipCursor(ref clientRect);

            WindowOpacity = 0.0;
            _fullscreenInitialBitmap = Snapshot.Fullscreen();
            if (_fullscreenInitialBitmap != null)
            {
                ScreenImageSource = _fullscreenInitialBitmap.ToBitmapImage();
            }
            WindowOpacity = 1.0;
        }

        #endregion

        #region QuitAppCmd

        public ICommand QuitAppCmd { get; }

        private static bool CanExecute_QuitAppCmd(object p)
        {
            return true;
        }

        private void OnExecuted_QuitAppCmd(object p)
        {
            Application.Current.Shutdown(0);
        }

        #endregion

        #endregion
    }
}
