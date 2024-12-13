using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.Utilities.Interfaces
{
    public interface IPathCreator
    {
        public string GetRootPath();
        string CombinePaths(string firstPath, string secondPath);
    }
}
