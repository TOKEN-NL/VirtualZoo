using System.Threading.Tasks;

namespace VirtualZooAPI.Services.Interfaces
{
    public interface IZooService
    {
        Task<List<string>> AutoAssignAsync(bool resetExisting);
    }
}
