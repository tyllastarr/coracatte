namespace CoraCatte
{
    internal class User
    {
        private int userId;
        private string userName;
        private string discordName;

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        public string DiscordName
        {
            get { return discordName; }
            set { discordName = value; }
        }

        public User(int newId, string newName, string newDiscord)
        {
            userId = newId;
            userName = newName;
            discordName = newDiscord;
        }

        public User(int newId, string newName) : this(newId, newName, "")
        {

        }

        public User() : this(0, "", "")
        {

        }
    }
}
