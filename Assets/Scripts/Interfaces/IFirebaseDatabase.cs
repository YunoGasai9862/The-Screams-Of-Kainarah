using Firebase.Database;

public interface IFirebaseDatabase
{
    abstract DatabaseReference GetDatabaseReference();
}