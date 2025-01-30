﻿using DataAccess.Repository.IRepository;
using Models;


namespace DataAccess.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
    }
}
