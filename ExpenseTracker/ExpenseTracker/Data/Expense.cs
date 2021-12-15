using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseTracker.Data
{
    public class Expense : IValidatableObject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid ExpenseID { get; set; }
        [Required]
        public decimal Amount { get; set; }

        [Display(Name = "Category")]
        [Required]
        [ForeignKey("Category")]
        public Guid CategoryID { get; set; }

        [Display(Name = "Date")]
        [Required]
        public DateTime Date_Added { get; set; }

        public virtual Category Category { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> results = new List<ValidationResult>();
            if (Date_Added.Date > DateTime.Now.Date)
            {
                results.Add(new ValidationResult($"Please enter a value less than or equal to {DateTime.Now.Date.ToString("dd/MM/yyyy")}", new[] { "Date_Added" }));
            }
            return results;
        }
    }
}
