using Assets.Annotations;
using Assets.Scripts.ScenePersistence.Models;
using System.Collections.Generic;

namespace Assets.Scripts.GameState
{
    [Subject(SubjectType = typeof(GameStateHandler), ContextType = typeof(SceneData))]

    public  class GameStateHandler
    {
        private static List<IGameStateHandler> GameStateHandlerObservers { get; set; } = new List<IGameStateHandler>();
    }
}
