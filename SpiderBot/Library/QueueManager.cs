using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BotScheduler.Library
{
    public class QueueManager
    {
        public async static Task AddMessageAsync(object obj)
        {
            const string connectionString = @"DefaultEndpointsProtocol=https;AccountName=easychatbot;AccountKey=hkDpJpqDZQKsHjSi0l6v9z1bgnAGswv9b3Win0c12PXhWVuuEFR9ONEFHgJUTx+I6vyMp1YGiBsn36p0bOyteg==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("queueappointmentchatbot");

            await queue.CreateIfNotExistsAsync();

            var data = JsonConvert.SerializeObject(obj);

            var msg = new CloudQueueMessage(data);

            await queue.AddMessageAsync(msg);
        }
    }
}
