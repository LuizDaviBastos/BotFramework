using System.Collections.Generic;

namespace BotScheduler.Models
{
    public class AppointmentResponseApi
    {
        public Appointment Item { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public IList<string> AdditionalMessages { get; set; }
        public object ApiVersion { get; set; }

    }
}
