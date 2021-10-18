using Microsoft.AspNet.SignalR;

namespace CSGOGamble
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            // your logic to fetch a user identifier goes here.

            // for example:
            return request.User.Identity.Name;
        }
    }
}
