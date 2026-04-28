using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CustomMessageBox.Avalonia;

namespace PasswordManager
{
    public partial class FieldsFilling
    {
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
    }
}
