using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CustomMessageBox.Avalonia;

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

        private void btnMasterPassword_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fMasterPassword.Text))
            {
                MessageBox.Show("Поле не может быть пустым.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (fMasterPassword.Text.Replace(" ", "").Length < 16)
            {
                MessageBox.Show("Слишком короткий пароль.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fMasterPassword.Clear();
            }
            else
            {
                this.Close(true);
            }
        }
    }
}
