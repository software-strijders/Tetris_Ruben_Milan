using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

namespace TetrisClient
{
    public partial class MultiplayerWindow : Window
    {
        private HubConnection _connection;
        private Random P1Random;
        private Random P2Random;
        
        public MultiplayerWindow()
        {
            InitializeComponent();

            // De url waar de meegeleverde TetrisHub op draait:
            string url = "http://127.0.0.1:5000/TetrisHub"; 
            
            // De Builder waarmee de connectie aangemaakt wordt:
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();
            
            // De eerste paramater moet gelijk zijn met de methodenaam in TetrisHub.cs
            // Wat er tussen de <..> staat bepaald wat de type van de paramater `seed` is.
            // Op deze manier loopt het onderstaande gelijk met de methode in TetrisHub.cs.
            _connection.On<int>("ReadyUp", seed =>
            {
                // Seed van de andere client:
                P2Random = new Random(seed);
                MessageBox.Show(seed.ToString());
            });
            
            // Let op: het starten van de connectie moet *nadat* alle event listeners zijn gezet!
            // Als de methode waarin dit voorkomt al `async` (asynchroon) is, dan kan `Task.Run` weggehaald worden.
            // In het startersproject staat dit in de constructor, daarom is dit echter wel nodig:
            Task.Run(async () => await _connection.StartAsync());
        }

        // Events kunnen `async` zijn in WPF:
        private async void StartGame_OnClick(object sender, RoutedEventArgs e)
        {
            // Als de connectie nog niet is geïnitialiseerd, dan kan er nog niks verstuurd worden:
            if (_connection.State != HubConnectionState.Connected)
            {
                return;
            }
            
            int seed = Guid.NewGuid().GetHashCode();
            
            P1Random = new Random(seed);

            // Het aanroepen van de TetrisHub.cs methode `ReadyUp`.
            // Hier geven we de int mee die de methode `ReadyUp` verwacht.
            await _connection.InvokeAsync("ReadyUp", seed);
        }
    }
}
