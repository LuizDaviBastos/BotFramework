using System;

namespace BotScheduler.Library
{
    public static class ExtensionMethods
    {
        public static DateTime ToDateTime(this string date)
        {
            try
            {
                if (string.IsNullOrEmpty(date)) return new DateTime();
                var splited = date.Split('/');
                var month = splited[0];
                var dayhour = splited[1].Split(' ');
                var day = dayhour[0];
                var hourMinutes = dayhour[1].Split(':');

                return new DateTime(DateTime.Now.Year, int.Parse(month), int.Parse(day), int.Parse(hourMinutes[0]), int.Parse(hourMinutes[1]), 0);

            }
            catch (Exception)
            {
                return new DateTime();
            }
        }
    }
}
