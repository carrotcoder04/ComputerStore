using ComputerStore.Models;
using ComputerStore.Requests;
using Microsoft.EntityFrameworkCore;
namespace ComputerStore.Services
{
    public class UserService : BaseService
    {
        private DbSet<User> Users => Context.Users;

        public UserService(AppDbContext context) : base(context)
        {
        }
        public User CheckUserLogin(LoginRequest userRequest)
        {
            var user = GetUserByEmail(userRequest.Email);
            if (user.Password != userRequest.Password)
            {
                throw new("Sai mật khẩu vui lòng đăng nhập lại");
            }
            return user;
        }
        public User GetUserByEmail(string email)
        {
            return Users.FirstOrDefault(u => u.Email == email) ?? throw new($"Người dùng với email {email} không tồn tại");
        }
        public User GetUserById(int id)
        {
            return Users.FirstOrDefault(u => u.Id == id) ?? throw new($"Người dùng {id} không tồn tại");
        }
        public User Register(RegisterRequest userRequest)
        {
            if (!Users.Any(u => u.Email == userRequest.Email))
            {
                throw new($"Địa chỉ email đã tồn tại trên hệ thống");
            }
            var user = new User
            {
                Email = userRequest.Email,
                Password = userRequest.Password,
                Name = userRequest.Name,
                Phone = userRequest.Phone,
                Gender = userRequest.Gender,
                Address = userRequest.Address,
                Role = Role.USER
            };
            var newUser = Users.Add(user);
            Context.SaveChanges();
            return newUser.Entity;
        }
        public User UpdateUser(int id, UpdateUserRequest userRequest)
        {
            var user = GetUserById(id);
            user.Name = userRequest.Name;
            user.Gender = userRequest.Gender;
            user.Phone = userRequest.Phone;
            user.Address = userRequest.Address;
            var newUser = Users.Update(user);
            Context.SaveChanges();
            return newUser.Entity;
        }

        public void ChangePassword(int id, ChangePasswordRequest userRequest)
        {
            var user = GetUserById(id);
            if (user.Password != userRequest.CurrentPassword)
            {
                throw new("Mật khẩu hiện tại không đúng");
            }
            user.Password = userRequest.NewPassword;
            Users.Update(user);
            Context.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var user = GetUserById(id);
            Users.Remove(user);
            Context.SaveChanges();
        }
        public (IEnumerable<User> Users, int Total) GetAllUsers(int page, int pageSize)
        {
            var query = Users.AsQueryable();
            var total = query.Count();
            var users = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return (users, total);
        }
    }
}