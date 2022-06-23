using System.Collections.Generic;
using System.Threading.Tasks;

public class StatClient : BaseClient
{
    public async Task<List<Stat>> List()
    {
        string route = $"{this.BaseRoute}{StatRoute.RoutePrefix}";
        return await this.GetAsync<List<Stat>>(route);
    }
}