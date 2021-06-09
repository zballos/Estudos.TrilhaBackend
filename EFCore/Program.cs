using System.Net;
using System.Net.Http.Headers;
using System;
using EFCore.Domain;
using System.Linq;
using System.Collections.Generic;

namespace EFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            InserirDados();
            InserirDadosEmMassa();
            ObterProdutos();
            CadastrarPedido();
        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto 1",
                CodigoBarras = "123123123",
                Valor = 15m,
                TipoProduto = ValueObjects.TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new Data.ApplicationContext();
            db.Produtos.Add(produto);
            // db.Set<Produto>().Add(produto);
            // db.Entry(produto).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            // db.Add(produto);
            var registros = db.SaveChanges();
            Console.WriteLine($"Registros: {registros}");
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto 2",
                CodigoBarras = "123123123",
                Valor = 10m,
                TipoProduto = ValueObjects.TipoProduto.Embalagem,
                Ativo = true
            };

            var cliente = new Cliente
            {
                CEP = "1111",
                Cidade = "Manaus",
                Email = "teste@teste.com",
                Estado = "AM",
                Nome = "John",
                Telefone = "98 99888777"
            };

            using var db = new Data.ApplicationContext();
            db.AddRange(produto, cliente);

            var registros = db.SaveChanges();
            Console.WriteLine($"Registros: {registros}");
        }

        private static void ObterProdutos()
        {
            using var db = new Data.ApplicationContext();
            foreach (var produto in db.Produtos.ToList())
            {
                Console.WriteLine($"ID: {produto.Id} | Descrição: {produto.Descricao} | Valor: {produto.Valor} | Tipo Produto: {produto.TipoProduto.ToString()}");
            }
        }

        private static void CadastrarPedido()
        {
            using var db = new Data.ApplicationContext();
            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            var pedido = new Pedido
            {
                ClientId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Teste 1",
                StatusPedido = ValueObjects.StatusPedido.Analise,
                TipoFrete = ValueObjects.TipoFrete.CIF,
                Itens = new List<PedidoItem>
                {
                    new PedidoItem
                    {
                        PedidoId = produto.Id,
                        Desconto = 1,
                        Quantidade = 2,
                        Valor = 166
                    }
                }
            };

            db.Pedidos.Add(pedido);
            db.SaveChanges();
        }

        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();
            var pedidos = db.Pedidos.ToList();

            Console.WriteLine(pedidos.Count);
        }
    }
}
