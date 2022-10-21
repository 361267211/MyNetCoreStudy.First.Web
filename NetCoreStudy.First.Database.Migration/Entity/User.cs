
using NetCoreStudy.First.Domain.AggregateRoot;
using NetCoreStudy.First.Domain.ValueObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zack.Commons;

namespace NetCoreStudy.First.Domain.Entity
{
    public record User: IAggregatrRoot
    {
        public string? passwordHash;



        public Guid Id { get; init; }
        public PhoneNumber PhoneNumber { get; private set; }

        public UserAccessFail AccessFail { get; private set; } //相同聚合种猜这样直接引用实体作为属性

        private User()
        {

        }

        /// <summary>
        /// 必须有手机号
        /// </summary>
        /// <param name="phoneNumber"></param>
        private User(PhoneNumber phoneNumber)
        {
            Id=Guid.NewGuid();
            PhoneNumber=phoneNumber;
            this.AccessFail=new UserAccessFail(this);
        }

        public bool HasPassword()
        {
            return !string.IsNullOrEmpty(passwordHash);
        }

        public void ChangePassword(string value)
        {
            if (value.Length<3)
            {
                throw new ArgumentException("密码长度不能小于3");
            }
            passwordHash = HashHelper.ComputeMd5Hash(value);
        }

        public bool CheckPassword(string password)
        {

            return passwordHash == HashHelper.ComputeMd5Hash(password);
        }

        public void ChangePhoneNumber(PhoneNumber phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }
    }
}
