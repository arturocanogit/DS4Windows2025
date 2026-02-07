using System;
using System.Configuration;

namespace DS4WinWPF.Infraestructura
{
    public static class FtpConfig
    {
        public static string Server { get; set; } = "ftp://BSX.somee.com/www.BSX.somee.com";
        public static string Username { get; set; } = "BSX";
        public static string Password { get; set; } = "THEGATO3381999";
        public static bool EnableFtpUpload { get; set; } = true;

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
                // Si hay error cargando la configuración, usar valores por defecto
            }
        }
    }
}