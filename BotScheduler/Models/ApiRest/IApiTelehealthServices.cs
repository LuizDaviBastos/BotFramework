using Refit;
using System.Threading.Tasks;

namespace BotScheduler.Models.ApiRest
{
    public interface IApiTelehealthServices
    {
        [Get("/api/Appointments?InviteId={InviteId}")]
        Task<AppointmentResponseApi> GetAppoitmentAsync(string InviteId);

        [Delete("/api/Appointments?Id={id}")]
        Task<AppointmentResponseApi> DeleteAppointment(int id);

        [Get("/api/Appointments/find?phoneNumber={phoneNumber}")]
        Task<AppointmentResponseApi> FindAppointmentAsync(string phoneNumber);

        [Post("/api/Appointments")]
        Task<AppointmentResponseApi> PostAppointmentAsync([Body]BaseAppointment appointment);

        [Put("/api/Appointments")]
        Task<AppointmentResponseApi> PutAppointmentAsync([Body]BaseAppointment appointment);
    }
}
