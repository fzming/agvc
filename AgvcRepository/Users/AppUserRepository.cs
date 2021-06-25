﻿using AgvcEntitys.Users;
using AgvcRepository.Users.Interfaces;
using CoreRepository;

namespace AgvcRepository.Users
{
    public class AppUserRepository : MongoRepository<AppUser>, IAppUserRepository
    {
        protected AppUserRepository(IMongoUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}