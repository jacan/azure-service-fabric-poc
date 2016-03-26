using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace JacksFabric.WebApi.Stateless.Controllers
{
    [RoutePrefix("api")]
    public class DefaultController : ApiController
    {
        [Route("default")]
        public IEnumerable<string> Get()
        {
            return new[] { "Value1", "Value2" };
        }

        [Route("default/{id}")]
        public string Get(int id)
        {
            return id.ToString();
        }

        // POST api/values
        [Route("default")]
        public void Post([FromBody]string value)
        {

        }

        // PUT api/values/5
        [Route("values/{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [Route("values/{id}")]
        public void Delete(int id)
        {
        }
    }
}
