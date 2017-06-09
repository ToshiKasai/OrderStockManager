using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using OrderStockManager.Models;
using OrderStockManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OrderStockManager.Infrastructure
{
    public class ApplicationUserManager : UserManager<UserModel, int>
    {
        public ApplicationUserManager(IUserStore<UserModel, int> store) : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<DataContext>();
            var appUserManager = new ApplicationUserManager(new UserStore(appDbContext));

            appUserManager.EmailService = new EmailService();

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                appUserManager.UserTokenProvider = new DataProtectorTokenProvider<UserModel, int>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    TokenLifespan = TimeSpan.FromHours(6)
                };
            }

            // ユーザーポリシー(英数字のみでユニークなメールアドレス)
            appUserManager.UserValidator = new UserValidator<UserModel, int>(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true,
            };

            // パスワードポリシー(６桁以上、大文字/小文字/数字が含まれること)
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                // RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            return appUserManager;
        }

#if false
        /// <summary>
        /// ユーザーカスタムバリデーションの実装の場合
        /// ドメインを特定する処理を記述
        /// </summary>
        public class MyCustomUserValidator : UserValidator<UserModel, int>
        {

            List<string> _allowedEmailDomains = new List<string> { "outlook.com", "hotmail.com", "gmail.com", "yahoo.com" };

            public MyCustomUserValidator(ApplicationUserManager appUserManager)
                : base(appUserManager)
            {
            }

            public override async Task<IdentityResult> ValidateAsync(UserModel user)
            {
                IdentityResult result = await base.ValidateAsync(user);

                var emailDomain = user.Email.Split('@')[1];

                if (!_allowedEmailDomains.Contains(emailDomain.ToLower()))
                {
                    var errors = result.Errors.ToList();

                    errors.Add(String.Format("Email domain '{0}' is not allowed", emailDomain));

                    result = new IdentityResult(errors);
                }

                return result;
            }
        }

        /// <summary>
        /// パスワードカスタムバリデーションを実装の場合
        /// 特定文字(abcdef / 123456)が含まれる場合は不可とする条件を記述
        /// </summary>
        public class MyCustomPasswordValidator : PasswordValidator
        {
            public override async Task<IdentityResult> ValidateAsync(string password)
            {
                IdentityResult result = await base.ValidateAsync(password);

                if (password.Contains("abcdef") || password.Contains("123456"))
                {
                    var errors = result.Errors.ToList();
                    errors.Add("Password can not contain sequence of chars");
                    result = new IdentityResult(errors);
                }
                return result;
            }
        }
#endif
    }
}
