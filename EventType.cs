namespace CoraCatte
{
    internal class EventType
    {
        private int eventTypeId;
        private string eventTypeName;

        public int EventTypeId
        {
            get { return eventTypeId; }
            set { eventTypeId = value; }
        }
        public string EventTypeName
        {
            get { return eventTypeName; }
            set { eventTypeName = value; }
        }

        public EventType(int newId, string newName)
        {
            eventTypeId = newId;
            eventTypeName = newName;
        }

        public EventType() : this(0, "")
        {

        }
    }
}
