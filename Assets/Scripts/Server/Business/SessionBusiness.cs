using System;
using System.Linq;
using System.Threading.Tasks;

using Zenject;

public class SessionBusiness
{
    [Inject]
    private readonly PlayerClient playerClient;

    [Inject]
    private readonly Serveur serveur;

    [Inject]
    private readonly SessionSendBusiness sessionSendBusiness;

    public async Task Connection(int clientId, int checkClientId, Player player)
    {
        try
        {
            if (clientId == checkClientId)
            {
                player = await this.playerClient.Connection(player);
                this.serveur.Clients[clientId].Player = player;
                this.sessionSendBusiness.DisplayPlayerData(clientId, player);
            }
            else
            {
                this.sessionSendBusiness.DisplayPlayerData(clientId, null);
            }
        }
        catch (ErrorException errorException)
        {
            if (errorException != null && errorException.Errors != null && errorException.Errors.Count > 0)
            {
                this.sessionSendBusiness.SendConnectionError(clientId, errorException.Errors.Keys.FirstOrDefault());
            }
            else
            {
                this.sessionSendBusiness.SendConnectionError(clientId, "Error_Happened");
            }
        }
        catch (Exception)
        {
            this.sessionSendBusiness.SendConnectionError(clientId, "Error_Happened");
        }
    }

    public async Task Register(int clientId, int checkClientId, Player player)
    {
        try
        {
            if (clientId == checkClientId)
            {
                player = await this.playerClient.Create(player);
                this.serveur.Clients[clientId].Player = player;
                this.sessionSendBusiness.DisplayPlayerData(clientId, player);
            }
            else
            {
                this.sessionSendBusiness.DisplayPlayerData(clientId, null);
            }
        }
        catch (ErrorException errorException)
        {
            if (errorException != null && errorException.Errors != null && errorException.Errors.Count > 0)
            {
                this.sessionSendBusiness.SendRegisterError(clientId, errorException.Errors.Keys.FirstOrDefault());
            }
            else
            {
                this.sessionSendBusiness.SendRegisterError(clientId, "Error_Happened");
            }
        }
        catch (Exception)
        {
            this.sessionSendBusiness.SendRegisterError(clientId, "Error_Happened");
        }
    }

    public void Welcome(int toClient, string msg)
    {
        this.sessionSendBusiness.Welcome(toClient, msg);
    }
}