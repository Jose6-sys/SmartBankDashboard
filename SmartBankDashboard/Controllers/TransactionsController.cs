using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBankDashboard.Data;
using SmartBankDashboard.Api.Models;
using System.Security.Claims;

namespace SmartBankDashboard.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransactionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(decimal amount)
        {
            if (amount <= 0)
                return BadRequest("Invalid amount");

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                return NotFound("Account not found");

            account.Balance += amount;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = amount,
                Type = "Deposit",
                TransactionDate = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(account.Balance);
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(decimal amount)
        {
            if (amount <= 0)
                return BadRequest("Invalid amount");

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                return NotFound("Account not found");

            if (account.Balance < amount)
                return BadRequest("Insufficient funds");

            account.Balance -= amount;

            var transaction = new Transaction
            {
                AccountId = account.Id,
                Amount = amount,
                Type = "Withdrawal",
                TransactionDate = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(account.Balance);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                return NotFound("Account not found");

            var transactions = await _context.Transactions
                .Where(t => t.AccountId == account.Id)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return Ok(transactions);
        }
    }
}