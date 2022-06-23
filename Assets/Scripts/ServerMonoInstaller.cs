using System.Net.Http;

using Zenject;

public class ServerMonoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        this.RegisterObject();
        this.RegisterMonoBehaviour();
        this.RegisterServerBusiness();
        this.RegisterSendBusiness();
        this.RegisterBusiness();
        this.RegisterHandleBusiness();
        this.RegisterHttpClient();
    }

    public void RegisterBusiness()
    {
        this.Container.Bind<GameBusiness>().AsTransient();
        this.Container.Bind<GamerBusiness>().AsTransient();
        this.Container.Bind<MobBusiness>().AsTransient();
        this.Container.Bind<SessionBusiness>().AsTransient();
        this.Container.Bind<TowerBusiness>().AsTransient();
    }

    public void RegisterHandleBusiness()
    {
        this.Container.Bind<GameHandleBusiness>().AsTransient();
        this.Container.Bind<SessionHandleBusiness>().AsTransient();
        this.Container.Bind<TowerHandleBusiness>().AsTransient();
    }

    public void RegisterHttpClient()
    {
        this.Container.Bind<HttpClient>().AsSingle();
        this.Container.Bind<PlayerClient>().AsTransient();
        this.Container.Bind<MobClient>().AsTransient();
        this.Container.Bind<WaveClient>().AsTransient();
        this.Container.Bind<StatClient>().AsTransient();
    }

    public void RegisterMonoBehaviour()
    {
        this.Container.Bind<MainManager>().AsSingle();
        this.Container.Bind<ThreadManager>().AsSingle();
    }

    public void RegisterObject()
    {
        this.Container.Bind<Serveur>().AsSingle();
        this.Container.Bind<Packet>().AsTransient();
    }

    public void RegisterSendBusiness()
    {
        this.Container.Bind<GamerSendBusiness>().AsTransient();
        this.Container.Bind<GameSendBusiness>().AsTransient();
        this.Container.Bind<MobSendBusiness>().AsTransient();
        this.Container.Bind<SessionSendBusiness>().AsTransient();
        this.Container.Bind<TowerSendBusiness>().AsTransient();
    }

    public void RegisterServerBusiness()
    {
        this.Container.Bind<TCPBusiness>().AsTransient();
        this.Container.Bind<ClientBusiness>().AsTransient();
        this.Container.Bind<ServerBusiness>().AsSingle();
    }
}