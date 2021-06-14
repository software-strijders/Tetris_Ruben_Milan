using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace TetrisClient
{
    public partial class Startup : Window
    {
        public Startup()
        {
            InitializeComponent();
        }

        private void HandleButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var button = (Button) sender;
            Window window = null;
            switch (button.Content)
            {
                case "Single player":
                    window = new MainWindow();
                    break;
                case "Multiplayer":
                    window = new MultiplayerWindow();
                    break;
            }

            Hide();
            Debug.Assert(window != null, nameof(window) + " != null");
            window.Closed += (s, args) => Close();
            window.Show();
        }
    }
}