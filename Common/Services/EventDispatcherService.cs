using Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Services
{
    public class EventDispatcherService : IEventDispatcherService
    {
        private readonly IMediator _mediator;
        private readonly List<INotification> _notificationQueue = new List<INotification>();
        public EventDispatcherService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void ClearQueue()
        {
            _notificationQueue.Clear();
        }

        public async Task Dispatch(CancellationToken cancellationToken)
        {
            while (_notificationQueue.Count > 0)
            {
                try
                {
                    await _mediator.Publish(_notificationQueue[0], cancellationToken);
                    //break;
                }
                catch (Exception e)
                {
                    // todo : let each notification define what to do on failure
                    Console.WriteLine(e);
                }
                finally
                {
                    _notificationQueue.RemoveAt(0);
                }
            }
        }

        public void QueueNotification(INotification notification)
        {
            _notificationQueue.Add(notification);
        }
    }
}
