﻿using ControllersServices.ProductService.Interfaces;
using ControllersServices.Utilities.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllersServices.ProductService
{
    public class ProductPhotoServices : IProductPhotoService
    {
        private readonly IPathCreator _pathCreator;
        private readonly IFileService _fileService;
        public ProductPhotoServices(IPathCreator pathCreator, IFileService fileService)
        {
            _pathCreator = pathCreator;
            _fileService = fileService;
        }

        public async Task AddPhotoAsync(IFormFile photo, string fileName, string imageDirectory)
        {
            string roothPath = _pathCreator.GetRootPath();
            string productPath = _pathCreator.CombinePaths(roothPath, imageDirectory);
            await _fileService.CreateFileAsync(photo, productPath, fileName);

        }
        public async Task DeletePhotoAsync(string photoUrl)
        {
            string rootPath = _pathCreator.GetRootPath();
            string normalizedPhotoUrl = photoUrl.Replace('/', Path.DirectorySeparatorChar);

            string fullPath = _pathCreator.CombinePaths(rootPath, normalizedPhotoUrl.TrimStart(Path.DirectorySeparatorChar));

            await _fileService.DeleteFileAsync(fullPath);
        }
    }
}
