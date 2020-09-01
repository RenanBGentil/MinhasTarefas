using MinhasTarefas.DataBase;
using MinhasTarefas.Repositories.V1.Contracts;
using MinhasTarefas.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MinhasTarefas.V1.Repositories
{
    public class TarefasRepository : ITarefasRepository
    {
        private readonly MinhasTarefasContext _banco;

        public TarefasRepository(MinhasTarefasContext banco)
        {
            _banco = banco;
        }

        public List<Tarefa> Resturacao(AplicationUser usuario, DateTime dataUltimaSincronizacao)
        {
            var query = _banco.Tarefas.Where(a=> a.UsuarioId == usuario.Id).AsQueryable();
            if (dataUltimaSincronizacao != null)
            {
                query.Where(a=> a.Criado >= dataUltimaSincronizacao || a.Atualizado >= dataUltimaSincronizacao);
            }

            return query.ToList<Tarefa>();
        }

        public List<Tarefa> Sincronizacao(List<Tarefa> tarefas)
        {
            var tarefasNovas = tarefas.Where(a=> a.IdTarefaApi == 0).ToList();
            var tarefasExcluidasAtualizadas = tarefas.Where(a => a.IdTarefaApi != 0).ToList();

            if (tarefasNovas.Count() > 0)
            {
                foreach (var tarefa in tarefasNovas)
                {
                    _banco.Tarefas.Add(tarefa);
                }
            }


            if (tarefasExcluidasAtualizadas.Count() > 0)
            {
                foreach (var tarefa in tarefasExcluidasAtualizadas)
                {
                    _banco.Tarefas.Update(tarefa);
                }
            }
            _banco.SaveChanges();

            return tarefasNovas.ToList() ; 
        }

    }
}
