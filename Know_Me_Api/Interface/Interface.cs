using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Know_Me_Api.Interface
{
    public interface IValuesController
    {
        ActionResult<IEnumerable<string>> Get();
        ActionResult<string> Get(int id);

    }
}
