using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace PasswordManager
{
    public partial class FieldsOutput
    {
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
    }
}
