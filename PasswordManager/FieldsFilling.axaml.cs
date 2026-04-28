using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CustomMessageBox.Avalonia;

namespace PasswordManager;

public partial class FieldsFilling : Window
{
    public FieldsFilling()
    {
        InitializeComponent();

        var titleBar = this.FindControl<Grid>("TitleBar");
        titleBar.PointerPressed += OnTitleBarPointerPressed;

        this.FindControl<Button>("MinimizeButton").Click += (_, _) => WindowState = WindowState.Minimized;

        this.FindControl<Button>("Shutdown").Click += (_, _) =>
        {
            this.Close(false);
        };

        btnAddorEditFields.Click += EnterInput_Click;
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            BeginMoveDrag(e);
        }
    }
}