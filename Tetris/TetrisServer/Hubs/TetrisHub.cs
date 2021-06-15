using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TetrisServer.Hubs
{
    public class TetrisHub : Hub
    {
        public async Task ReadyUp(int seed) => await Clients.Others.SendAsync("ReadyUp", seed);

        public async Task StartGame(int seed) => await Clients.All.SendAsync("StartGame", seed);

        public async Task SendBoard(string board) => await Clients.Others.SendAsync("SendBoard", board);

        public async Task SendTetromino(string tetromino) => await Clients.Others.SendAsync("SendTetromino", tetromino);

        public async Task SendNextTetromino(string tetromino) => await Clients.Others.SendAsync("SendNextTetromino", tetromino);

        public async Task SendScore(string score) => await Clients.Others.SendAsync("SendScore", score);

        public async Task SendGameStatus(bool status) => await Clients.Others.SendAsync("SendGameStatus", status);
    }
}