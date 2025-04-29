namespace JWTAuthDotnet9.Entities
{
    public class User
    {
        public string Username { get; set; } = string.Empty;

        public string HashedPassword { get; set; } = string.Empty;
    }
}
