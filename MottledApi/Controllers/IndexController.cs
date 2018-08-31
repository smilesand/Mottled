using Microsoft.AspNet.Identity;
using MottledApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace MottledApi.Controllers
{
    /// <summary>
    /// 首页处理
    /// </summary>
    public class IndexController : BaseController
    {
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <returns></returns>
        public UserInfoViewModel GetUser()
        {
            UserInfoViewModel user = new UserInfoViewModel { Email = "847666418@qq.com", HasRegistered = true, LoginProvider = "123456" };
            return user;
        }
        /// <summary>
        /// 获取用户集合
        /// </summary>
        /// <returns></returns>
        public List<UserInfoViewModel> GetListUser()
        {
            List<UserInfoViewModel> u = new List<UserInfoViewModel>();
            for (int i = 0; i < 10; i++)
            {
                UserInfoViewModel user = new UserInfoViewModel { Email = "847666418@qq.com", HasRegistered = true, LoginProvider = "123456" };
                u.Add(user);
            }
            return u;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> UpdateUser(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool result;
            result = await Task.Run(Update);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }

        private async Task<bool> Update()
        {
            return await Task.Run(() =>
            {
                return false;                               //异步执行完成标记
            });
        }
    }
}