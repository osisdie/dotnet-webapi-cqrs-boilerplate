using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TestAbstractions.App_Start;
using Xunit;

namespace UnitTest.CoreFX.Tests
{
    public class Mapper_Test : UnitTestBase
    {
        [Fact]
        public void TypeCovert_Test()
        {
            var cfg = new MapperConfigurationExpression();
            cfg.AddProfile<TesMapperProfile>();
            var configuration = new MapperConfiguration(cfg, NullLoggerFactory.Instance);
            var mapper = configuration.CreateMapper();

            var user1 = new TestUserEntityFrom
            {
                Id = "Fake user id",
                Name = "Fake user name"
            };
            var user2 = mapper.Map<TestUserEntityTo>(user1);

            Assert.NotNull(user2);
            Assert.Equal(user1.Id, user2.UserId);
            Assert.Equal(user1.Name, user2.UserName);
        }
    }

    public class TestUserEntityFrom
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class TestUserEntityTo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? Exp { get; set; }
        public string _id { get; set; } = Guid.NewGuid().ToString();
    }

    public class TesMapperProfile : AutoMapper.Profile
    {
        public TesMapperProfile()
        {
            CreateMap<TestUserEntityFrom, TestUserEntityTo>()
                .ForMember(des => des.UserId, opt => { opt.MapFrom(map => map.Id); })
                .ForMember(des => des.UserName, opt => { opt.MapFrom(map => map.Name); })
                .ReverseMap();
        }
    }
}
