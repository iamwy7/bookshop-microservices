using System;

namespace Payment.Models
{
    public class Order
    {
        // Globally Unique Identifier
        public Guid id { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; } 
        public string Phone { get; set; }
        public string ProductId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Stats { get; set; }
    }
}

