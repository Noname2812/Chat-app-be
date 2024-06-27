namespace ChatApp.Models.RequestModels
{
    public class UserRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public string? OldPassword { get; set; }
        public string? newPassword { get; set; }
    }
}
