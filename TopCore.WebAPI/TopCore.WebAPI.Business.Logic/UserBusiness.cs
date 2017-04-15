#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI.Business.Logic </Project>
//     <File>
//         <Name> UserBusiness </Name>
//         <Created> 15/04/2017 04:54:49 PM </Created>
//         <Key> 4b69ba98-2795-426d-a905-fd58b010c921 </Key>
//     </File>
//     <Summary>
//         UserBusiness
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.WebAPI.Data;
using TopCore.WebAPI.Data.Entity;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.WebAPI.Business.Logic
{
    [PerRequestDependency(ServiceType = typeof(IUserBusiness))]
    public class UserBusiness : IUserBusiness
    {
        private readonly IRepository<User> _useRepository;

        public UserBusiness(IRepository<User> useRepository)
        {
            _useRepository = useRepository;
        }

        public void Add(string userName)
        {
            User user = new User(0)
            {
                UserName = userName
            };

            _useRepository.Add(user);
        }
    }
}