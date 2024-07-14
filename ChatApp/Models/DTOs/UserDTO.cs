namespace ChatApp.Models.DTOs
{
    public class UserDTO
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public DateTime? createAt { get; set; }
        public Guid? userTypeId { get; set; }
    }
}
