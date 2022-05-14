
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Recognizer
{
    public class LuisRecognizerQuery : IRecognizer
    {

        private readonly LuisRecognizer _luisRecognizer;

        public LuisRecognizerQuery(IConfiguration configuration)
        {
            var luisConfigured = (!string.IsNullOrEmpty(configuration["LuisAppId"]) &&
                !string.IsNullOrEmpty(configuration["LuisAPIKey"]) &&
                !string.IsNullOrEmpty(configuration["LuisAPIHostName"]));


            if (luisConfigured)
            {
                var luisApplication = new LuisApplication()
                {
                    ApplicationId = configuration["LuisAppId"],
                    EndpointKey = configuration["LuisAPIKey"],
                    Endpoint = configuration["LuisAPIHostName"]
                };

                _luisRecognizer = new LuisRecognizer(luisApplication);
            }
        }

        public Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            return this._luisRecognizer.RecognizeAsync(turnContext, cancellationToken);
        }

        public Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken) where T : IRecognizerConvert, new()
        {
            return this._luisRecognizer.RecognizeAsync<T>(turnContext, cancellationToken);
        }

        public virtual bool IsConfigured => this._luisRecognizer != null;
    }
}
