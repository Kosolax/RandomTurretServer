using System.Collections.Generic;
using System.Threading.Tasks;

public class MobClient : BaseClient
{
    public async Task<List<Mob>> List()
    {
        string route = $"{this.BaseRoute}{MobRoute.RoutePrefix}";
        return await this.GetAsync<List<Mob>>(route);
    }
}