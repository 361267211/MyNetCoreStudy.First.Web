using IdentityServer.EFCore.Entity;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web
{
    public class ResourceOwnerPasswordExt : IResourceOwnerPasswordValidator
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;

        public ResourceOwnerPasswordExt(SignInManager<MyUser> signInManager, UserManager<MyUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            System.Console.WriteLine(111);
            return;
        }
    }
}
