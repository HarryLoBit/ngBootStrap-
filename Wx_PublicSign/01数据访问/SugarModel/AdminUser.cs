using System;
namespace SugarModel
{
    /// <summary>
    /// 后台登录用户
    /// </summary>
    public partial class AdminUser:BaseEntity
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { set; get; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        public string NickName { get; set; }
    }
}

