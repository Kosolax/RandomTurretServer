public class Client
{
    public Client(int clientId)
    {
        this.Id = clientId;
        this.Tcp = new TCP(this.Id);
        this.Player = new Player();
        this.Gamer = new Gamer();
    }

    public Gamer Gamer { get; set; }

    public int Id { get; set; }

    public Player Player { get; set; }

    public TCP Tcp { get; set; }
}