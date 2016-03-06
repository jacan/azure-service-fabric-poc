using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Owin;

namespace JacksFabric.WebApi.Stateless
{
    public class Startup : IOwinBuilder
    {
        public void Configuration(IAppBuilder builder)
        {
            var httpConfig = new HttpConfiguration();
            httpConfig.MapHttpAttributeRoutes();
            
            FormatterConfig.ConfigureFormatters(httpConfig.Formatters);

            builder.UseWebApi(httpConfig);
        }
    }
}
