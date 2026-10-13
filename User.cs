namespace CoraCatte
{
    internal class User
    {
        private int userId;
        private string userName;
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
        public bool CheckedIn
        {
            get { return checkedIn; }
            set { checkedIn = value; }
        }

        public User(int newId, string newName, bool newCheckin)
        {
            userId = newId;
            userName = newName;
            checkedIn = newCheckin;
        }

        public User(int newId, string newName) : this(newId, newName, false)
        {

        }

        public User() : this(0, "", false)
        {

        }
    }
}
