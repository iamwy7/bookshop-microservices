using System;

namespace Api.Models
{
    public class ProdutosDto
    {
        /*
            O ideal, é termos uma classe que represente o retorno da API.
            Mais ou menos um objeto serealizador...

            Também serve na hora de juntar as collections, já que elas não tem relacionamentos,
            mas você quer trazer as coisas juntas.

            Também proteje seu backend, e não expõe o funcionamento dele, afinal, quem consome
            minha API, não precisa saber que eu uso mongodb ou algo do tipo...
        */
        public string NomeLivro { get; set; }
        public DateTime DataPub { get; set; }

        public int ISBN { get; set; }
        public string NomeAutor { get; set; }
        public double PrecoLivro { get; set; }
    }
}