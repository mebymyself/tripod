﻿using System;
using System.Threading.Tasks;

namespace Tripod.Domain.Security
{
    public class UserBy : IDefineQuery<Task<User>>
    {
        public UserBy(int id) { Id = id; }
        public UserBy(string name) { Name = name; }

        public string Name { get; private set; }
        public int? Id { get; private set; }
    }

    public class HandleUserByQuery : IHandleQuery<UserBy, Task<User>>
    {
        private readonly IQueryEntities _entities;

        public HandleUserByQuery(IQueryEntities entities)
        {
            _entities = entities;
        }

        public async Task<User> Handle(UserBy query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var queryable = _entities.Query<User>();
            var entity = query.Id.HasValue
                ? queryable.SingleOrDefaultAsync(x => x.Id == query.Id.Value)
                : queryable.SingleOrDefaultAsync(x => x.Name.Equals(query.Name));

            return await entity;
        }
    }
}
