using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkNet;
using VkNet.Model;

namespace SpotifyTransferVkMusic
{
    /// <summary>
    /// Логика взаимодействия для VkPage.xaml
    /// </summary>
    public partial class VkPage : Page
    {
        private TransferAgent agent;
        private const string wrongPassMessage = "Неправильный логин или пароль";

        public VkPage(TransferAgent agent)
        {
            InitializeComponent();
            this.agent = agent;
            AuthButton.Click += (o, e) => Auth();
        }

        private void Auth()
        {
            try
            {
                agent.VkAgent.VkApi.Authorize(new ApiAuthParams
                {
                    Login = LoginTextBox.Text,
                    Password = PasswordTextBox.Text
                });
            }
            catch
            {
                ErrorLabel.Content = wrongPassMessage;
                return;
            }

            NextPage();
        }

        private void NextPage()
        {
            NavigationService.Navigate(new TransferPage(agent));
        }
    }
}
