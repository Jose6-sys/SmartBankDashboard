using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBankDashboard.Data;
using System.Security.Claims;

namespace SmartBankDashboard.Controllers
{
    [Authorize] // user must be logged in
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("my")]
        public IActionResult GetMyAccount()
        {
            //  THIS is where you extract UserId
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var account = _context.Accounts
                .FirstOrDefault(a => a.UserId == userId);

            return Ok(account);
        }
    }
}
