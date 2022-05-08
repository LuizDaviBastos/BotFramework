using BotScheduler.Models;
using BotScheduler.Models.ApiRest;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Refit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BotScheduler.API
{
    public class TelehealthApiService
    {
        private readonly IConfiguration _configuration;
        private readonly string _host = string.Empty;

        public TelehealthApiService(IConfiguration configuration)
        {
            this._configuration = configuration;
            this._host = _configuration["ApiHost"];
        }

        public async Task<AppointmentResponseApi> GetAppointmentAsync(string inviteId)
        {
            try
            {
                return await RestService.For<IApiTelehealthServices>(this._host).GetAppoitmentAsync(inviteId);
            }
            catch (System.Exception ex)
            {
                return new AppointmentResponseApi()
                {
                    Message = ex.Message,
                    Success = false
                };
            }

        }

        public async Task<AppointmentResponseApi> FindAppointmentAsync(string phoneNumber)
        {
            try
            {
                return await RestService.For<IApiTelehealthServices>(this._host).FindAppointmentAsync(phoneNumber);

            }
            catch (System.Exception ex)
            {
                return new AppointmentResponseApi()
                {
                    Message = ex.Message,
                    Success = false
                };
            }

        }

        public async Task<AppointmentResponseApi> PostAppointmentAsync(BaseAppointment appointment)
        {
            try
            {
                return await RestService.For<IApiTelehealthServices>(this._host).PostAppointmentAsync(appointment);
            }
            catch (System.Exception ex)
            {
                return new AppointmentResponseApi()
                {
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        public async Task<AppointmentResponseApi> PutAppointmentAsync(BaseAppointment appointment)
        {
            try
            {
                return await RestService.For<IApiTelehealthServices>(this._host).PutAppointmentAsync(appointment);
            }
            catch (System.Exception ex)
            {
                return new AppointmentResponseApi()
                {
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        public async Task<List<string>> GetTimes()
        {
            var paths = new string[] { ".", "API", "times.json" };
            if (File.Exists(Path.Combine(paths)))
            {
                var json = await File.ReadAllTextAsync(Path.Combine(paths));
                var obj = JsonConvert.DeserializeObject<List<string>>(json);
                return obj;
            }
            else return new List<string>() { "01/12 13:00", "01/13 14:30" };
        } 
    }
}
