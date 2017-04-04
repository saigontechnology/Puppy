using System.Threading.Tasks;

namespace TopCore.SSO.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}