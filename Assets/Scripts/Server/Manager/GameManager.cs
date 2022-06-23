using System.Collections.Generic;

using UnityEngine;

using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject]
    public readonly GameBusiness GameBusiness;

    [Inject]
    public readonly MobBusiness MobBusiness;

    public GameObject GamePrefab;

    public Transform GridLayoutTransform;

    public List<Color> MobsColor;

    public List<Color> TowersColor;

    public static GameManager Instance { get; set; }

    public void SetPlayersId(int currentId, int index, List<GamerGameManager> playerGameManagers)
    {
        playerGameManagers[index].Id = currentId;
        playerGameManagers[index].MobBusiness = this.MobBusiness;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}