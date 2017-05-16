﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using QuickLook.Annotations;

namespace QuickLook.Plugin
{
    /// <summary>
    ///     A runtime object which allows interaction between this plugin and QuickLook.
    /// </summary>
    public class ContextObject : INotifyPropertyChanged
    {
        private bool _isBusy = true;

        private string _title = "";
        internal ContentControl CurrentContentContainer;
        internal IViewer ViewerPlugin;

        /// <summary>
        ///     Get or set the title of Viewer window.
        /// </summary>
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the viewer content control.
        /// </summary>
        public object ViewerContent
        {
            get => CurrentContentContainer.Content;
            set => CurrentContentContainer.Content = value;
        }

        /// <summary>
        ///     Show or hide the busy indicator icon.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Set the exact size you want.
        /// </summary>
        public Size PreferredSize { get; set; } = new Size {Width = 800, Height = 600};

        /// <summary>
        ///     Set whether user are allowed to resize the viewer window.
        /// </summary>
        public bool CanResize { get; set; } = true;

        /// <summary>
        ///     Set whether user are allowed to set focus at the viewer window.
        /// </summary>
        public bool Focusable { get; set; } = false;

        public void DisposePlugin()
        {
            ViewerPlugin?.Cleanup();
            ViewerPlugin = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Show a notification balloon.
        /// </summary>
        /// <param name="title">Title of the notification.</param>
        /// <param name="content">The content.</param>
        /// <param name="isError">Is this indicates a error?</param>
        public void ShowNotification(string title, string content, bool isError = false)
        {
            TrayIcon.GetInstance().ShowNotification(title, content, isError);
        }

        /// <summary>
        ///     Set the size of viewer window and shrink to fit (to screen resolution).
        ///     The window can take maximum (maxRatio*resolution) space.
        /// </summary>
        /// <param name="size">The desired size.</param>
        /// <param name="maxRatio">The maximum percent (over screen resolution) it can take.</param>
        public double SetPreferredSizeFit(Size size, double maxRatio)
        {
            if (maxRatio > 1)
                maxRatio = 1;

            var max = GetMaximumDisplayBound();

            var widthRatio = max.Width * maxRatio / size.Width;
            var heightRatio = max.Height * maxRatio / size.Height;

            var ratio = Math.Min(widthRatio, heightRatio);
            if (ratio > 1) ratio = 1;

            PreferredSize = new Size {Width = size.Width * ratio, Height = size.Height * ratio};

            return ratio;
        }

        /// <summary>
        ///     Get the device-independent resolution.
        /// </summary>
        public Size GetMaximumDisplayBound()
        {
            return new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
        }

        internal void Reset()
        {
            Title = "";
            ViewerContent = null;
            IsBusy = true;
            PreferredSize = new Size();
            CanResize = true;
            Focusable = false;
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}