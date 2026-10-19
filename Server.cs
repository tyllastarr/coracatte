namespace CoraCatte
{
    internal class Server
    {
        private int serverId;
        private string serverName;
        public int ServerId
        {
            get { return serverId; }
            set { serverId = value; }
        }
        public string ServerName
        {
            get { return serverName; }
            set { serverName = value; }
        }

        public Server(int newId, string newName)
        {
            serverId = newId;
            serverName = newName;
        }

        public Server() : this(0, "")
        {

        }
    }
}
