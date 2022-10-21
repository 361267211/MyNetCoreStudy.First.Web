using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Domain.Entity
{
    public class UserAccessFail
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public User User { get; set; }

        private bool lockOut;//是否锁定

        public DateTime? LockoutEnd { get; private set; }

        public int AccessFailCount { get; private set; }

        private UserAccessFail() { }
        public UserAccessFail(User user)
        {
            Id = Guid.NewGuid();
            User = user;    
        }

        public void Reset()
        {
            lockOut = false;
            LockoutEnd=null;
            AccessFailCount=0;
        }

        public void Fail()
        {
            AccessFailCount ++;
            if (AccessFailCount>=3)
            {
                lockOut = true;
                LockoutEnd=DateTime.Now.AddMinutes(5);
            }
        }

        public bool IsLockOut()
        {
            if (lockOut)
            {
                if (LockoutEnd>=DateTime.Now)
                {
                    return true;
                }
                else
                {
                    AccessFailCount = 0;
                    LockoutEnd = null;
                    lockOut=false;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
