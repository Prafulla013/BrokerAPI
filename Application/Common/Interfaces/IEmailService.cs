using Application.Common.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailModel email, CancellationToken cancellationToken);
    }
}
