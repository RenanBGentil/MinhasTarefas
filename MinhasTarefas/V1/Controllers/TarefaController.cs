using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinhasTarefas.Repositories;
using MinhasTarefas.Repositories.V1.Contracts;
using MinhasTarefas.V1.Models;

namespace MinhasTarefas.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TarefaController : ControllerBase
    {
        private readonly ITarefasRepository _tarefasRepository;
        private readonly UserManager<AplicationUser> _userManager;

        public TarefaController(ITarefasRepository tarefasRepository, UserManager<AplicationUser> userManager)
        {
            _tarefasRepository = tarefasRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("sincronizar")]
        public ActionResult Sincronizar ([FromBody]List<Tarefa> tarefa)
        {
            return Ok(_tarefasRepository.Sincronizacao(tarefa));
        }

        [Authorize]
        [HttpGet("restaurar")]
        public ActionResult Restaurar (DateTime data)
        {
            var usuario = _userManager.GetUserAsync(HttpContext.User).Result;
            
            return Ok(_tarefasRepository.Resturacao(usuario, data)); 
        }
    }
}
