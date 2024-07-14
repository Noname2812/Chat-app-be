namespace ChatApp.Data.Modals
{
    public class UserType
    {
        public int id { get; set; }
        public string name { get; set; }
        public string? description { get; set; }
        public DateTime createAt { get; set; }
        public DateTime modifiedDate { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }
}
