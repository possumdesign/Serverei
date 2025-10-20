namespace ServerRackSimulator
{
    internal enum Role { Admin = 0, User = 1 }

    internal class User
    {
        public string Name;
        public Role Role;
        public User() { }
        public User(string name, Role role) { Name = name; Role = role; }
    }
}