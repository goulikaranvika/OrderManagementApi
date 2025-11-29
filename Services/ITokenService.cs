using System.Threading.Tasks;
using OrderManagementApi.Models;

namespace OrderManagementApi.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(ApplicationUser user);
    }
}
