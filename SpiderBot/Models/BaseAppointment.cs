using System;
using System.Collections.Generic;

namespace BotScheduler.Models
{
    public class BaseAppointment
    {
        public int Id { get; set; }
        public IList<Patient> Patients { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Token { get; set; }
        public string SessionId { get; set; }
        public string GroupId { get; set; }
    }
}
