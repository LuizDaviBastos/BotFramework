using System;
using System.Collections.Generic;
using System.Linq;

namespace BotScheduler.Library.Keys
{
    public class Intents
    {

        public const string UtilitiesConfirm = "Utilities_Confirm";
        public const string UtilitiesReject = "Utilities_Reject";
        public const string ConnectToMeeting = "Calendar_ConnectToMeeting";
        public const string CreateCalendarEntry = "Calendar_CreateCalendarEntry";
        public const string FindCalendarEntry = "Calendar_FindCalendarEntry";

        public const string ChangeCalendarEntry = "Calendar_ChangeCalendarEntry";
        public const string Calendar_Cancel = "Calendar_Cancel";
        public const string ContactMeetingAttendees = "Calendar_ContactMeetingAttendees";
        public const string DeleteCalendarEntry = "Calendar_DeleteCalendarEntry";
        public const string Calendar_Confirm = "Calendar_Confirm";
        public const string Symptoms_Fever = "Symptoms_Fever";

        public const string CreateAnnouncement = "1CRTAPPTMNT";
        public const string GetAppointment = "2GTAPPTMNT";

        public const string Cancel = "@cancel";

        public class AnnouncementIntents
        {
            public const string NewAnnouncement = "Criar anuncio 📝";
            public const string ListAnnouncement = "Listar anuncios 🧾";
            public const string EditAnnouncement = "Excluir anuncio 🗑";
            public const string DeleteAnnouncement = "Editar anuncio ✏";

            public static List<string> GetIntents() 
            {
                var s = new List<string>
                {
                    AnnouncementIntents.NewAnnouncement,
                    AnnouncementIntents.ListAnnouncement,
                    AnnouncementIntents.EditAnnouncement,
                    AnnouncementIntents.DeleteAnnouncement,
                    AnnouncementIntents.EditAnnouncement,
                    AnnouncementIntents.DeleteAnnouncement,
                    AnnouncementIntents.EditAnnouncement,
                    AnnouncementIntents.DeleteAnnouncement,
                    AnnouncementIntents.EditAnnouncement,
                    AnnouncementIntents.DeleteAnnouncement
                };
                return s;
            }

        }
    }
}
