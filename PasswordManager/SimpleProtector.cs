using Avalonia.Controls.ApplicationLifetimes;
using CustomMessageBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Avalonia;
using CustomMessageBox.Avalonia;

namespace PasswordManager
{
    public class SimpleProtector
    {
        private const int SaltSize = 16;
        private const int NonceSize = 12;
        private const int TagSize = 16;

        public string Encrypt(string passwordToHide, string masterPassword)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
            byte[] plainBytes = Encoding.UTF8.GetBytes(passwordToHide);

            using var pbkdf2 = new Rfc2898DeriveBytes(masterPassword, salt, 100000, HashAlgorithmName.SHA256);

            byte[] key = pbkdf2.GetBytes(32);
            byte[] cipherText = new byte[plainBytes.Length];
            byte[] tag = new byte[TagSize];

            using (var aesGcm = new AesGcm(key))
            {
                try
                {
                    aesGcm.Encrypt(nonce, plainBytes, cipherText, tag);
                }
                catch
                {
                    MessageBox.Show("Пароль неверный или данные повреждены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.Shutdown();
                    }
                }
            }

            byte[] result = salt.Concat(nonce).Concat(tag).Concat(cipherText).ToArray();

            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(cipherText);
            key = null;
            cipherText = null;

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string base64Data, string masterPassword)
        {
            byte[] fullData = Convert.FromBase64String(base64Data);

            byte[] salt = fullData.Take(SaltSize).ToArray();
            byte[] nonce = fullData.Skip(SaltSize).Take(NonceSize).ToArray();
            byte[] tag = fullData.Skip(SaltSize + NonceSize).Take(TagSize).ToArray();
            byte[] cipherText = fullData.Skip(SaltSize + NonceSize + TagSize).ToArray();

            using var pbkdf2 = new Rfc2898DeriveBytes(masterPassword, salt, 100000, HashAlgorithmName.SHA256);
            byte[] key = pbkdf2.GetBytes(32);

            byte[] decryptedBytes = new byte[cipherText.Length];

            using (var aesGcm = new AesGcm(key))
            {
                try
                {
                    aesGcm.Decrypt(nonce, cipherText, tag, decryptedBytes);
                }
                catch
                {
                    return null;
                }
            }

            CryptographicOperations.ZeroMemory(key);
            CryptographicOperations.ZeroMemory(cipherText);
            key = null;
            cipherText = null;

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
