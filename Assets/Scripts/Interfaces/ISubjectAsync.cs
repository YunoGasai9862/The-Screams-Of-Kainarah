using System.Threading.Tasks;
public interface ISubjectAsync<T>
{
    public Task OnNotifySubject(T Data, params object[] optional);
}

public interface ISubjectAsync
{
    public Task OnNotifySubject(params object[] optional);
}