using Accio.Business.Models.AuthenticationModels;
using Accio.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accio.Business.Services.AuthenticationHistoryServices
{
    public class AuthenticationHistoryService
    {
        private AccioContext _context { get; set; }

        public AuthenticationHistoryService(AccioContext context)
        {
            _context = context;
        }

        public void LogAuthentication(AuthAttemptType attemptType, string userName, string emailAddress, Guid clientId, string bogusData = null)
        {
            var now = DateTime.UtcNow;
            var history = new AuthenticationHistory() 
            {
                AuthenticationHistoryId = Guid.NewGuid(),
                Username = userName,
                EmailAddress = emailAddress,
                ClientId = clientId,
                BogusData = bogusData,
                Type = attemptType.ToString(),
                CreatedById = Guid.Empty,
                CreatedDate = now,
                UpdatedById = Guid.Empty,
                UpdatedDate = now,
                Deleted = false,
            };
            _context.AuthenticationHistories.Add(history);
            _context.SaveChanges();
        }
    }
}
