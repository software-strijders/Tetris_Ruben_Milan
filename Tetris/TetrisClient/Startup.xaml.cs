using System;
using System.Windows;
using System.Windows.Controls;

namespace TetrisClient
{
    public partial class Startup
    {
        public Startup()
        {
            InitializeComponent();
        }

        private void HandleButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var button = (Button) sender;
            Window window = button.Content switch
            {
                "Single player" => new MainWindow(),
                "Multiplayer" => new MultiplayerWindow(),
                _ => throw new Exception("Invalid option")
            };

            Hide();
            window.Closed += (_, _) => Close();
            window.Show();
        }
    }
}