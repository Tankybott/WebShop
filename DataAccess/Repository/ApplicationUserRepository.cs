﻿using DataAccess.Repository.IRepository;
using Models;

namespace DataAccess.Repository
{
    public class ApplicationUserRepository: Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(ApplicationDbContext applicationDbContex): base(applicationDbContex) { }
    }
}
