namespace BotScheduler.Models
{
    public class Appointment : BaseAppointment
    {

        public string MsCommunicationsToken
        {
            set { base.Token = value; }
            get { return base.Token; }
        }
        public string MsCommunicationsSessionId
        {
            set { base.SessionId = value; }
            get { return base.SessionId; }
        }
        public string MsCommunicationsGroupId
        {
            set { base.GroupId = value; }
            get { return base.GroupId; }
        }

    }
}
