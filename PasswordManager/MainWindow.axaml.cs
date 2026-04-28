using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using CustomMessageBox.Avalonia;
using ExCSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.Json;
using static PasswordManager.ExtensionMethods;

namespace PasswordManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ToolPanelControl();
            var titleBar = this.FindControl<Grid>("TitleBar");
            titleBar.PointerPressed += OnTitleBarPointerPressed;

            Opened += Window_Opened;
            AddField.Click += AddField_Click;
            EditField.Click += EditField_Click;
            DelField.Click += DelField_Click;
            Div.DoubleTapped += ListBoxItem_DoubleTapped;
            LockFields.Click += LockFields_Click;
            UnLockFields.Click += UnLockFields_Click;
            btnExit.Click += btnExit_Click;
        }

        private async void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown(0);
            }
        }

        private void ToolPanelControl()
        {
            this.FindControl<Button>("MinimizeButton").Click += (_, _) => WindowState = WindowState.Minimized;
            this.FindControl<Button>("MaximizeButton").Click += (_, _) =>
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            this.FindControl<Button>("CloseButton").Click += (_, _) =>
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
            };
        }

        private async void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                BeginMoveDrag(e);
            }
            else
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }
    }
}