using BotScheduler.API;
using BotScheduler.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace BotScheduler.Library
{
    public class Feature
    {
        public static string GetUrlChat(int appointmentId, IConfiguration configuration)
        {
            var url = configuration["UrlChat"];
            url += $"?appointmentId={appointmentId}";

            return url;
        }

        public static async Task<AppointmentResponseApi> CreateSessionIdAndGroupId(AppointmentResponseApi appointment, TelehealthApiService telehealthApiService)
        {
            try
            {
                if ((appointment?.Success ?? false))
                {
                    var groupIdIsNull = string.IsNullOrEmpty(appointment.Item.MsCommunicationsGroupId);
                    var sessionIdIsNull = string.IsNullOrEmpty(appointment.Item.MsCommunicationsSessionId);

                    if (groupIdIsNull || sessionIdIsNull)
                    {
                        if (sessionIdIsNull) appointment.Item.SessionId = Guid.NewGuid().ToString();
                        if (groupIdIsNull) appointment.Item.GroupId = Guid.NewGuid().ToString();

                        return await telehealthApiService.PutAppointmentAsync(appointment.Item);
                    }
                }
                return appointment;
            }
            catch (Exception ex)
            {
                if ((appointment?.Success ?? false)) 
                {
                    appointment.Message = "error creating threadId or groupId";
                    return appointment; 
                }
                else return new AppointmentResponseApi()
                {
                    Message = ex.Message,
                    Success = false
                };
            }
           

        }
    }
}
