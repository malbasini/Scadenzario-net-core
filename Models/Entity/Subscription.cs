using System;
using Scadenzario.Models.Entities;
using Scadenzario.Models.Entity;

namespace Scadenzario.Models.Entities
{
    public class Subscription
    {
        public Subscription(string userId, int scadenzaId)
        {
            UserId = userId;
            ScadenzaId = scadenzaId;
        }
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ScadenzaId { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentType { get; set; }
        public decimal Paid { get; set; }
        public string TransactionId { get; set; }

        public virtual Scadenza scadenze { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
