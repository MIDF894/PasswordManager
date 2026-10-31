using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CustomMessageBox.Avalonia;
using ExCSS;

namespace PasswordManager
{
    public partial class Login
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Escape)
            {
                fMasterPassword.Clear();
                this.Close(false);
            }
        }

        private async void btnMasterPassword_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fMasterPassword.Text))
            {
                await MessageBox.Show("Поле не может быть пустым.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            else if (fMasterPassword.Text.Replace(" ", "").Length < 16)
            {
                await MessageBox.Show("Слишком короткий пароль.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                fMasterPassword.Clear();
            }
            else
            {
                this.Close(true);
            }
        }
    }
}
