using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;

namespace TetrisClient
{
    public partial class MultiplayerWindow
    {
        private HubConnection _connection;
        private TetrisEngine _engine = new();
        private DispatcherTimer _renderTimer;
        private Random P1Random;
        private Random P2Random;

        public MultiplayerWindow()
        {
            InitializeComponent();

            // Url that TetrisHub will run on.
            const string url = "http://127.0.0.1:5000/TetrisHub";

            // The Builder that helps us create the connection.
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            // The first parameter has to be the same as the one in TetrisHub.cs
            // The type specified between <..> determines what the type of the parameter `seed` is.
            // This way the code below corresponds with the method in TetrisHub.cs
            _connection.On<int>("ReadyUp", seed =>
            {
                // Seed van de andere client:
                P2Random = new Random(seed);
                MessageBox.Show(seed.ToString());
                _connection.InvokeAsync("StartGame", seed);
            });

            _connection.On<int>("StartGame", seed =>
            {
                StartGame(seed);
            });


            // It is mandatory that the connection is started *after* all event listeners are set.
            // If the method this occurs in happens to be `async`, Task.Run can be removed.
            // It is necessary because of the constructor.
            Task.Run(async () => await _connection.StartAsync());
        }

        /// <summary>
        /// Generates a seed and gives it to P1Random, then fires the ReadyUp event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StartGame_OnClick(object sender, RoutedEventArgs e)
        {
            // If the connection isn't initialized, nothing can be sent to it.
            if (_connection.State != HubConnectionState.Connected) return;
            var seed = Guid.NewGuid().GetHashCode();
            P1Random = new Random(seed);

            // Calls `ReadyUp` from the TetrisHub.cs and gives the int it expects
            await _connection.InvokeAsync("ReadyUp", seed);
        }

        private void StartGame(int seed)
        {
            Dispatcher.Invoke(() => { ReadyButton.Visibility = Visibility.Hidden; });
            _engine.StartGame(seed);
        }
    }
}