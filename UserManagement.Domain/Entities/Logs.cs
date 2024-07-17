namespace UserManagement.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        //for assumption of admin
        public string ActionBy { get; set; }
        //if implemeny proper user after Login
        //public int UserId {  get; set; }
        //public User User { get; set; }
        public DateTime Time { get; set; }
    }
}
