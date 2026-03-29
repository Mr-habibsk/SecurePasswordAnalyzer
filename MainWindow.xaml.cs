using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SecurePasswordAnalyzer
{
    public partial class MainWindow : Window
    {
        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private string GetPassword()
        {
            if (txtPassword.Visibility == Visibility.Visible)
                return txtPassword.Password;

            return txtVisiblePassword.Text;
        }

        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            AnalyzePassword();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            AnalyzePassword();
        }

        private void AnalyzePassword()
        {
            string password = GetPassword();

            bool upper = Regex.IsMatch(password, "[A-Z]");
            bool lower = Regex.IsMatch(password, "[a-z]");
            bool number = Regex.IsMatch(password, "[0-9]");
            bool symbol = Regex.IsMatch(password, "[^a-zA-Z0-9]");

            int score = 0;

            if (password.Length >= 8) score++;
            if (upper) score++;
            if (lower) score++;
            if (number) score++;
            if (symbol) score++;

            strengthBar.Value = score;

            if (score <= 2)
            {
                txtStrength.Text = "Strength: Weak Password";
                strengthBar.Foreground = Brushes.Red;
            }
            else if (score <= 4)
            {
                txtStrength.Text = "Strength: Medium Password";
                strengthBar.Foreground = Brushes.Orange;
            }
            else
            {
                txtStrength.Text = "Strength: Strong Password";
                strengthBar.Foreground = Brushes.LimeGreen;
            }

            txtCrackTime.Text = "Estimated Hack Time: " + CalculateHackTime(password);
            txtAdvice.Text = GetAdvice(password);
        }

        private string CalculateHackTime(string password)
        {
            int length = password.Length;

            double guesses = Math.Pow(94, length);
            double guessesPerSecond = 1000000000;

            double seconds = guesses / guessesPerSecond;

            if (seconds < 60)
                return seconds.ToString("0") + " seconds";

            if (seconds < 3600)
                return (seconds / 60).ToString("0") + " minutes";

            if (seconds < 86400)
                return (seconds / 3600).ToString("0") + " hours";

            if (seconds < 31536000)
                return (seconds / 86400).ToString("0") + " days";

            return (seconds / 31536000).ToString("0") + " years";
        }

        private string GetAdvice(string password)
        {
            if (password.Length < 8)
                return "Use at least 8 characters.";

            if (!Regex.IsMatch(password, "[A-Z]"))
                return "Add uppercase letters.";

            if (!Regex.IsMatch(password, "[0-9]"))
                return "Add numbers.";

            if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
                return "Add special characters.";

            return "Good password! Hard to crack.";
        }

        private void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < 12; i++)
                password.Append(chars[rnd.Next(chars.Length)]);

            txtPassword.Password = password.ToString();
        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtVisiblePassword.Text = txtPassword.Password;
            txtPassword.Visibility = Visibility.Collapsed;
            txtVisiblePassword.Visibility = Visibility.Visible;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPassword.Password = txtVisiblePassword.Text;
            txtPassword.Visibility = Visibility.Visible;
            txtVisiblePassword.Visibility = Visibility.Collapsed;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}