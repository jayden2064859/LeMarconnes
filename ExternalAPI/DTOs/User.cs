namespace ExternalAPI.Classes
{
    public class UserResponseDTO
    {
        public int id { get; set; }
        public string username { get; set; }
        public string passwordHash { get; set; }
        public string email { get; set; }
        public string role { get; set; }
    }

}
