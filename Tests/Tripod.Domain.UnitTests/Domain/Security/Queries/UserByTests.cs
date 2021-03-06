﻿using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using Moq;
using Should;
using Xunit;

namespace Tripod.Domain.Security
{
    public class UserByTests
    {
        #region UserBy Id

        [Fact]
        public void Query_IntCtor_SetsIdProperty()
        {
            var id = FakeData.Id();
            var query = new UserBy(id);
            query.Id.ShouldEqual(id);
            query.Name.ShouldBeNull();
            query.UserLoginInfo.ShouldBeNull();
            query.Principal.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNonNullUser_WhenFound_ById()
        {
            var userId = FakeData.Id();
            var data = new[] { new ProxiedUser(userId) }.AsQueryable();
            var query = new UserBy(userId);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            User result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.ShouldEqual(data.Single());
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUser_WhenNotFound_ById()
        {
            var userId = FakeData.Id();
            var otherUserId = FakeData.Id(userId);
            var data = new[] { new ProxiedUser(otherUserId) }.AsQueryable();
            var query = new UserBy(userId);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            User result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        #endregion
        #region UserBy Name

        [Fact]
        public void Query_StringCtor_SetsNameProperty()
        {
            var name = FakeData.String();
            var query = new UserBy(name);
            query.Name.ShouldEqual(name);
            query.Id.HasValue.ShouldBeFalse();
            query.UserLoginInfo.ShouldBeNull();
            query.Principal.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNonNullUser_WhenFound_ByName()
        {
            var userName = FakeData.String();
            var data = new[] { new User { Name = userName } }.AsQueryable();
            var query = new UserBy(userName);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.ShouldEqual(data.Single());
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUser_WhenNotFound_ByName()
        {
            var userName = FakeData.String();
            var data = new[] { new User { Name = FakeData.String() } }.AsQueryable();
            var query = new UserBy(userName);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        #endregion
        #region UserBy Principal

        [Fact]
        public void Query_PrincipalCtor_SetsPrincipalProperty()
        {
            var principal = new GenericPrincipal(new GenericIdentity("username"), null);
            var query = new UserBy(principal);
            query.Name.ShouldBeNull();
            query.Id.HasValue.ShouldBeFalse();
            query.UserLoginInfo.ShouldBeNull();
            query.Principal.ShouldEqual(principal);
        }

        [Fact]
        public void Handler_ReturnsNonNullUser_WhenAuthenticatedAndFound_ByPrincipal()
        {
            var userId = FakeData.Id();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
            };
            var identity = new ClaimsIdentity(claims, "authenticationType");
            var principal = new GenericPrincipal(identity, null);
            var data = new[] { new ProxiedUser(userId) }.AsQueryable();
            var query = new UserBy(principal);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.ShouldEqual(data.Single());
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUser_WhenByUnauthenticatedPrincipal()
        {
            var userId = FakeData.Id();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
            };
            var identity = new ClaimsIdentity(claims, null);
            var principal = new GenericPrincipal(identity, null);
            var data = new[] { new User { Name = "username", } }.AsQueryable();
            var query = new UserBy(principal);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Loose);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUser_WhenAuthenticatedButNotFound_ByPrincipal()
        {
            var userId = FakeData.Id();
            var otherUserId = FakeData.Id(userId);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(CultureInfo.InvariantCulture)),
            };
            var identity = new ClaimsIdentity(claims, "authenticationType");
            var principal = new GenericPrincipal(identity, null);
            var data = new[] { new ProxiedUser(otherUserId) }.AsQueryable();
            var query = new UserBy(principal);
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        #endregion
        #region UserBy UserLoginInfo

        [Fact]
        public void Query_UserLoginInfoCtor_SetsUserLoginInfoProperty()
        {
            var userLoginInfo = new UserLoginInfo("loginProvider", "providerKey");
            var query = new UserBy(userLoginInfo);
            query.Name.ShouldBeNull();
            query.Id.HasValue.ShouldBeFalse();
            query.UserLoginInfo.ShouldEqual(userLoginInfo);
            query.Principal.ShouldBeNull();
        }

        [Fact]
        public void Handler_ReturnsNonNullUser_WhenFound_ByUserLoginInfo()
        {
            var loginProvider = FakeData.String();
            var providerKey = FakeData.String();
            var remoteMembershipId = new RemoteMembershipId
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey,
            };
            var remoteMembership = new ProxiedRemoteMembership(remoteMembershipId);
            var user = new User();
            user.RemoteMemberships.Add(remoteMembership);
            var data = new[] { user }.AsQueryable();
            var query = new UserBy(new UserLoginInfo(loginProvider, providerKey));
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldNotBeNull();
            result.ShouldEqual(data.Single());
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        [Fact]
        public void Handler_ReturnsNullUser_WhenNotFound_ByUserLoginInfo()
        {
            var loginProvider = FakeData.String();
            var providerKey = FakeData.String();
            var remoteMembershipId = new RemoteMembershipId
            {
                LoginProvider = loginProvider,
                ProviderKey = FakeData.String(),
            };
            var remoteMembership = new ProxiedRemoteMembership(remoteMembershipId);
            var user = new User();
            user.RemoteMemberships.Add(remoteMembership);
            var data = new[] { user }.AsQueryable();
            var query = new UserBy(new UserLoginInfo(loginProvider, providerKey));
            var dbSet = new Mock<DbSet<User>>(MockBehavior.Strict).SetupDataAsync(data);
            var entities = new Mock<IReadEntities>(MockBehavior.Strict);
            var entitySet = new EntitySet<User>(dbSet.Object, entities.Object);
            entities.Setup(x => x.Query<User>()).Returns(entitySet);
            var handler = new HandleUserByQuery(entities.Object);

            var result = handler.Handle(query).Result;

            result.ShouldBeNull();
            entities.Verify(x => x.Query<User>(), Times.Once);
        }

        #endregion
    }
}
