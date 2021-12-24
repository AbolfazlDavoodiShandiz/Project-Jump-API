using PMS.Common.Enums;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.Services
{
    public interface INotificationService
    {
        Task Create(int recieverUserId, NotificationType notificationType, string relatedObjectTitle, int relatedObjectId, CancellationToken cancellationToken);
        Task<IEnumerable<Notification>> GetAll(int userId, CancellationToken cancellationToken, bool justUnread = true);
        Task MarkAsRead(IEnumerable<int> idList, CancellationToken cancellationToken);
    }
}
