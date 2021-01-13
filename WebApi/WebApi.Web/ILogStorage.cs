using WebApi.Web.Models;

namespace WebApi.Web
{
    public interface ILogStorage
    {
        public void Store(LogModel log);
    }
}
