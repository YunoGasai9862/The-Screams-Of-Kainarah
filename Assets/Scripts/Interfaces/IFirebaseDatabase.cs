using Firebase.Database;
using System.Threading.Tasks;

public interface IFirebaseDatabase
{
    abstract DatabaseReference GetDatabaseReference();

    abstract Task CreateEntity(IEntity entity, INode firebaseNode);

    abstract Task UpdateEntity(IEntity entity, INode firebaseNode);

}