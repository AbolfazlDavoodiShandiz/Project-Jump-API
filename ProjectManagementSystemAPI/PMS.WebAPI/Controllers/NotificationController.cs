using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.Common.Utility;
using PMS.DTO;
using PMS.Services;
using PMS.WebFramework.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.WebAPI.Controllers
{
    [Route("api/notification/[action]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public NotificationController(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        [HttpGet]
        [ActionName("GetUnreadNotifications")]
        public async Task<ApiResult<IEnumerable<NotificationDTO>>> GetUnreadNotifications(CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();

            var notifications = await _notificationService.GetAll(userId, cancellationToken);

            var mappedNotifications = _mapper.Map<IEnumerable<NotificationDTO>>(notifications);

            return Ok(mappedNotifications);
        }
    }
}
