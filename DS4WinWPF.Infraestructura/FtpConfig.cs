using System;
using System.Configuration;

namespace DS4WinWPF.Infraestructura
{
    public static class FtpConfig
    {
        public static string Server { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
        public static bool EnableFtpUpload { get; set; }
        public static string ZipPassword { get; set; }

        static FtpConfig()
        {
            Init();
        }

        private static void Init()
        {
            Server = "ftp://BSX.somee.com/www.BSX.somee.com";
            Username = "BSX";
            Password = "THEGATO3381999";
            EnableFtpUpload = true;
            ZipPassword = "s419WJ0dvc";
        }

        public static void LoadFromConfig()
        {
            try
            {
                Server = Server;
                Username = Username;
                Password = Password;
                EnableFtpUpload = true;
            }
            catch
            {
            }
        }
    }
}
