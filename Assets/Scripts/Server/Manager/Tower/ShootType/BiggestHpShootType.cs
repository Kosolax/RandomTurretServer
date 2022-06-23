using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class BiggestHpShootType : IAim
{
    public List<GameObject> SelectMobs(List<GameObject> mobsInstantiated)
    {
        return new List<GameObject>()
        {
            mobsInstantiated.OrderByDescending(x => x.GetComponent<MobInGame>().Life).FirstOrDefault(),
        };
    }
}