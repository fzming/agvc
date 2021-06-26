using System;
using AgvcCoreData.Users;
using CoreData.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgvcEntitys.Users
{
    /// <summary>
    ///     通用用户模型接口
    /// </summary>
    public abstract class UserAccountBase : OEntity
    {
        /// <summary>
        ///     登录ID
        /// </summary>
        /// <remarks>
        ///     系统唯一 手机号
        /// </remarks>
        public string LoginId { get; set; }

        /// <summary>
        ///     用户昵称
        /// </summary>
        public string Nick { get; set; }

        /// <summary>
        ///     介绍
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        ///     性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        ///     头像地址
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        ///     来源设备
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        ///     最近登录时间
        /// </summary>
        public DateTime? RecentLoginTime { get; set; }

        /// <summary>
        ///     管理角色Id
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string RoleId { get; set; }


        #region 用户找回，重置密码

        /// <summary>
        ///     登录密码
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        ///     首次登陆强制修改密码
        /// </summary>
        public bool NeedChangePassword { get; set; }

        /// <summary>
        ///     实名身份认证
        /// </summary>
        public IdCert Cert { get; set; }

        /// <summary>
        ///     已征得车主授权
        /// </summary>
        public bool Agreement { get; set; }

        /// <summary>
        ///     绑定邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     绑定手机号
        /// </summary>
        [BsonIgnoreIfNull]
        public string Mobile { get; set; }

        /// <summary>
        ///     联系电话
        /// </summary>
        public string ContactPhone { get; set; }

        #endregion
    }
}