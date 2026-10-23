using DiscordClone.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http.Json;

namespace DiscordClone.DesktopClient;

public partial class CreateServerWindow : Window
{
    private readonly string _token;

    public CreateServerWindow(string token)
    {
        InitializeComponent();
        _token = token;
    }

    private async void OnCreateClick(object sender, RoutedEventArgs e)
    {
        var name = ServerNameTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(name)) return;

        using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7232/") };
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        var dto = new CreateGuildRequestDto(name, null);
        var response = await client.PostAsJsonAsync("api/guilds", dto);

        if (response.IsSuccessStatusCode)
        {
            DialogResult = true;
            Close();
        }
    }
}