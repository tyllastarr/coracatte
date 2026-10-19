namespace CoraCatte
{
    internal class UserServer
    {
        private int userId;
        private int serverId;

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        public int ServerId
        {
            get { return serverId; }
            set { serverId = value; }
        }

        public UserServer(int newUser, int newServer)
        {
            userId = newUser;
            serverId = newServer;
        }

        public UserServer() : this(0, 0)
        {

        }
    }
}
