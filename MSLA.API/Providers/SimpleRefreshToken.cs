using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security.Infrastructure;

namespace MSLA.API.Providers
{
    public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private AuthRepository _repo;
        public SimpleRefreshTokenProvider()
        {
            _repo = new AuthRepository();
        }
        public void Create(AuthenticationTokenCreateContext context)
        {
            
        }

        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

            if (string.IsNullOrEmpty(clientid))
            {
                return ;
            }

            //var refreshTokenId = Guid.NewGuid().ToString("n"); 
            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            var token = new RefreshToken()
            {
                fldUserId = Convert.ToInt64(context.Ticket.Properties.Dictionary["UserID"]),//Helper.GetHash(refreshTokenId),
                fldWebClientId = clientid,
                fldSubject = context.Ticket.Identity.Name,
                fldIssuedUtc = DateTime.UtcNow,
                fldExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime)),
                fldRemoteIP = context.Request.RemoteIpAddress
            };

            context.Ticket.Properties.IssuedUtc = token.fldIssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.fldExpiresUtc;

            token.fldProtectedTicket = context.SerializeTicket();

            var result = await _repo.AddRefreshToken(token);

            if (result)
            {
                context.SetToken(context.Ticket.Properties.Dictionary["UserID"]);
            }
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        { 
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            var hashedTokenId = Convert.ToInt64(context.Token);

            var refreshToken = _repo.FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.fldProtectedTicket);
               await _repo.RemoveRefreshToken(hashedTokenId);
            }

        }
    }
}