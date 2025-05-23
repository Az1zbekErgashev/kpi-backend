namespace Kpi.Domain.Models.User
{
    public class AuthModel
    {
        public string Token { get; set; }
        public UserModel User { get; set; }

        public virtual AuthModel MapFromEntity(string token, Domain.Entities.User.User user)
        {
            Token = token;
            User = new UserModel().MapFromEntity(user);
            return this;
        }
    }
}
