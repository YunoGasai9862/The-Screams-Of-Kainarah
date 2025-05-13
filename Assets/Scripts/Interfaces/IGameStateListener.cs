using System.Threading.Tasks;

public interface IGameStateListener
{
    public Task Ping(GameState gameState);
}