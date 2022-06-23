using System.Threading.Tasks;

using Newtonsoft.Json;

public class PlayerClient : BaseClient
{
    public async Task<Player> Connection(Player player)
    {
        string route = $"{this.BaseRoute}{PlayerRoute.RoutePrefix}{PlayerRoute.Connection}";
        return await this.PostAsync<Player>(JsonConvert.SerializeObject(player), route);
    }

    public async Task<Player> Create(Player player)
    {
        string route = $"{this.BaseRoute}{PlayerRoute.RoutePrefix}";
        player = await this.PostAsync<Player>(JsonConvert.SerializeObject(player), route);
        return player;
    }

    public async Task<Player> UpdateElo(Player player)
    {
        string route = $"{this.BaseRoute}{PlayerRoute.RoutePrefix}/{PlayerRoute.UpdateElo}/{player.Id}";
        player = await this.PutAsync<Player>(JsonConvert.SerializeObject(player), route);
        return player;
    }
}