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
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.SignalR.Client;

namespace DiscordClone.DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _token;
        private readonly string _username;
        private HubConnection? _hubConnection;
        private string _currentChannel = "general";
        public ObservableCollection<string> Messages { get; set; } = new();
        public MainWindow(string token, string username)
        {
            InitializeComponent();
            _token = token;
            _username = username;
            DataContext = this;

            InitializeSignalR();
        }
        
        private async void InitializeSignalR()
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7232/hubs/chat", options =>
                {
                   options.AccessTokenProvider = () => Task.FromResult<string?>(_token);
                })
                .WithAutomaticReconnect()
                .Build();
            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Messages.Add($"{user}: {message}");
                });
            });
            try
            {
                await _hubConnection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SignalR Connection Error: {ex.Message}");
            }
        }
        private async Task SwitchChannel(string newChannel)
        {
            if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected) return;

            // Leave previous channel group
            if (!string.IsNullOrEmpty(_currentChannel))
            {
                await _hubConnection.InvokeAsync("LeaveChannel", _currentChannel);
            }

            _currentChannel = newChannel;

            // Join new channel group
            await _hubConnection.InvokeAsync("JoinChannel", _currentChannel);

            Messages.Clear();
            Messages.Add($"System: Joined #{_currentChannel} as {_username}.");
        }

        private async void OnChannelClick(object sender, MouseButtonEventArgs e)
        {
            await SwitchChannel("general");
        }

        private async void SendCurrentMessage()
        {
            var text = MessageTextBox.Text.Trim();
            if (string.IsNullOrEmpty(text) || _hubConnection == null) return;

            try
            {
                // Send Message Through SignalR Hub
                await _hubConnection.InvokeAsync("SendMessageToChannel", "general", _username, text);
                MessageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message: {ex.Message}");
            }
        }

        private void OnSendMessageClick(object sender, RoutedEventArgs e)
        {
            SendCurrentMessage();
        }

        private void OnMessageTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendCurrentMessage();
            }
        }
    }
}