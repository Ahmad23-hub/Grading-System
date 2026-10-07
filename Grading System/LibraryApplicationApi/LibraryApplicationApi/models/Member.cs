namespace LibraryApplicationApi.models
{
    public class Member
    {
        public int id { get; set; } 
        public string FullName { get; set; } = "SANNI SAEED AHMAD";
        public String Email { get; set; } = "sannisaeedahmad@gmail.com";
        public String PhoneNumber { get; set; } = "08102532016";
        public DateTime  DateTimeRegistrationDate { get; set; } = new DateTime (2026, 10, 06);
    }
}
