using System;

namespace Checkout.Models
{
    public class Customer
    {
        public string Name { get; set; } 
        public string Email { get; set; } 
        public string Phone { get; set; }
        public string CreditCard { get; set; }
        // public Customer(string name, string email, string phone, string creditCard)
        // {
        //     Name = name;
        //     Email = email;
        //     Phone = phone;
        //     CreditCard = creditCard;
        // }
    }
}

