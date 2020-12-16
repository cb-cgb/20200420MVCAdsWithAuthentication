using System;
using System.Data.SqlClient;

namespace AdsAuth.Data
{
    public class User
    {
        public int UserId { get; set; }        
        public String Name { get; set; }
        public String Email { get; set; }
        public String PasswordHash { get; set; }
    }

    public class Ad
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public String? Phone { get; set; }
        public String Text { get; set; }
        public DateTime DatePosted { get; set; }
        public int UserId { get; set; }
        public string PostedBy { get; set; }
    }
    
    
}
