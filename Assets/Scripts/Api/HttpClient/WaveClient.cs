using System.Collections.Generic;
using System.Threading.Tasks;

public class WaveClient : BaseClient
{
    public async Task<List<Wave>> List()
    {
        string route = $"{this.BaseRoute}{WaveRoute.RoutePrefix}";
        return await this.GetAsync<List<Wave>>(route);
    }
}