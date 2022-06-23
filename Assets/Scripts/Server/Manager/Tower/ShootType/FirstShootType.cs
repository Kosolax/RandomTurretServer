using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class FirstShootType : IAim
{
    public List<GameObject> SelectMobs(List<GameObject> mobsInstantiated)
    {
        int index = mobsInstantiated.OrderByDescending(x => x.GetComponent<MobInGame>().MobMoveManager.Index).Select(x => x.GetComponent<MobInGame>().MobMoveManager.Index).FirstOrDefault();
        List<GameObject> mobs = mobsInstantiated.Where(x => x.GetComponent<MobInGame>().MobMoveManager.Index == index).ToList();

        // We sort mobs depending on which index they are because it's the only solution to find which one is the first
        if (index >= 0 && index <= 4)
        {
            mobs.Sort((x, y) =>
            {
                if (x.transform.position.y < y.transform.position.y)
                {
                    return -1;
                }
                else if (x.transform.position.y < y.transform.position.y)
                {
                    return 1;
                }

                return 0;
            });
        }
        else if (index >= 6 && index <= 7)
        {
            mobs.Sort((x, y) =>
            {
                if (x.transform.position.x > y.transform.position.x)
                {
                    return -1;
                }
                else if (x.transform.position.x < y.transform.position.x)
                {
                    return 1;
                }

                return 0;
            });
        }
        else if (index >= 9 && index <= 12)
        {
            mobs.Sort((x, y) =>
            {
                if (x.transform.position.y > y.transform.position.y)
                {
                    return -1;
                }
                else if (x.transform.position.y < y.transform.position.y)
                {
                    return 1;
                }

                return 0;
            });
        }
        else if (index >= 14 && index <= 16)
        {
            mobs.Sort((x, y) =>
            {
                if (x.transform.position.x > y.transform.position.x)
                {
                    return -1;
                }
                else if (x.transform.position.x < y.transform.position.x)
                {
                    return 1;
                }

                return 0;
            });
        }
        else if (index >= 18 && index <= 22)
        {
            mobs.Sort((x, y) =>
            {
                if (x.transform.position.y < y.transform.position.y)
                {
                    return -1;
                }
                else if (x.transform.position.y > y.transform.position.y)
                {
                    return 1;
                }

                return 0;
            });
        }
        else if (index == 5 || index == 17)
        {
            mobs.Sort((x, y) =>
            {
                if (Math.Round(x.transform.position.x, 2) > Math.Round(y.transform.position.x, 2) || Math.Round(x.transform.position.y, 2) < Math.Round(y.transform.position.y, 2))
                {
                    return -1;
                }
                else if (Math.Round(x.transform.position.x, 2) < Math.Round(y.transform.position.x, 2) || Math.Round(x.transform.position.y, 2) > Math.Round(y.transform.position.y, 2))
                {
                    return 1;
                }

                return 0;
            });
        }
        else if (index == 8 || index == 13)
        {
            mobs.Sort((x, y) =>
            {
                if (Math.Round(x.transform.position.x, 2) > Math.Round(y.transform.position.x, 2) || Math.Round(x.transform.position.y, 2) > Math.Round(y.transform.position.y, 2))
                {
                    return -1;
                }
                else if (Math.Round(x.transform.position.x, 2) < Math.Round(y.transform.position.x, 2) || Math.Round(x.transform.position.y, 2) < Math.Round(y.transform.position.y, 2))
                {
                    return 1;
                }

                return 0;
            });
        }

        if (mobs.Count > 0)
        {
            return new List<GameObject>()
            {
                mobs[0],
            };
        }

        return new List<GameObject>();
    }
}