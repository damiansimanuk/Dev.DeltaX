namespace DeltaX.GenericReportDb.Controllers.Models
{
    public class EditUserModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
