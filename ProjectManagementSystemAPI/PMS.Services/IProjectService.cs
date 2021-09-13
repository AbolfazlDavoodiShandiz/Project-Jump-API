using PMS.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PMS.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAll(int userId);
    }
}