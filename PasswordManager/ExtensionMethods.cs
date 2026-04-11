using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace PasswordManager
{
    internal static class ExtensionMethods
    {
        public static SecureString Secure(this String value)
        {
            SecureString securedStringValue = new SecureString();
            if (!(string.IsNullOrWhiteSpace(value)))
            {
                foreach (char c in value.ToCharArray())
                {
                    securedStringValue.AppendChar(c);
                }
            }
            else
            {
                return null;
            }
            return securedStringValue;
        }
        public static String UnSecure(this SecureString value)
        {
            var unsecuredString = IntPtr.Zero;
            try
            {
                unsecuredString = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(unsecuredString);
            }
            catch
            {
                return "";
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unsecuredString);
            }
        }

        public static void SaveSecureText(string textToSave)
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appData, "PasswordManager");
            string FilePath = Path.Combine(appFolder, "Field.data");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
                ApplyStrictPermissions(appFolder);
            }

            //try
            //{
            //    File.SetAttributes(FilePath, FileAttributes.Normal);
            //} catch (Exception e) {}

            File.WriteAllText(FilePath, textToSave);

            File.SetAttributes(appFolder, FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System);
            //File.SetAttributes(FilePath, FileAttributes.Hidden | FileAttributes.ReadOnly | FileAttributes.System);
        }
        public static string ReadSecureText()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PasswordManager", "Field.data");

            if (File.Exists(filePath))
            {
                // Временно снимаем атрибут скрытности для чтения, если нужно
                //File.SetAttributes(filePath, FileAttributes.Normal);
                return File.ReadAllText(filePath);
            }
            return string.Empty;
        }

        private static void ApplyStrictPermissions(string path)
        {
            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            dSecurity.SetAccessRuleProtection(true, false);

            SecurityIdentifier currentUser = WindowsIdentity.GetCurrent().User;

            dSecurity.AddAccessRule(new FileSystemAccessRule(currentUser,
                FileSystemRights.FullControl,
                AccessControlType.Allow));

            dInfo.SetAccessControl(dSecurity);
        }
    }
}
