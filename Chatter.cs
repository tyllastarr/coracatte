namespace CoraCatte
{
    internal class Chatter
    {
        private int chatterId;
        private int userId;

        public int ChatterId
        {
            get { return chatterId; }
            set { chatterId = value; }
        }
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public Chatter(int newChatter, int newUser)
        {
            chatterId = newChatter;
            userId = newUser;
        }

        public Chatter() : this(0, 0)
        {

        }
    }
}
