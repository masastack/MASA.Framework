// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.


namespace Masa.Framework.EventApiTest.Infrastructure
{
    public class UserRepository : Masa.Contrib.Ddd.Domain.Repository.EFCore.Repository<CustomDbContext, User>, Masa.Framework.EventApiTest.Domain.IUserRepository
    {
        public UserRepository(CustomDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
        }
    }
}
