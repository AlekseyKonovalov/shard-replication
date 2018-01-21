using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace ConsoleApplication7
{
    public class SlavesController : ApiController
    {    
        // PUT api/slaves/5 
        public void Put(string id, [FromBody]string value)
        {
            Storage.slavesPorts.Add(id);
        }

    }
}
