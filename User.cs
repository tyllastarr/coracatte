namespace CoraCatte
{
    internal class User
    {
        private int userId;
        private string userName;
        private string discordName;
        private bool checkedIn;

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
        public bool CheckedIn
        {
            get { return checkedIn; }
            set { checkedIn = value; }
        }

        public User(int newId, string newName, string newDiscord, bool newCheckin)
        {
            userId = newId;
            userName = newName;
            discordName = newDiscord;
            checkedIn = newCheckin;
        }

        public User(int newId, string newName, string newDiscord) : this(newId, newName, newDiscord, false)
        {

        }

        public User(int newId, string newName, bool newCheckin) : this(newId, newName, "", newCheckin)
        {

        }

        public User(int newId, string newName) : this(newId, newName, "", false)
        {

        }

        public User() : this(0, "", "", false)
        {

        }
    }
}
