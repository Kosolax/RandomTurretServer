using System.Collections.Generic;

public class Player : BaseBusinessObject
{
    public int Elo { get; set; }

    public int Gold { get; set; }

    public int Id { get; set; }

    public string Mail { get; set; }

    public string Password { get; set; }

    public string Pseudo { get; set; }

    public List<Tower> Towers { get; set; }
}