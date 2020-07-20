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

namespace SpotifyTransferVkMusic
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class SpotifyPage : Page
    {
        private TransferAgent transferAgent;

        public SpotifyPage()
        {
            InitializeComponent();
            var agent = new TransferAgent();
            agent.SpotifyAgent.OnAuthEvent += () => Dispatcher.Invoke(NextPage);
            transferAgent = agent;
            spotifyAuthButton.Click += async (o, e) => await agent.SpotifyAuth();
            LayoutUpdated += (o, e) => CloseNavBar();
        }

        private void CloseNavBar()
        {
            var navWindow = Window.GetWindow(this) as NavigationWindow;
            if (navWindow != null) navWindow.ShowsNavigationUI = false;
        }

        private void NextPage()
        {
            NavigationService.Navigate(new VkPage(transferAgent));
        }
    }
}
