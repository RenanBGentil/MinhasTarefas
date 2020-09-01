using Microsoft.EntityFrameworkCore.Diagnostics;
using MinhasTarefas.DataBase;
using MinhasTarefas.Repositories.V1.Contracts;
using MinhasTarefas.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefas.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly MinhasTarefasContext _banco;

        public TokenRepository(MinhasTarefasContext banco)
        {
            _banco = banco;
        }
        public Token Obter(string refreshToken)
        {
            return _banco.Token.FirstOrDefault(a=> a.RefreshToken == refreshToken && a.Utilizado == false);
        }
        public void Cadastrar(Token token)
        {
            _banco.Token.Add(token);
            _banco.SaveChanges();
        }
        public void Atualizar(Token token)
        {
            _banco.Token.Update(token);
            _banco.SaveChanges();
        }
    }
}
