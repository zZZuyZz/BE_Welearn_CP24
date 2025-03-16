using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RepoLayer.Interface;
using MimeKit;
using MailContentType = MimeKit.ContentType;
using MimeKit.Utils;
using Microsoft.Extensions.Configuration;
using DataLayer.DbObject;
using ServiceLayer.Utils;
using ServiceLayer.DTOs;
using DataLayer.Enums;
using ServiceLayer.Services.Interface.Db;
using ServiceLayer.Services.Interface.Mail;

namespace ServiceLayer.Services.Implementation.Mail
{
    public class AutoMailService : IAutoMailService
    {
        private readonly IRepoWrapper repos;
        private readonly IAccountService accountsService;
        private readonly IStatService statService;
        //private readonly IServiceWrapper services;

        public AutoMailService(IWebHostEnvironment env, IRepoWrapper repositories, IConfiguration configuration/*, IServiceWrapper services*/, IAccountService accountsService, IStatService statService)
        {
            _emailConfig = configuration
                .GetSection("EmailConfiguration")
                .Get<MailConfiguration>();
            //this.env = env;
            rootPath = env.WebRootPath;
            repos = repositories;
            logoPath = rootPath + Path.DirectorySeparatorChar + "Images" + Path.DirectorySeparatorChar +
                           //"logowhite.png";
                           "Logo.png";
            this.accountsService = accountsService;
            this.statService = statService;
            //this.services = services;
        }

        /// <summary>
        ///     The default Email Template. {0}: email content
        /// </summary>
        private const string DefaultTemplate = "<h2 style='color:red;'>{0}</h2>";
        private readonly MailConfiguration _emailConfig;
        //private readonly IWebHostEnvironment env;
        private readonly string rootPath;
        private readonly string logoPath;

        public async Task<bool> SendEmailWithDefaultTemplateAsync(IEnumerable<string> receivers, string subject,
            string content, IFormFileCollection attachments)
        {
            MailMessageEntity message = new MailMessageEntity(receivers, subject, content, attachments);
            List<MimeMessage> mimeMessages = CreateMimeMessageWithSimpleTemplateList(message /*, rootPath*/);

            foreach (var mimeMessage in mimeMessages) await SendAsync(mimeMessage);
            return true;
        }

        public async Task<bool> SendNewPasswordMailAsync(string email)
        {
            Account account = await repos.Accounts.GetByEmailAsync(email);
            MimeMessage message = await CreateMimeMessageForNewPasswordAsync(account);
            await SendAsync(message);
            return true;
        }

        public async Task<bool> SendConfirmResetPasswordMailAsync(string email, string serverLink)
        {
            Account account = await repos.Accounts.GetByEmailAsync(email);
            if(serverLink.EndsWith("/"))
            {
                serverLink = serverLink.Substring(0, serverLink.Length-1);
            }
            MimeMessage message = await CreateMimeMessageForResetPasswordAsync(account, serverLink);
            await SendAsync(message);
            return true;
        }

        public async Task<bool> SendMonthlyStatAsync()
        {
            var students = repos.Accounts.GetList()
                //.Include(a=>a.SupervisionsForStudent).ThenInclude(s=>s.Parent)
                .Where(e => e.RoleId == (int)RoleNameEnum.Student);
            List<MimeMessage> mimeMessages = new List<MimeMessage>();
            foreach (var student in students)
            {
                StatGetDto stat = await statService.GetStatForAccountInMonth(student.Id, DateTime.Now);
                mimeMessages.AddRange(CreateMimeMessageForMonthlyStat(student, stat));
            }
            foreach (var mimeMessage in mimeMessages)
            {
                await SendAsync(mimeMessage);
            }
            return true;
        }
        private List<MimeMessage> CreateMimeMessageForMonthlyStat(Account student, StatGetDto stat)
        {
            List<MimeMessage> list = new List<MimeMessage>();
            List<Account> receivers = new List<Account>() { student };
            //receivers.AddRange(student.SupervisionsForStudent.Select(e => e.Parent));
            string template = MailTemplateHelper.MONTHLY_STAT_TEMPLATE(rootPath);

            foreach (var receiver in receivers)
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailConfig.From));
                mimeMessage.To.Add(new MailboxAddress(receiver.Email));
                mimeMessage.Subject = "Chi tiết học tập của tháng";


                var bodyBuilder = new BodyBuilder();
                try
                {
                    //var email = receiver.Address;
                    //<!--{0} is logo-->
                    //<!--{1} is username-->
                    //<!--{2} is content-->
                    //bodyBuilder = new BodyBuilder { HtmlBody = string.Format(template, logoPath, email, message.Content) };
                    var logo = bodyBuilder.LinkedResources.Add(logoPath);
                    logo.ContentId = MimeUtils.GenerateMessageId();
                    bodyBuilder.HtmlBody = FormatTemplate(template, logo.ContentId, receiver.FullName,
                        stat.Month.ToString("MM/yyyy"), stat.StudentFullname, stat.StudentUsername, stat.TotalMeetingsCount.ToString(),
                        stat.AtendedMeetingsCount.ToString(), stat.MissedMeetingsCount.ToString(), stat.TotalMeetingTme, stat.AverageVoteResult.ToString());
                }
                catch
                {
                    //bodyBuilder = new BodyBuilder { HtmlBody = string.Format(DefaultTemplate, message.Content) };
                    bodyBuilder = new BodyBuilder { HtmlBody = FormatTemplate(DefaultTemplate, stat.ToHtml()) };
                }

                mimeMessage.Body = bodyBuilder.ToMessageBody();
                list.Add(mimeMessage);
            }
            return list;
        }
        #region old code
        //public async Task<bool> SendEmailWithDefaultTemplateAsync(MailMessageEntity message)
        //{
        //    var mimeMessages = CreateMimeMessageWithSimpleTemplateList(message/*, rootPath*/);

        //    foreach (var mimeMessage in mimeMessages) await SendAsync(mimeMessage);
        //    return true;
        //}

        //public async Task<bool> SendSimpleMailAsync(IEnumerable<string> receivers, string subject, string content, IFormFileCollection attachments)
        //{
        //    MailMessageEntity message = new MailMessageEntity(receivers, subject, content, attachments);
        //    MimeMessage mailMessage = CreateSimpleEmailMessage(message);

        //    await SendAsync(mailMessage);
        //    return true;
        //}
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private List<MimeMessage> CreateMimeMessageWithSimpleTemplateList(MailMessageEntity message)
        {
            List<MimeMessage> list = new List<MimeMessage>();
            //var templatePath = rootPath + Path.DirectorySeparatorChar + MailTemplateHelper.FOLDER +
            //                   Path.DirectorySeparatorChar + MailTemplateHelper.DEFAULT_TEMPLATE_FILE;
            string template = MailTemplateHelper.DEFAULT_TEMPLATE(rootPath);

            foreach (var receiver in message.Receivers)
            {
                var mimeMessage = new MimeMessage();
                mimeMessage.From.Add(new MailboxAddress(_emailConfig.From));
                mimeMessage.To.Add(receiver);
                mimeMessage.Subject = message.Subject;


                var bodyBuilder = new BodyBuilder();
                try
                {
                    var email = receiver.Address;
                    //<!--{0} is logo-->
                    //<!--{1} is username-->
                    //<!--{2} is content-->
                    //bodyBuilder = new BodyBuilder { HtmlBody = string.Format(template, logoPath, email, message.Content) };
                    var logo = bodyBuilder.LinkedResources.Add(logoPath);
                    logo.ContentId = MimeUtils.GenerateMessageId();
                    bodyBuilder.HtmlBody = FormatTemplate(template, logo.ContentId, email, message.Content);
                }
                catch
                {
                    //bodyBuilder = new BodyBuilder { HtmlBody = string.Format(DefaultTemplate, message.Content) };
                    bodyBuilder = new BodyBuilder { HtmlBody = FormatTemplate(DefaultTemplate, message.Content) };
                }

                if (message.Attachments != null && message.Attachments.Any())
                {
                    byte[] fileBytes;
                    foreach (var attachment in message.Attachments)
                    {
                        using (var ms = new MemoryStream())
                        {
                            attachment.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }

                        bodyBuilder.Attachments.Add(attachment.FileName, fileBytes,
                            MailContentType.Parse(attachment.ContentType));
                    }
                }

                mimeMessage.Body = bodyBuilder.ToMessageBody();
                list.Add(mimeMessage);
            }

            return list;
        }

        private async Task<MimeMessage> CreateMimeMessageForResetPasswordAsync(Account account, string serverLink)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailConfig.From));
            mimeMessage.To.Add(new MailboxAddress(account.Email));
            mimeMessage.Subject = "Reset password";

            string secret = DateTime.Today.ToString("yyyy-MM-dd").CustomHash();
            string resetLink = serverLink + $"/api/Accounts/Password/Reset/Confirm?email={account.Email}&secret={secret}";
            //<!--{0} is logo-->
            //<!--{1} is fullname-->
            //<!--{2} is content-->
            var bodyBuilder = new BodyBuilder();
            try
            {
                string template = MailTemplateHelper.CONFIRM_RESET_PASSWORD_TEMPLATE(rootPath);
                var logo = bodyBuilder.LinkedResources.Add(logoPath);
                logo.ContentId = MimeUtils.GenerateMessageId();
                bodyBuilder.HtmlBody = FormatTemplate(template, logo.ContentId, account.FullName, resetLink);
            }
            catch
            {
                bodyBuilder = new BodyBuilder { HtmlBody = FormatTemplate(DefaultTemplate, $"<div>Link để lấy mật khẩu là {resetLink}</div>") };
            }
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            return mimeMessage;
        }


        private async Task<MimeMessage> CreateMimeMessageForNewPasswordAsync(Account account)
        {
            string newPassword = PasswordUtil.RandomPassword(9);
            //account.Password = newPassword.CustomHash();
            //await accountsService.UpdateAsync(account);
            await accountsService.UpdatePasswordAsync(account.Id, new AccountChangePasswordDto()
            {
                OldPassword = account.Password,
                Password = newPassword,
                ConfirmPassword = newPassword
            });

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(_emailConfig.From));
            mimeMessage.To.Add(new MailboxAddress(account.Email));
            mimeMessage.Subject = "New password";

            //<!--{0} is logo-->
            //<!--{1} is fullname-->
            //<!--{2} is content-->
            //bodyBuilder = new BodyBuilder { HtmlBody = string.Format(template, logoPath, email, message.Content) };
            var bodyBuilder = new BodyBuilder();
            try
            {
                string template = MailTemplateHelper.NEW_PASSWORD_TEMPLATE(rootPath);
                var logo = bodyBuilder.LinkedResources.Add(logoPath);
                logo.ContentId = MimeUtils.GenerateMessageId();
                bodyBuilder.HtmlBody = FormatTemplate(template, logo.ContentId, account.FullName, newPassword);
            }
            catch
            {
                bodyBuilder = new BodyBuilder { HtmlBody = FormatTemplate(DefaultTemplate, $"<div>Mật khẩu mới của bạn là {account.Password}</div>") };
            }
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            return mimeMessage;
        }



        /// <summary>
        ///     Replacing {number} marker in the email template with string values
        /// </summary>
        /// <param name="template">Email template</param>
        /// <param name="values">String values</param>
        /// <returns></returns>
        /// 
        private string FormatTemplate(string template, string logoContentId, params string[] values)
        {
            template = template.Replace("{logo}", logoContentId);
            for (var i = 0; i < values.Length; i++)
                // {{{i}}}: {0}, {1} in string
                template = template.Replace($"{{{i}}}", values[i]);
            return template;
        }


        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.AppPassword);

                    await client.SendAsync(mailMessage);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        #region Unused Code


        //private MimeMessage CreateSimpleEmailMessage(MailMessageEntity message)
        //{
        //    MimeMessage emailMessage = new MimeMessage();
        //    emailMessage.From.Add(new MailboxAddress(_emailConfig.From));
        //    emailMessage.To.AddRange(message.Receivers);
        //    emailMessage.Subject = message.Subject;

        //    //var bodyBuilder = new BodyBuilder { HtmlBody = string.Format(DefaultTemplate, message.Content) };
        //    var bodyBuilder = new BodyBuilder { HtmlBody = FormatTemplate(DefaultTemplate, message.Content) };

        //    if (message.Attachments != null && message.Attachments.Any())
        //    {
        //        byte[] fileBytes;
        //        foreach (var attachment in message.Attachments)
        //        {
        //            using (var ms = new MemoryStream())
        //            {
        //                attachment.CopyTo(ms);
        //                fileBytes = ms.ToArray();
        //            }

        //            bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
        //        }
        //    }

        //    emailMessage.Body = bodyBuilder.ToMessageBody();
        //    return emailMessage;
        //}
        #endregion

    }
}
