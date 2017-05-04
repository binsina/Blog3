using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using  Blog3.Models;
using System.Web.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Web.Helpers;
using System.Net.Mail;
using System.Net;

namespace  Blog3
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.

            // SendGrid email code.  Some people had trouble with SendGrid
            //dynamic sg = new SendGridAPIClient(
            //      WebConfigurationManager.AppSettings["SendGridAPIKey"]);

            //Email from = new Email(WebConfigurationManager.AppSettings["ContactEmail"]);
            //Email to = new Email(message.Destination);

            //Content content = new Content("text/plain", message.Body);

            //Mail mail = new Mail(from, message.Subject, to, content);

            //dynamic response = await sg.client.mail.send.post(requestBody: mail.Get());

            //Send email with Gmail with a separate method
            SendViaGmail(message);
        }

        private void SendViaGmail(IdentityMessage message)
        {
            // Example mail code
            try
            {
                var smtp = new SmtpClient
                {
                    Host = WebConfigurationManager.AppSettings["EmailHost"],
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["EmailSender"],
                    WebConfigurationManager.AppSettings["EmailSenderPassword"])
                };

                // Configure your credentials

                // Build your message
                MailMessage mail = new MailMessage();
                // Configure it
                mail.To.Add(message.Destination);
                mail.From = new MailAddress("binsina9@gmail.com", "wahid H J");
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                // Enable if you are sending HTML content
                mail.IsBodyHtml = false;

                // Send your message
                smtp.Send(mail);

            }
            catch (Exception ex)
            {
                // Something went wrong sending your message; Handle accordingly                
            }
        }
    }




    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            //manager.EmailService = new PersonalEmail();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }


   

}