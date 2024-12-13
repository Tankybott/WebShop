using ControllersServices.Utilities.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Models;
using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.Utilities
{
    public class PathCreator : IPathCreator
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string wwwRootPath;
        public PathCreator(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            wwwRootPath = _webHostEnvironment.WebRootPath;
        }

        public string GetRootPath()
        {
            return wwwRootPath;
        }

        public string CombinePaths(string firstPath, string secondPath) 
        {
            return Path.Combine(firstPath, secondPath);
        }
    }
}
