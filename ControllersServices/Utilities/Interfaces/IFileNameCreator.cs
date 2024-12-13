using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.Utilities.Interfaces
{
    public interface IFileNameCreator
    {
        public string CreateFileName(IFormFile file);
    }
}
