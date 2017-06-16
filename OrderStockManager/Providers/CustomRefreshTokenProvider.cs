using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Owin.Security;
using OrderStockManager.Services;

namespace OrderStockManager.Providers
{
    public class RefreshToken
    {
        public RefreshToken()
        {
            this.expires = DateTime.UtcNow.AddDays(1);
            this.clientIp = HttpContext.Current.Request.UserHostAddress;
        }

        public RefreshToken(AuthenticationTicket ticket) : this()
        {
            this.ticket = ticket;
        }

        public AuthenticationTicket ticket;
        public DateTime expires;
        public string clientIp;

        public bool isExpired()
        {
            return this.expires < DateTime.UtcNow;
        }
    }

    public class CustomRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, RefreshToken> _refreshTokens = new ConcurrentDictionary<string, RefreshToken>();

        public void Create(AuthenticationTokenCreateContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];

            RefreshToken dticket;
            var dlist = _refreshTokens.Where(x => x.Value.ticket.Identity.Name == context.Ticket.Identity.Name).Select(x => x.Key).ToList();
            foreach (var item in dlist)
            {
                _refreshTokens.TryRemove(item, out dticket);
            }

            if (originalClient != null)
            {
                var guid = ShortGuid.NewGuid();
                _refreshTokens.TryAdd(guid, new RefreshToken(context.Ticket));
                context.SetToken(guid);
            }
        }

        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Create(context);
            return Task.FromResult(0);
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            RefreshToken token;
            if (_refreshTokens.TryRemove(context.Token, out token))
            {
                if (!token.isExpired())
                {
                    context.SetTicket(token.ticket);
                }
            }
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Receive(context);
            return Task.FromResult(0);
        }
    }
}
