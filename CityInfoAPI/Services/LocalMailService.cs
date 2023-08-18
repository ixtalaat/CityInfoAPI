﻿namespace CityInfoAPI.Services
{
	public class LocalMailService : IMailService
	{
		private readonly string _mailTo = string.Empty;
		private readonly string _mailFrom = string.Empty;
        public LocalMailService(IConfiguration configuration)
        {
			_mailTo = configuration["mailSettings:mailToAddress"] ?? string.Empty;
			_mailFrom = configuration["mailSettings:mailFromAddress"] ?? string.Empty;
        }

        public void Send(string subject, string message)
		{
			// send mail - output to console window
			Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with {nameof(LocalMailService)}.");
			Console.WriteLine($"Subject: {subject}");
			Console.WriteLine($"Message: {message}");
		}
	}
}
