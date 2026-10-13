namespace CoraCatte
{
    internal class Event
    {
        private int eventId;
        private int eventTypeId;
        private int userId;
        private DateTimeOffset eventTime;
        
        public int EventId
        {
            get { return eventId; }
            set { eventId = value; }
        }
        public int EventTypeId
        {
            get { return eventTypeId; }
            set { eventTypeId = value; }
        }
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        public DateTimeOffset EventTime
        {
            get { return eventTime; }
            set { eventTime = value; }
        }

        public Event(int newEvent, int newType, int newUser, DateTimeOffset newTime)
        {
            eventId = newEvent;
            eventTypeId = newType;
            userId = newUser;
            eventTime = newTime;
        }

        public Event(int newEvent, int newType, int newUser) : this(newEvent, newType, newUser, DateTimeOffset.UtcNow)
        {

        }

        public Event() : this(0, 0, 0, DateTimeOffset.UtcNow)
        {

        }
    }
}
