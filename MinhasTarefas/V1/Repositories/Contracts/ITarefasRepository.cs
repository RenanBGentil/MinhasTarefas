using MinhasTarefas.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinhasTarefas.Repositories.V1.Contracts
{
    public interface ITarefasRepository
    {
        List<Tarefa> Sincronizacao(List<Tarefa> tarefas);

        List<Tarefa> Resturacao(AplicationUser usuario,DateTime dataUltimaSincronizacao);
    }
}
