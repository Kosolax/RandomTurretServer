using System.Threading.Tasks;

using Zenject;

public class TowerHandleBusiness
{
    [Inject]
    private readonly TowerBusiness towerBusiness;

    public Task InvokeTower(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        this.towerBusiness.InvokeTower(fromClient, clientIdCheck);

        return Task.CompletedTask;
    }

    public Task MergeTower(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        //recupérer l'index de la première tour
        int draggedTowerIndex = packet.ReadInt();
        //recupérer l'index de la ou on dépose la tour
        int droppedTowerIndex = packet.ReadInt();

        this.towerBusiness.MergeTower(fromClient, clientIdCheck, draggedTowerIndex, droppedTowerIndex);

        return Task.CompletedTask;
    }
}