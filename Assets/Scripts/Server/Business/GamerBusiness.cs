using Zenject;

public class GamerBusiness
{
    [Inject]
    private readonly GamerSendBusiness gamerSendBusiness;

    public void UpdateLife(int clientId, float currentLife)
    {
        this.gamerSendBusiness.UpdateLife(clientId, currentLife);
    }

    public void UpdateMoney(int clientId, int currentMoney)
    {
        this.gamerSendBusiness.UpdateMoney(clientId, currentMoney);
    }
}