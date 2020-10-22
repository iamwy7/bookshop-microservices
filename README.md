# Microservices-BookShop :tw-1f4da:
Esse projeto é meu primeiro em arquitetura de microservices, e é um sistema fake de checkout de pedido de uma loja ( no caso, de livros ) baseado no conteúdo do canal [Fullcycle](https://www.youtube.com/channel/UCMUoZehUZBhLb8XaTc8TQrA "Fullcycle") do youtube, paralelamente também as empresas [School of net](https://www.schoolofnet.com/ "School of net") e a [Code.Education](https://code.education/ "Code.Education").

O Intuito é adquirir experiência com arquitetura de microservices, em um estudo de caso atual,  com tecnologias atuais.

Essa é a arquitetura, o que ela usa, e como ela se comunica:

[![Microservices-Bookshop](https://i.imgur.com/QLjPZy2.png "Microservices-Bookshop")](https://i.imgur.com/QLjPZy2.png "Microservices-Bookshop")

## ATTENTION :tw-26a0:
Os sistemas são bem simples, bem simples meeeeeeeesmo, somente o essencial pro fluxo de checkout funcionar, feitos em dotnet Core. Lembrando, é para adquirir experiência com a arquitetura, e as tecnologias.

## How to run :tw-1f680: 
Todo o ambiente foi feito no Docker/Kubernetes.
Temos os arquivos descritivos dos Deployments, para você rodar no seu minikube.
Atualmente as imagens docker apontam para o meu perfil no hub.docker, fica a seu critério trocar :)

##### Fluxo explicado:
 1- Primeiro de tudo, cadastre quantos livros quiser na API enviando via HTTP POST um json como esse:

```json
{
	"Turtles All the Way Down",
	"dataPub": "2017-08-10",
	"isbn": 9788551002001
	"NomeAutor": -"John Green",
	"PrecoLivro": 26.72
}
```
2- Depois isso, o Catalogo pode mostrar os seus livros cadastrados.

3- Com o ID do livro, no navegador mesmo, vá para http://ip-do-checkout/Index/id-do-produto

4- Ele vai te pedir seus "dados cadastrais" para enviar o "pedido".

5- Depois disso, você pode acompanhar no log do console do microservice de Order, indo para o Payment para provação de pagamento.

6- O Resultado é aleatório, e também pode ser visto no log do console do microservice de Payment, e é retornado para o de Order.

7- O Order atualiza o status do pedido no Redis, e se aprovado, envia para os outros microservices possíveis seguintes.

Bem Tranquilo :tw-1f604:


