using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TetrisServer.Hubs
{
    public class TetrisHub : Hub
    {
        public async Task DropShape()
        {
            await Clients.Others.SendAsync("DropShape");
        }

        public async Task RotateShape(string direction)
        {
            await Clients.Others.SendAsync("RotateShape", direction);
        }

        public async Task MoveShape(string moveDirection)
        {
            await Clients.Others.SendAsync("MoveShape", moveDirection);
        }
        
        public async Task ReadyUp(int seed)
        {
            await Clients.Others.SendAsync("ReadyUp", seed);
        }
    }
}
