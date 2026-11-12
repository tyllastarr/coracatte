namespace CoraCatte
{
    internal class Server
    {
        private int serverId;
        private string serverName;
        private string twitchId;
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
        public string TwitchId
        {
            get { return twitchId; }
            set { twitchId = value; }
        }

        public Server(int newId, string newName, string newTwitch)
        {
            serverId = newId;
            serverName = newName;
            twitchId = newTwitch;
        }

        public Server(int newId, string newName) : this(newId, newName, "")
        {

        }

        public Server() : this(0, "", "")
        {

        }
    }
}
