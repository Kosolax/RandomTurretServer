using System.Collections.Generic;

using UnityEngine;

public interface IAim
{
    List<GameObject> SelectMobs(List<GameObject> mobsInstantiated);
}