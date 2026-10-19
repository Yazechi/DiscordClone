using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net.Http;
using System.Net.Http.Json;
using DiscordClone.Application.DTOs;

namespace DiscordClone.DesktopClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7232/")
        };
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void OnLoginClick(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Visibility = Visibility.Collapsed;

            var request = new LoginRequestDto(
                EmailTextBox.Text.Trim(),
                PasswordBox.Password
            );

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (result != null)
                    {
                        // Launch MainWindow passing authenticated user info & token
                        var mainWindow = new MainWindow(result.Token, result.Username);
                        mainWindow.Show();
                        Close();
                    }
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    ErrorTextBlock.Text = string.IsNullOrWhiteSpace(errorMsg) ? "Login failed." : errorMsg;
                    ErrorTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = $"Connection error: {ex.Message}";
                ErrorTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void OnRegisterToggleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var RegisterWindow = new RegisterWindow();
            RegisterWindow.Show();
            Close();
        }
    }
}