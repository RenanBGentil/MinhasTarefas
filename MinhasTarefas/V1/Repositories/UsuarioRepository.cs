using Microsoft.AspNetCore.Identity;
using MinhasTarefas.Repositories.V1.Contracts;
using MinhasTarefas.V1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinhasTarefas.V1.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UserManager<AplicationUser> _userManager;

        public UsuarioRepository(UserManager<AplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public AplicationUser Obter(string email, string senha)
        {
           var usuario = _userManager.FindByEmailAsync(email).Result;
            if (_userManager.CheckPasswordAsync(usuario, senha).Result)
            {
                return usuario;
            }
            else
            {
                throw new Exception("Usuário não localizado!");
            }
        }

        public void Cadastrar(AplicationUser usuario, string senha)
        {
           var result = _userManager.CreateAsync(usuario, senha).Result;

            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in result.Errors) 
                {
                    sb.Append(error.Description);
                }
                
                throw new Exception($"Usuário não localizado! {sb.ToString()}");
            }
        }

        public AplicationUser Obter(string id)
        {
            return _userManager.FindByIdAsync(id).Result;
        }
    }
}
