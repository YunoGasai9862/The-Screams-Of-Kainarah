using System.Collections.Generic;
using System.Threading.Tasks;
public interface IDelegateExecutor
{
     Task ExecuteDelegates(List<UnityEngine.Object> preloadedEntities);
}