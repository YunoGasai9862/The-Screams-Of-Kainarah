using System;
using System.Threading.Tasks;
public interface IPreloadWithGenericAction
{
    public Task ExecuteGenericAction(Action action);
}