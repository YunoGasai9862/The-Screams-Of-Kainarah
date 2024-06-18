public class UsersNode: INode
{
    private string Users { get; set; } = "Users";
    public string GetNode()
    {
        return Users;
    }
}