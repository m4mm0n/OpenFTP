using System;

namespace Core.FTP.Helpers
{
    public enum ActionEventType : int
    {
        Success = 0,
        Warning = 1,
        Failure = 2,
        Error = 3,
        CriticalError = 4,
        None = -1
    }

    public class ActionEventArgs : EventArgs
    {
        public ActionReply RecievedReply { get; } = null;
        public ActionEventType ActionType { get; }
        public string Message { get; }
        public float CurrentPercent => (_max > _current && _current != -1) ? ((float) (_current / _max) * 100.0f) : -1;
        public string CurrentShortDateTime { get; }
        public string CurrentLongDateTime { get; }

        private long _max = -1;
        private long _current = -1;

        public ActionEventArgs(ActionEventType actionType, ActionReply reply) : this(actionType, reply, null, -1, -1)
        { }
        public ActionEventArgs(ActionEventType actionType, string message) : this(actionType, null, message, -1, -1)
        { }
        public ActionEventArgs(ActionReply reply) : this(ActionEventType.Success, reply, null, -1, -1)
        { }
        public ActionEventArgs(string message):this(ActionEventType.Success, null, message, -1,-1)
        { }
        public ActionEventArgs(long maxProgress, long curProgress) : this(ActionEventType.Success, null, null, maxProgress, curProgress)
        { }
        public ActionEventArgs(ActionEventType actionType, long maxProgress, long curProgress) : this(actionType, null, null, maxProgress, curProgress)
        { }
        public ActionEventArgs(string message, long maxProgress, long curProgress) : this(ActionEventType.Success, null, message, maxProgress, curProgress)
        { }
        public ActionEventArgs(ActionEventType actionType, string message, long maxProgress, long curProgress) : this(actionType, null, message, maxProgress, curProgress)
        { }
        public ActionEventArgs(ActionEventType actionType, ActionReply reply, string message, long maxPercent, long currentPercent)
        {
            ActionType = actionType;
            RecievedReply = reply;
            Message = message;
            _max = maxPercent;
            _current = currentPercent;

            var dt = DateTime.Now;
            CurrentShortDateTime = dt.ToShortTimeString();
            CurrentLongDateTime = string.Format("{0}/{1}/{2} - {3}", dt.Day.ToString("00"), dt.Month.ToString("00"),
                dt.Year.ToString("00"), CurrentShortDateTime);
        }
    }
}
