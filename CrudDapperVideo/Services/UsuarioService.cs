using System.Data.SqlClient;
using AutoMapper;
using CrudDapperVideo.Dto;
using CrudDapperVideo.Models;
using Dapper;

namespace CrudDapperVideo.Services
{
    public class UsuarioService : IUsuarioInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UsuarioService(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ResponseModel<UsuarioListarDto>> BuscarUsuarioPorId(int usuarioId)
        {
            ResponseModel<UsuarioListarDto> response = new ResponseModel<UsuarioListarDto>();

            using(var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuarioBanco = await connection.QueryFirstOrDefaultAsync<Usuario>("SELECT * FROM Usuarios WHERE id = @Id", new { Id = usuarioId });

                if(usuarioBanco == null)
                {
                    response.Mensagem = "Nenhum usuário localizado!";
                    response.Status = false;
                    return response;
                }

                var usriarioMapeado = _mapper.Map<UsuarioListarDto>(usuarioBanco);

                response.Dados = usriarioMapeado;
                response.Mensagem = "Usuario localizado com sucesso!";

            }
            return response;

        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> BuscarUsuarios()
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuariosBanco = await connection.QueryAsync<Usuario>("SELECT * FROM Usuarios");

                if(usuariosBanco.Count() == 0)
                {
                    response.Mensagem = "Nenhum usuário localizado!";
                    response.Status = false;
                    return response;
                }

                //Transformação Mapper
                var usuarioMapeado = _mapper.Map<List<UsuarioListarDto>>(usuariosBanco);

                response.Dados = usuarioMapeado;
                response.Mensagem = "Usuarios localizados com sucesso!";
            }
            return response;
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> CriarUsuario(UsuarioCriarDto usuarioCriarDto)
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usariosBanco = await connection.ExecuteAsync("INSERT INTO Usuarios (NomeCompleto, Email, Cargo, Salario, CPF, Senha, Situacao)" +
                                                                 "values (@NomeCompleto, @Email, @Cargo, @Salario, @CPF, @Senha, @Situacao)", usuarioCriarDto);
                if(usariosBanco == 0)
                {
                    response.Mensagem = "Ocorreu um erro ao realizar o registro!";
                    response.Status = false;
                    return response;
                }

                var usuarios = await ListarUsuarios(connection);

                var usuariosMapeados = _mapper.Map<List<UsuarioListarDto>>(usuarios);

                response.Dados = usuariosMapeados;
                response.Mensagem = "Usuários Listados Com Sucesso!";

            }

            return response;
        }

        private static async Task<IEnumerable<Usuario>> ListarUsuarios(SqlConnection connection)
        {
            return await connection.QueryAsync<Usuario>("SELECT * FROM Usuarios");
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> EditarUsuario(UsuarioEditarDto usuarioEditarDto)
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuarioBanco = await connection.ExecuteAsync("UPDATE Usuarios SET NomeCompleto = @NomeCompleto," +
                                                                 "Email = @Email, Cargo = @Cargo, Salario = @Salario, " +
                          
                                                                 "Situacao = @Situacao, CPF = @CPF WHERE Id = @Id", usuarioEditarDto);
               
                if (usuarioBanco == 0)
                {
                    response.Mensagem = "Ocorreu um eroo ao realizar a edição!";
                    response.Status = false;
                    return response;
                }

                var usuarios = await ListarUsuarios(connection);

                var usuariosMspeados = _mapper.Map<List<UsuarioListarDto>>(usuarios);

                response.Dados = usuariosMspeados;
                response.Mensagem = "Usuarios listados com sucesso!";

            }
            return response;    
        }

        public async Task<ResponseModel<List<UsuarioListarDto>>> RemoverUsuario(int usuarioId)
        {
            ResponseModel<List<UsuarioListarDto>> response = new ResponseModel<List<UsuarioListarDto>>();

            using(var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var usuarioBanco = await connection.ExecuteAsync("DELETE FROM Usuarios WHERE id = @Id", new { Id = usuarioId });

                if(usuarioBanco == 0)
                {
                    response.Mensagem = "OCorreu um erro ao Remover um usuario";
                    response.Status = false;
                    return response;
                }

                var usuarios = await ListarUsuarios(connection);

                var usuariosMapeados = _mapper.Map<List<UsuarioListarDto>>(usuarios);

                response.Dados = usuariosMapeados;
                response.Mensagem = "Usuários Listados com sucesso";
            }
            return response;
        }
    }
}
