using System;

namespace Checkout.Models
{
    public class CheckoutDto
    {
        public Products Product { get; set; }
        public Customer Customer { get; set; }
    }
}
