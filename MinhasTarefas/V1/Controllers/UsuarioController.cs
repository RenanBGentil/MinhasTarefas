using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MinhasTarefas.Repositories.V1.Contracts;
using MinhasTarefas.V1.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;


namespace MinhasTarefas.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly SignInManager<AplicationUser> _signInManager;
        private readonly UserManager<AplicationUser> _userManager;

        public UsuarioController(IUsuarioRepository usuarioRepository, SignInManager<AplicationUser> signInManager, UserManager<AplicationUser> userManager, ITokenRepository tokenRepository)
        {
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody]UsuarioDTO usuarioDTO)
        {

            ModelState.Remove("Nome");
            ModelState.Remove("ConfimacaoSenha");

            if (ModelState.IsValid)
            {
               AplicationUser usuario = _usuarioRepository.Obter(usuarioDTO.Email, usuarioDTO.Senha);

                if (usuario != null)
                {
                    return GerarToken(usuario);
                }
                else
                {
                    return NotFound("Usuario não localizado");                
                }
                
            }
            else
            {
                return UnprocessableEntity(ModelState); 
            }

        }

       
        [HttpPost("renovar")]
        public ActionResult Renovar([FromBody]TokenDTO tokenDTO)
        {
            var refreshTokenDb =_tokenRepository.Obter(tokenDTO.RefreshToken);

            if (refreshTokenDb == null)
                return NotFound();


            refreshTokenDb.Atualizado = DateTime.Now;
            refreshTokenDb.Utilizado = true;
            _tokenRepository.Atualizar(refreshTokenDb);

            var usuario = _usuarioRepository.Obter(refreshTokenDb.UsuarioId);

            return GerarToken(usuario);
        }

        [HttpPost("")]
        public ActionResult Cadastrar([FromBody]UsuarioDTO usuarioDTO) 
        {
            if (ModelState.IsValid)
            {
                AplicationUser usuario = new AplicationUser();
                usuario.FullName = usuarioDTO.Nome;
                usuario.Email = usuarioDTO.Email;
                usuario.UserName = usuarioDTO.Email;
                var resultado = _userManager.CreateAsync(usuario, usuarioDTO.Senha).Result;

                if (!resultado.Succeeded) 
                {
                    List<string> erros = new List<string>();
                    foreach (var erro in resultado.Errors)
                    {
                        erros.Add(erro.Description);

                    }
                    return UnprocessableEntity(erros);
                }
                else
                {
                    return Ok(usuario);
                }
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }

        private TokenDTO BuildToken(AplicationUser usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("chave-api-jwt-minhas-tarefas"));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: exp,
                    signingCredentials: sign
                );

            var refreshToken = Guid.NewGuid().ToString();

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var expRefreshToken = DateTime.UtcNow.AddHours(2);

            var tokenDTO = new TokenDTO { Token = tokenString, Expiration = exp, ExpirationRefreshToken = expRefreshToken, RefreshToken = refreshToken };

            return tokenDTO;
        }
        private ActionResult GerarToken(AplicationUser usuario)
        {
            var token = BuildToken(usuario);

            var tokenModel = new Token()
            {
                RefreshToken = token.RefreshToken,
                ExpirationToken = token.Expiration,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                Usuario = usuario,
                Criado = DateTime.Now,
                Utilizado = false,
            };
            _tokenRepository.Cadastrar(tokenModel);
            return Ok(token);
        }
    }
}
