public enum ClientPackets : int
{
    AskingConnection = 1,
    AskingRegister = 2,
    AskForGame = 3,
    CancelAskForGame = 4,
    InvokeTower = 5,
    MergeTower = 6,
    LoadGameDone = 7,
    AskForPrivateGame = 8,
    AskJoinPrivateGame = 9,
}