using Owin;

namespace JacksFabric.WebApi.Stateless
{
    public interface IOwinBuilder
    {
        void Configuration(IAppBuilder builder);
    }
}