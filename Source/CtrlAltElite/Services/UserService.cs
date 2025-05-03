using CtrlAltElite.Models;
using CtrlAltElite.ServiceProxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;

namespace CtrlAltElite.Services
{
    public class UserService
    {
        private IUserServiceProxy _userServiceProxy;
        public UserService(IUserServiceProxy userServiceProxy)
        {
            _userServiceProxy = userServiceProxy;
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            //this.WalletBalance = walletBalance;
            //this.PointsBalance = pointsBalance;
            //this.UserRole = userRole;
            var result =new List<User>();
            var response = await _userServiceProxy.GetUsersAsync();
            foreach(var user in response.Users)
            {
                var currentUser = new User
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    WalletBalance = user.WalletBalance,
                    PointsBalance = user.PointsBalance,
                    UserRole = (User.Role)user.Role,
                };
                result.Add(currentUser);
            }
            return result;
        }



    }
}
