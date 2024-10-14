using System.Threading.Tasks;
public interface IDelegateExecutor
{
    public Task ExecuteDelegateMethod(IDelegate delegateMethod);
}