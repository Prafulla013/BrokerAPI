using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IEventDispatcherService
    {
        void QueueNotification(INotification notification);
        void ClearQueue();
        Task Dispatch(CancellationToken cancellationToken);
    }
}
