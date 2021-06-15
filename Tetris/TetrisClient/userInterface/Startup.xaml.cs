using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;

namespace TetrisClient
{
    public partial class Startup
    {
        private readonly SoundPlayer _themeSong = new (Resource1.TetrisTechno);
        
        public Startup()
        {
            InitializeComponent();
            _themeSong.PlayLooping();
        }

        private void HandleButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            _themeSong.Stop();
            var button = (Button) sender;
            Window window = (string) button.Content switch
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