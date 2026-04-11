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

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Escape)
        {

            FName.Clear();
            FIData.Clear();
            FEndValue.Clear();
            this.Close(false);
        }
    }

    private void EnterInput_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FName.Text) || string.IsNullOrWhiteSpace(FIData.Text) || string.IsNullOrWhiteSpace(FEndValue.Text))
        {
            MessageBox.Show("Input cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else
        {
            this.Close(true);
        }
    }

    private void OnTitleBarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.ClickCount == 1)
        {
            BeginMoveDrag(e);
        }
    }
}