namespace Area.Models
{
    public class User_role
    {
        public int    id { get; set; } 
        public int    fk_user_id {get; set;}
        public string role {get; set;}
    }
}