using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MimicAPI.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _banco;

        public PalavraRepository(MimicContext banco)
        {
            _banco = banco;
        }

        public PaginationList<Palavra> ObterPalavras(PalavraUrlQuery query)
        {
            var lista = new PaginationList<Palavra>();

            var item = _banco.Palavras.AsNoTracking().AsQueryable();
            if (query.data.HasValue)
            {
                item = item.Where(p => p.Criado > query.data.Value || p.Atualizado > query.data.Value);
            }

            if (query.pagNumero.HasValue)
            {
                var quantidadeTotalRegistros = item.Count();
                item = item.Skip((query.pagNumero.Value - 1) * query.pagRegistro.Value).Take(query.pagRegistro.Value);

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.pagNumero.Value;
                paginacao.RegistrosPorPagina = query.pagRegistro.Value;
                paginacao.TotalRegistros = quantidadeTotalRegistros;
                paginacao.TotalPaginas = (int)Math.Ceiling((double)quantidadeTotalRegistros / query.pagRegistro.Value); /* 30/10 = 3   21/10=2,1 = 3  */

                lista.Paginacao = paginacao;
            }

            lista.AddRange(item.ToList());

            return lista;
        }

        public Palavra Obter(int id)
        {
            return _banco.Palavras.AsNoTracking().FirstOrDefault(p => p.Id == id);
        }

        public void Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra = Obter(id);
            palavra.Ativo = false;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

    }
}
