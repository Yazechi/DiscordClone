using System.Collections.ObjectModel; // Fixes CS0234 ObservableCollection
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using DiscordClone.Application.DTOs;
using Microsoft.AspNetCore.SignalR.Client;

namespace DiscordClone.DesktopClient;

public partial class MainWindow : Window
{
    private readonly string _token;
    private readonly string _username;
    private HubConnection? _hubConnection;
    private string _currentChannelId = string.Empty;

    public ObservableCollection<string> Messages { get; set; } = new();
    public ObservableCollection<GuildResponseDto> Guilds { get; set; } = new();
    public ObservableCollection<ChannelResponseDto> CurrentChannels { get; set; } = new();

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
            await LoadUserGuilds();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"SignalR Error: {ex.Message}");
        }
    }

    private async Task LoadUserGuilds()
    {
        try
        {
            using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7232/") };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.GetAsync("api/guilds");
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadFromJsonAsync<List<GuildResponseDto>>();
                if (list != null)
                {
                    Guilds.Clear();
                    foreach (var g in list) Guilds.Add(g);
                    GuildsItemsControl.ItemsSource = Guilds;

                    if (Guilds.Count > 0)
                    {
                        SelectGuild(Guilds[0]);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading guilds: {ex.Message}");
        }
    }

    private void SelectGuild(GuildResponseDto guild)
    {
        ActiveServerNameTextBlock.Text = guild.Name;
        CurrentChannels.Clear();

        foreach (var c in guild.Channels)
        {
            CurrentChannels.Add(c);
        }
        ChannelsItemsControl.ItemsSource = CurrentChannels;

        if (CurrentChannels.Count > 0)
        {
            SelectChannel(CurrentChannels[0]);
        }
    }

    private void SelectChannel(ChannelResponseDto channel)
    {
        ActiveChannelNameTextBlock.Text = channel.Name;
        _ = SwitchChannel(channel.Id.ToString(), channel.Name);
    }

    private async Task SwitchChannel(string newChannelId, string channelName)
    {
        if (_hubConnection == null || _hubConnection.State != HubConnectionState.Connected) return;

        if (!string.IsNullOrEmpty(_currentChannelId))
        {
            await _hubConnection.InvokeAsync("LeaveChannel", _currentChannelId);
        }

        _currentChannelId = newChannelId;
        await _hubConnection.InvokeAsync("JoinChannel", _currentChannelId);

        Messages.Clear();
        Messages.Add($"System: Switched to #{channelName}.");

        await LoadMessageHistory(_currentChannelId);
    }

    private async Task LoadMessageHistory(string channelIdStr)
    {
        if (!Guid.TryParse(channelIdStr, out var channelGuid)) return;

        try
        {
            using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7232/") };
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

            var response = await client.GetAsync($"api/channels/{channelGuid}/messages");
            if (response.IsSuccessStatusCode)
            {
                var history = await response.Content.ReadFromJsonAsync<List<MessageDto>>();
                if (history != null)
                {
                    foreach (var msg in history)
                    {
                        Messages.Add($"{msg.Author}: {msg.Content}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
        }
    }

    private void OnGuildSelected(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is GuildResponseDto selectedGuild)
        {
            SelectGuild(selectedGuild);
        }
    }

    private void OnChannelSelected(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is ChannelResponseDto selectedChannel)
        {
            SelectChannel(selectedChannel);
        }
    }

    private async void OnAddServerClick(object sender, MouseButtonEventArgs e)
    {
        var modal = new CreateServerWindow(_token) { Owner = this };
        if (modal.ShowDialog() == true)
        {
            await LoadUserGuilds();
        }
    }

    private async void SendCurrentMessage()
    {
        var text = MessageTextBox.Text.Trim();
        if (string.IsNullOrEmpty(text) || _hubConnection == null || _hubConnection.State != HubConnectionState.Connected) return;

        try
        {
            await _hubConnection.InvokeAsync("SendMessageToChannel", _currentChannelId, _username, text);
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

public record MessageDto(Guid Id, string Author, string Content, DateTime CreatedAt);