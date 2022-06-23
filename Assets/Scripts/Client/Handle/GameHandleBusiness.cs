using System.Threading.Tasks;

using Zenject;

public class GameHandleBusiness
{
    [Inject]
    private readonly GameBusiness gameBusiness;

    public async Task AskForGame(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        await this.gameBusiness.AskForGame(fromClient, clientIdCheck);
    }

    public Task AskForPrivateGame(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        this.gameBusiness.AskForPrivateGame(fromClient, clientIdCheck);
        return Task.CompletedTask;
    }

    public async Task AskJoinPrivateGame(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        int lobbyCode = packet.ReadInt();
        await this.gameBusiness.AskJoinPrivateGame(fromClient, clientIdCheck, lobbyCode);
    }

    public Task CancelAskForGame(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        this.gameBusiness.CancelAskForGame(fromClient, clientIdCheck);
        return Task.CompletedTask;
    }

    public async Task LoadGameDone(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        await this.gameBusiness.LoadGameDone(fromClient, clientIdCheck);
    }
}