using Business.Models;

namespace Business.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Gets login credentials and if everything is ok returns it
        /// </summary>
        /// <param name="login">login credentials</param>
        /// <returns></returns>
        //TODO change this shit
        Task<LoginDto> Check(LoginDto login);
        /// <summary>
        /// Gets register credentials and if everithing is ok, registers user, and returns it
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        Task<LoginDto> Register(RegisterDto register);
        /// <summary>
        /// Gets user's username, public key, and if user is connected to chat hub, isOnline is true
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<UserInfo> GetInfo(string username);
        /// <summary>
        /// if user exist in db, returns true
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<bool> UserExist(string username);
        Task Delete(string username);
    }
}
