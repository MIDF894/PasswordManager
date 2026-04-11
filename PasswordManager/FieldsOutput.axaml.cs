using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace PasswordManager;

public partial class FieldsOutput : Window
{
    public FieldsOutput(string data)
    {
        InitializeComponent();

        this.FindControl<TextBlock>("FieldName").Text = data;

        var titleBar = this.FindControl<Grid>("TitleBar");
        titleBar.PointerPressed += OnTitleBarPointerPressed;

        this.FindControl<Button>("MinimizeButton").Click += (_, _) => WindowState = WindowState.Minimized;

        this.FindControl<Button>("MaximizeButton").Click += (_, _) =>
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        this.FindControl<Button>("Shutdown").Click += (_, _) =>
        {
            this.Close();
        };

        this.Closing += Window_Closing;
        btnCopyEndValue.Click += btnCopyEndValue_Click;
    }

    private async void btnCopyEndValue_Click(object sender, RoutedEventArgs e)
    {
        var text = FOutputEndValue.Text;

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

        if (clipboard != null && !string.IsNullOrEmpty(text))
        {
            await clipboard.SetTextAsync(text);
        }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        FieldName.Text = null;
        FOutputData.Clear();
        FOutputEndValue.Clear();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {
            FieldName.Text = null;
            FOutputData.Clear();
            FOutputEndValue.Clear();
            this.Close();
        }
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
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