using Firebase.Database;
using System.Threading.Tasks;

public interface IFirebaseDatabase
{
    abstract DatabaseReference GetDatabaseReference();
    abstract Task Create(IEntity entity, INode firebaseNode);
}