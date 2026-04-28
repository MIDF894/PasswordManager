using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CustomMessageBox.Avalonia;

namespace PasswordManager;

public partial class Login : Window
{
    public Login()
    {
        InitializeComponent();

        this.FindControl<Button>("Shutdown").Click += (_, _) =>
        {
            this.Close(false);
        };

        btnMasterPassword.Click += btnMasterPassword_Click;

        fMasterPassword.MaxLength = 128;
    }
}