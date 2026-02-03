using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartBankDashboard.Api.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } // Debit / Credit

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}