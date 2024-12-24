using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Models
{
    public interface IUserService
    {
        Task<User> GetUserByUsernameAndPassword(string username, string password);
        Task AddUser(User user);
    }
}
