using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using CoreData;
using Utility;
using Utility.Extensions;

namespace AgvcService.Users.Models
{
    public class AuthorizedUser
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        private IPrincipal Principal { get; }
        public AuthorizedUser()
        {
            Principal = Thread.CurrentPrincipal;
        }

        public AuthorizedUser(IPrincipal principal)
        {
            Principal = principal;
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string ClientId => "";//Principal.Identity.GetUserId();

        /// <summary>
        /// 委托方ID
        /// </summary>
        public string ConsignorId => GetString("ConsignorId");
        /// <summary>
        /// 用户名称
        /// </summary>
        public string ClientName => GetString("ClientName");

        public string Mobile => GetString("Mobile");
        public string Phone => GetString("Phone");

        /// <summary>
        /// 船公司角色，限制箱属
        /// </summary>
        public string[] BoxOwnerIds => GetArray("BoxOwners");
        /// <summary>
        /// 机构ID
        /// </summary>
        public string OrgId => GetString("OrgId");
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId => GetString("RoleId");
        /// <summary>
        /// 车队用户，关联车船查的企业账户ID
        /// </summary>
        public string VehicleUserId => GetString("vehicleRelId");
        /// <summary>
        /// 是否系统人员
        /// </summary>
        public bool IsSys
        {
            get
            {
                var value = GetString("IsSys");
                return !string.IsNullOrEmpty(value) && value.ToBool();
            }
        }

        public string[] Menus => GetArray("Menus");
        public string[] Codes => GetArray("Codes");

        private string[] GetArray(string claimType)
        {
            if (Principal == null)
            {
                throw new Exception($"GetArray {claimType} Principal is null");
            }
            var claimsIdentity = Principal.Identity as ClaimsIdentity;
        
            var strings = claimsIdentity?.FindFirst(p => p.Type == claimType)?.Value;
            return !string.IsNullOrEmpty(strings) ? strings.Split(',') : new string[] { };
        }

        private string GetString(string claimType)
        {
            if (Principal == null)
            {
                throw new Exception($"GetString {claimType} Principal is null");
            }
            var claimsIdentity = Principal.Identity as ClaimsIdentity;
            return claimsIdentity?.FindFirst(p => p.Type == claimType)?.Value;
        }

        /// <summary>
        /// 是否拥有指令权限
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool HasAuthorityCode(string code)
        {
            return Codes.Contains(code);
        }
        /// <summary>
        /// 是否拥有菜单权限
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public bool HasAuthorityMenu(string menuId)
        {
            return Menus.Contains(menuId);
        }
        /// <summary>
        /// 是否权限已定义
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool IsAuthDefined(string tag)
        {
            if (tag.StartsWith("zl-"))
            {
                return HasAuthorityCode(tag);
            }

            return HasAuthorityMenu(tag);
        }

        public Identity GetIdentity()
        {
            return new(ClientId, ClientName);
        }
    }
}