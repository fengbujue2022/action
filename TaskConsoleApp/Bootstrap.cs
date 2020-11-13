using APIClient;
using Infrastructure.Autofac.Attributes;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    [AutofacResolve]
    public class Bootstrap
    {
        [AutofacResolve]
        private readonly ILogger _logger;
        [AutofacResolve]
        private readonly HitokotoApi _hitokotoApi;
        [AutofacResolve]
        private readonly IOptionsMonitor<Config> _configOptions;

        public async Task Main()
        {

            await DailyNeteaseCloud();

        }

        /// <summary>
        /// 每天一句网抑云
        /// </summary>
        /// <returns></returns>
        protected async Task DailyNeteaseCloud()
        {
            _logger.LogInformation("-----网抑云模式开始-----");

            var hitokoResponse = await _hitokotoApi.FetchContent(Category.NeteaseCloud);
            var config = _configOptions.CurrentValue;
            var toEmailAddressList = config.ToEmailAddressList;
            if (toEmailAddressList.Any())
            {
                foreach (var to in toEmailAddressList)
                {
                    await SendEmailAsync(to, config.EmailAccount, config.EmailKey,
                        $"{hitokoResponse.hitokoto}——{hitokoResponse.from}"
                        );
                    _logger.LogInformation($"今日份网抑云已发送到: {to}");
                }
            }

            _logger.LogInformation("-----网抑云模式关闭-----");

        }

        protected async static Task SendEmailAsync(string to, string from, string key, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(from, from));
            emailMessage.To.Add(new MailboxAddress(to, to));
            emailMessage.Subject = "今日份网抑云";
            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.qq.com", 25);
                await client.AuthenticateAsync(from, key);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
