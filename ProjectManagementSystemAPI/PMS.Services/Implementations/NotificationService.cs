using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PMS.Common.Enums;
using PMS.Common.Utility;
using PMS.Data;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(IRepository<Notification> notificationRepository, IHttpContextAccessor httpContextAccessor)
        {
            _notificationRepository = notificationRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Create(int recieverUserId, NotificationType notificationType, string relatedObjectTitle, int relatedObjectId, CancellationToken cancellationToken)
        {
            var currentUser = _httpContextAccessor.HttpContext.User.Identity;
            var currentUserId = currentUser.GetUserId();
            var currentUsername = currentUser.Name;

            var notification = new Notification()
            {
                CreatedUserId = currentUserId,
                CreatedUsername = currentUsername,
                RecieverUserId = recieverUserId,
                NotificationType = notificationType,
                RelatedObjectTitle = relatedObjectTitle,
                RelatedObjectId = relatedObjectId
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);
        }

        public async Task<IEnumerable<Notification>> GetAll(int userId, CancellationToken cancellationToken, bool justUnread = true)
        {
            if (justUnread)
            {
                return await _notificationRepository
                .TableNoTracking
                .Where(n => n.RecieverUserId == userId && n.IsRead == false)
                .ToListAsync(cancellationToken);
            }
            else
            {
                return await _notificationRepository
                .TableNoTracking
                .Where(n => n.RecieverUserId == userId)
                .ToListAsync(cancellationToken);
            }
        }

        public async Task MarkAsRead(IEnumerable<int> idList, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository
                .TableNoTracking
                .Where(n => idList.Contains(n.Id))
                .ToListAsync(cancellationToken);

            notifications.ForEach(n => n.IsRead=true);

            await _notificationRepository
                .UpdateRangeAsync(notifications, cancellationToken);
        }
    }
}
