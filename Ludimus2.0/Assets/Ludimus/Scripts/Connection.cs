public class Connection
{
    public delegate void HandleInputDel(Data data);

    public int ClientId { get; set; }
    public string Playername { get; set; }
    public System.Net.Sockets.Socket Client { get; set; }
    public PlayerUI PlayerUI { get; set; }

    public ConnectionController.MessageDel HandleInput;
}