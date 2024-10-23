using task_management.src.API.Interface;
using task_management.src.API.Model;
using task_management.src.API.View.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using task_management.src.API.Repository;

namespace task_management.src.API.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _repository = userRepository;
            _passwordHasher = passwordHasher;
        }

        // Método para criar um novo usuário
        public async Task<User> CreateUserAsync(UserRequest request)
        {
            // Verifica se o usuário já existe
            var existingUser = await _repository.GetByUsernameOrEmailAsync(request.Username, request.Email);
            if (existingUser != null)
            {
                throw new Exception("User already exists.");
            }

            // Cria um novo usuário
            var user = new User
            {
                Username = request.Username,
                Email = request.Email
            };

            // Hash da senha
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            await AddAsync(user);

            return user;
        }

        // Adiciona um novo usuário
        public async Task<User> AddAsync(User user)
        {
            return await _repository.AddAsync(user);
        }

        // Implementação do método para deletar um usuário passando a entidade
        public async Task<bool> DeleteAsync(User entity)
        {
            return await _repository.DeleteAsync(entity);
        }

        // Implementação do método para deletar um usuário pelo id
        public async Task<bool> DeleteByIdAsync(int id)
        {
            return await _repository.DeleteByIdAsync(id);
        }

        // Retorna todos os usuários
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Busca um usuário pelo id
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // Atualiza um usuário existente
        public async Task<User> UpdateAsync(User entity)
        {
            var existingUser = await _repository.GetByIdAsync(entity.Id);

            if (existingUser == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            // Verifica se o username ou email já estão sendo utilizados por outro usuário
            var existingUserByEmailOrUsername = await _repository.GetByUsernameOrEmailAsync(entity.Username, entity.Email);
            if (existingUserByEmailOrUsername != null && existingUserByEmailOrUsername.Id != entity.Id)
            {
                throw new Exception("Outro usuário já utiliza esse nome de usuário ou email.");
            }

            // Atualiza os dados do usuário
            existingUser.Username = entity.Username;
            existingUser.Email = entity.Email;

            if (!string.IsNullOrEmpty(entity.PasswordHash))
            {
                existingUser.PasswordHash = _passwordHasher.HashPassword(existingUser, entity.PasswordHash);
            }

            return await _repository.UpdateAsync(existingUser);
        }

        public async Task<User> GetByUsernameOrEmailAsync(string username, string email)
        {
            var user = await _repository.GetByUsernameOrEmailAsync(username, email);

            if (user == null)
            {
                throw new Exception("Usuário não encontrado.");
            }

            return user;
        }
    }
}
