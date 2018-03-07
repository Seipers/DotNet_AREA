namespace Area.Models
{
    public class FormLogin
    {
        public string username { get; set; }
        public string password { get; set; }
    }
    public class FormRegister
    {
        public string username { get; set; }
        public string password { get; set; }
        public string confirmation { get; set; }  
        public string email { get; set; }      
    }
    public class FormCode
    {
        public string code { get; set; }
    }
}