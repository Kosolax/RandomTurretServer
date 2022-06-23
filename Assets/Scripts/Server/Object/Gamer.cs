using System.Collections.Generic;

public class Gamer
{
    public Gamer()
    {
    }

    public Gamer(int id)
    {
        this.TowerPrice = 10;
        this.Id = id;
        this.Money = 10000;
        this.Life = 3;
        this.TowersInGame = new Dictionary<int, TowerInGame>();
        this.TowerIndexsAvailableToInstantiate = new List<int>()
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24
        };
        this.IsReady = false;
    }

    public static int MAX_TOWERS { get; set; } = 25;

    public int Id { get; set; }

    public bool IsReady { get; set; }

    public float Life { get; set; }

    public int LobbyCode { get; set; }

    public int Money { get; set; }

    public List<int> TowerIndexsAvailableToInstantiate { get; set; }

    public int TowerPrice { get; set; }

    public Dictionary<int, TowerInGame> TowersInGame { get; set; }
}