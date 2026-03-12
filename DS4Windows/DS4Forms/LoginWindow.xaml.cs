using KeyAuth;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DS4WinWPF.DS4Forms
{
    public partial class LoginWindow : Window
    {
        public bool LoginSuccessful { get; private set; } = false;

        public static Api KeyAuthApp = new Api(
              name: KeyAuthConfig.AppName,
              ownerid: KeyAuthConfig.OwnerId,
              version: "1.0"
          );

        public LoginWindow()
        {
            InitializeComponent();
            VersionLabel.Text = $"Versión {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await KeyAuthApp.init();

            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string token = TokenTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                StatusLabel.Text = "Por favor, completa todos los campos";
                return;
            }

            try
            {
                LoginButton.IsEnabled = false;
                StatusLabel.Text = "Verificando credenciales...";

                if (string.IsNullOrEmpty(token))
                {
                    await KeyAuthApp.login(username, password);
                }
                else
                {
                    await KeyAuthApp.register(username, password, token);
                }

                if (KeyAuthApp.response.success)
                {
                    LoginSuccessful = true;
                    Environment.SetEnvironmentVariable("config",
                        KeyAuthApp.user_data.subscriptions.First().subscription);
                    StatusLabel.Text = "Login exitoso";
                    DialogResult = true;
                    Close();
                }
                else
                {
                    StatusLabel.Text = "Credenciales o token inválidos";
                    LoginButton.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StatusLabel.Text = "Error de conexión: " + ex.Message;
                LoginButton.IsEnabled = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}