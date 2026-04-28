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
    public partial class MainWindow
    {
        SimpleProtector Protector = new SimpleProtector();

        List<Field> Fields = new List<Field>();

        public SecureString M { get; set; }

        private bool isFieldsPrivate = false;

        public class Field
        {
            public string FieldName { get; set; }
            public string Description { get; set; }
            public string Password { get; set; }
        }

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private async void Window_Opened(object? sender, EventArgs e)
        {
            Login MasterPassword = new Login();
            bool? result = await MasterPassword.ShowDialog<bool?>(this);

            if (result == true)
            {
                M = MasterPassword.fMasterPassword.Text.Replace(" ", "").Secure();
            }
            else
            {
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown(0);
                }
            }

            if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PasswordManager", "Field.data")))
            {
                MessageBox.Show("Если вы потеряете ключ, единственный способ сбросить пароль — удалить файл данных.", "Важная информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Fields = JsonSerializer.Deserialize<List<Field>>(Protector.Decrypt(ReadSecureText(), M.UnSecure()));
            }
            catch
            {
                await MessageBox.Show("Пароль неверный.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown(0);
                }
            }

            Div.Items.Clear();
            foreach (var f in Fields)
            {
                ListBoxItem Item = new ListBoxItem();
                var converter = new BrushConverter();
                Item.Content = f.FieldName;
                Div.Items.Add(Item);
            }

            MasterPassword = null;
        }

        private async void AddField_Click(object sender, RoutedEventArgs e)
        {
            FieldsFilling DataFields = new FieldsFilling();
            bool? result = await DataFields.ShowDialog<bool?>(this);
            if (result == true)
            {
                Fields.Add(new Field
                {
                    FieldName = DataFields.FName.Text,
                    Description = DataFields.FIData.Text,
                    Password = DataFields.FEndValue.Text,
                });

                ListBoxItem Item = new ListBoxItem();
                var converter = new BrushConverter();
                Item.Content = DataFields.FName.Text;
                Div.Items.Add(Item);

                DataFields.FName.Clear();
                DataFields.FIData.Clear();
                DataFields.FEndValue.Clear();
            }
            else
            {
                return;
            }

            SaveSecureText(Protector.Encrypt(JsonSerializer.Serialize(Fields, options), M.UnSecure()));

            DataFields = null;
        }

        private async void EditField_Click(object sender, RoutedEventArgs e)
        {

            if (Div.SelectedIndex == -1)
            {
                MessageBox.Show("Сначала выберите поле.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {

                FieldsFilling DataFields = new FieldsFilling();


                DataFields.TitleBarName.Text = "Изменение поля";

                DataFields.btnAddorEditFields.Content = "Изменить поле";

                DataFields.FName.Text = Fields[Div.SelectedIndex].FieldName;
                DataFields.FIData.Text = Fields[Div.SelectedIndex].Description;
                DataFields.FEndValue.Text = Fields[Div.SelectedIndex].Password;

                bool? result = await DataFields.ShowDialog<bool?>(this);

                if (result == true)
                {
                    Fields[Div.SelectedIndex].FieldName = DataFields.FName.Text;
                    Fields[Div.SelectedIndex].Description = DataFields.FIData.Text;
                    Fields[Div.SelectedIndex].Password = DataFields.FEndValue.Text;
                    Div.Items[Div.SelectedIndex] = DataFields.FName.Text;
                }
                else
                {
                    return;
                }

                SaveSecureText(Protector.Encrypt(JsonSerializer.Serialize(Fields, options), M.UnSecure()));

                DataFields = null;
            }
        }

        private async void DelField_Click(object sender, RoutedEventArgs e)
        {
            if (Div.SelectedIndex == -1)
            {
                MessageBox.Show("Сначала выберите поле.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var messageBox = new MessageBox("Вы уверены?", "Вопрос", MessageBoxIcon.Question);

                var result = await messageBox.Show(MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1, "accent");

                bool isConfirmed = result == MessageBoxResult.Yes;

                if (isConfirmed)
                {
                    Fields.RemoveAt(Div.SelectedIndex);

                    SaveSecureText(Protector.Encrypt(JsonSerializer.Serialize(Fields, options), M.UnSecure()));

                    if (Fields.Count() == 0)
                    {
                        File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PasswordManager", "Field.data"));
                    }
                    Div.Items.Remove(Div.SelectedItem);
                }
            }
        }

        private async void ListBoxItem_DoubleTapped(object sender, TappedEventArgs e)
        {
            if (Div.SelectedIndex == -1)
            {
                return;
            }
            else
            {
                if (isFieldsPrivate)
                {
                    Login MasterPassword = new Login();
                    bool? result = await MasterPassword.ShowDialog<bool?>(this);
                    if (result == true)
                    {
                        M = MasterPassword.fMasterPassword.Text.Secure();
                    }
                    else
                    {
                        return;
                    }
                    MasterPassword = null;
                }

                try
                {
                    Fields = JsonSerializer.Deserialize<List<Field>>(Protector.Decrypt(ReadSecureText(), M.UnSecure()));
                }
                catch
                {
                    await MessageBox.Show("Пароль неверный.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.Shutdown(0);
                    }
                }

                var item = sender as ListBoxItem;
                string FName = Fields[Div.SelectedIndex].FieldName;

                FieldsOutput Output = new FieldsOutput(FName);
                Output.Show();

                Output.Title = Fields[Div.SelectedIndex].FieldName;
                Output.FOutputData.Text = Fields[Div.SelectedIndex].Description;
                Output.FOutputEndValue.Text = Fields[Div.SelectedIndex].Password;
            }
        }

        private void LockFields_Click(object sender, RoutedEventArgs e)
        {
            if (isFieldsPrivate)
            {
                MessageBox.Show("Поля уже закрыты.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                M.Clear();
                M.Dispose();
                M = null;
                MessageBox.Show("Доступ к полям закрыт.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);

                isFieldsPrivate = true;
                AddField.IsVisible = false;
                EditField.IsVisible = false;
                DelField.IsVisible = false;
                AddField.Click -= AddField_Click;
                EditField.Click -= EditField_Click;
                DelField.Click -= DelField_Click;
            }
        }

        private async void UnLockFields_Click(object sender, RoutedEventArgs e)
        {
            if (isFieldsPrivate)
            {
                Login MasterPassword = new Login();
                bool? result = await MasterPassword.ShowDialog<bool?>(this);
                if (result == true)
                {
                    M = MasterPassword.fMasterPassword.Text.Secure();
                }

                MasterPassword = null;

                if (Protector.Decrypt(ReadSecureText(), M.UnSecure()) == null)
                {
                    await MessageBox.Show("Пароль неверный.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.Shutdown(0);
                    }
                    return;
                }

                MessageBox.Show("Доступ к полям открыт.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);

                isFieldsPrivate = false;
                AddField.IsVisible = true;
                EditField.IsVisible = true;
                DelField.IsVisible = true;
                AddField.Click += AddField_Click;
                EditField.Click += EditField_Click;
                DelField.Click += DelField_Click;
            }
            else
            {
                MessageBox.Show("Поля уже открыты.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
