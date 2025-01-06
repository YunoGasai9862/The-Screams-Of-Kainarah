//the solution i came to my mind - the better one
//as soon as the object enables it, instead of using these subjects,
//the object will call the delegator (the main listener), which will ping the actual object,
//the actual object can return/invoke anything via the the same delegator!!
//no need to use subjects - just on the onenable/ondisable, call it
//we dont want to continuously invoke that for all the observers, do we? 

//or we are using the subjects, then only ping back that particular subject, instead of all of them!!!

using System.Threading.Tasks;

public class SceneSingletonDelegator : IDelegator
{
    public Task NotifyListenerAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task NotifySubjectAsync()
    {
        throw new System.NotImplementedException();
    }
}