using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ITwilioService
    {
        Task SendSMSAsync(string receiver, string content);
    }
}
