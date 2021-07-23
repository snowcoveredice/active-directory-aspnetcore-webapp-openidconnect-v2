using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Constants = WebApp_OpenIDConnect_DotNet.Infrastructure.Constants;

namespace WebApp_OpenIDConnect_DotNet.Controllers
{
    public class GroupAssignController : Controller
    {
        private readonly GraphServiceClient graphServiceClient;

        public GroupAssignController(GraphServiceClient graphServiceClient)
        {
            this.graphServiceClient = graphServiceClient;
        }

        [Authorize(Policy = "GroupAdmin")]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var groupsPage = await graphServiceClient.Groups.Request().GetAsync(cancellationToken);
            ViewData["Groups"] = groupsPage.ToList();
            return View();
        }

        [Authorize(Policy = "GroupAdmin")]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })]
        [HttpGet]
        [Route("Group")]
        public async Task<IActionResult> Group(CancellationToken cancellationToken, string groupId)
        {
            var group = await graphServiceClient.Groups[groupId].Request().GetAsync(cancellationToken);
            ViewData["Group"] = group;

            var groupMembersPage = await graphServiceClient.Groups[groupId].Members.Request().GetAsync(cancellationToken);
            var groupMemberIds = groupMembersPage.Select(member => member.Id).ToList();

            var usersPage = await graphServiceClient.Users.Request().GetAsync(cancellationToken);
            ViewData["UsersNotInGroup"] = usersPage.Where(user => !groupMemberIds.Contains(user.Id)).ToList();
            ViewData["UsersInGroup"] = usersPage.Where(user => groupMemberIds.Contains(user.Id)).ToList();
            return View();
        }

        [Authorize(Policy = "GroupAdmin")]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })]
        [HttpPost]
        [Route("Assign")]
        public async Task<IActionResult> Assign(CancellationToken cancellationToken, string groupId, string[] userIds)
        {
            foreach (var userId in userIds)
            {
                var userToAdd = await graphServiceClient.Users[userId].Request().GetAsync(cancellationToken);
                await graphServiceClient.Groups[groupId].Members.References.Request().AddAsync(userToAdd, cancellationToken);
            }

            var group = await graphServiceClient.Groups[groupId].Request().Expand("members").GetAsync(cancellationToken);
            ViewData["Group"] = group;

            return View("Members");
        }

        [Authorize(Policy = "GroupAdmin")]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })]
        [HttpPost]
        [Route("Remove")]
        public async Task<IActionResult> Remove(CancellationToken cancellationToken, string groupId, string[] userIds)
        {
            foreach (var userId in userIds)
            {
                var userToAdd = await graphServiceClient.Users[userId].Request().GetAsync(cancellationToken);
                await graphServiceClient.Groups[groupId].Members[userId].Reference.Request().DeleteAsync(cancellationToken);
            }

            var group = await graphServiceClient.Groups[groupId].Request().Expand("members").GetAsync(cancellationToken);
            ViewData["Group"] = group;

            return View("Members");
        }
    }
}