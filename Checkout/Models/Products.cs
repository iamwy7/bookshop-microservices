using System;

namespace Checkout.Models
{
    public class Products
    {

        public string id { get; set; } 
        public string nomeLivro { get; set; } 
        public DateTime dataPub { get; set; } 
        public int isbn { get; set; } 
        public string nomeAutor { get; set; } 
        public double precoLivro { get; set; } 
        // public Products(string nomeLivro, DateTime dataPub, int isbn, string nomeAutor, double precoLivro)
        // {
        //     this.nomeLivro = nomeLivro;
        //     this.dataPub = dataPub;
        //     this.isbn = isbn;
        //     this.nomeAutor = nomeLivro;
        //     this.precoLivro = precoLivro;
        // }
    }
}

