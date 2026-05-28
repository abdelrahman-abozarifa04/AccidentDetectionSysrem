using AccidentDetectionSystem.efmodels;

namespace AccidentDetectionSysrem.modelsef
{
    public class user
    {
        public int UserId { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginTime { get; set; }
        public virtual List<UserNotifiction> UserNotifi { get; } = new List<UserNotifiction>();





    }
}
