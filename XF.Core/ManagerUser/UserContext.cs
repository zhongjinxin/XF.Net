using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XF.Core.CacheManager;
using XF.Core.DBManager;
using XF.Core.Extensions;
using XF.Core.Extensions.AutofacManager;
using XF.Entity.DomainModels;

namespace XF.Core.ManagerUser
{
    public class UserContext
    {
        public static UserContext Current
        {
            get
            {
                return Context.RequestServices.GetService(typeof(UserContext)) as UserContext;
            }
        }

        private static Microsoft.AspNetCore.Http.HttpContext Context
        {
            get { return Utilities.HttpContext.Current; }
        }
    
        private static ICacheService CacheService
        {
            get { return GetService<ICacheService>(); }

        }
        private static T GetService<T>() where T : class
        {
            return AutofacContainerModule.GetService<T>();
        }

        public int UserId
        {
            get
            {
                return (Context.User.FindFirstValue(JwtRegisteredClaimNames.Jti)
                    ?? Context.User.FindFirstValue(ClaimTypes.NameIdentifier)).GetInt();
            }
        }

        private UserInfo _userInfo { get; set; }
        public UserInfo UserInfo
        {
            get
            {
                if (_userInfo != null)
                {
                    return _userInfo;
                }
                return GetUserInfo(UserId);
            }
        }
        public UserInfo GetUserInfo(int userId)
        {
            if (_userInfo != null) return _userInfo;
            if (userId <= 0)
            {
                _userInfo = new UserInfo();
                return _userInfo;
            }
            string key = userId.GetUserIdKey();
            _userInfo = CacheService.Get<UserInfo>(key);
            if (_userInfo != null && _userInfo.User_Id > 0) return _userInfo;

            //_userInfo = DBServerProvider.DbContext.Set<Sys_User>()
            //    .Where(x => x.User_Id == userId).Select(s => new UserInfo()
            //    {
            //        User_Id = userId,
            //        Role_Id = s.Role_Id.GetInt(),
            //        RoleName = s.RoleName,
            //        Token = s.Token,
            //        UserName = s.UserName,
            //        UserTrueName = s.UserTrueName,
            //        Enable = s.Enable
            //    }).FirstOrDefault();

            //if (_userInfo != null && _userInfo.User_Id > 0)
            //{
            //    CacheService.AddObject(key, _userInfo);
            //}
            return _userInfo ?? new UserInfo();
        }
    }
}
