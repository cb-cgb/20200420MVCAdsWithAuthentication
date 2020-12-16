using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Linq;



namespace AdsAuth.Data
{
    public class DBManager
    {
        private string _conn;

        public DBManager(string conn)
        {
            _conn = conn;
        }


        //hash the entered pw and add the user and hashed pw to the db
        public void AddUser(User u)
        {
            var user = GetUserByEmail(u.Email);

            if (user is null) //user doesn't exist yet 
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(u.PasswordHash);

                using (var conn = new SqlConnection(_conn))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Users (email, name, passwordhash)
                                    SELECT @email, @name, @password select scope_identity()";
                    cmd.Parameters.AddWithValue("@email", u.Email);
                    cmd.Parameters.AddWithValue("@name", u.Name);
                    cmd.Parameters.AddWithValue("@password", hash);
                    conn.Open();
                    int userId = (int)(decimal)cmd.ExecuteScalar();

                }
            }
        }

        public User GetUserByEmail(string email)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"select * from users where email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (!reader.Read())
                {
                    return null;
                }
                return new User
                {
                    UserId = (int)reader["Id"],
                    Email = (String)reader["email"],
                    Name = (string)reader["name"],
                    PasswordHash = (string)reader["passwordhash"]
                };

            }
        }

         public User Login (User u)
        {
                var  user = GetUserByEmail(u.Email);

                  if (user is null) //didn't find a user with that email, invalid email
                    {
                        return null;
                    }
                   
                  if (u.PasswordHash is null)
                  {
                     return null;
                   }
                    bool isValidPassword = BCrypt.Net.BCrypt.Verify(u.PasswordHash,user.PasswordHash); 
                    if (!isValidPassword) //pw doesn't matches the pw in the db
                    {
                        return null;
                    }                    
                
                return user; //pw matched the db hash pw
            }

        public List<Ad> GetAds()
        {
            var  Ads = new List<Ad>();
            using (var conn = new SqlConnection(_conn))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"select a.* , name UserName from Ads a, users u where a.userId = u.Id";
                conn.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Ads.Add(new Ad
                    {
                        Id = (int)reader["Id"],
                        Title = (string)reader["Title"],
                        Phone = (string)reader["Phone"],
                        Text = (string)reader["Text"],
                        DatePosted = (DateTime)reader["DatePosted"],
                        UserId = (int)reader["UserId"],
                        PostedBy = (string)reader["UserName"]
                    });
                }
            }
                return Ads;
        }

        public IEnumerable<Ad> GetAdsById(int Id)
        {
            List<Ad> Ads = GetAds();
            if (Ads is  null) 
            {
                return null;
            }
            
            return GetAds().Where(a => a.UserId == Id);       
        }

        public int AddAd (Ad Ad)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Ads (Title, Phone, Text, UserId)
                                    select @title, @phone, @text, @userId select scope_identity()";

                cmd.Parameters.AddWithValue("@title", Ad.Title);
                cmd.Parameters.AddWithValue("@phone", Ad.Phone);
                cmd.Parameters.AddWithValue("@text", Ad.Text);
                cmd.Parameters.AddWithValue("@userId", Ad.UserId);

                conn.Open();
                return  (int)(decimal)cmd.ExecuteScalar();
            }            
        }

        public void DeleteAd(int Id)
        {
            using (var conn = new SqlConnection(_conn))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"DELETE FROM  Ads
                                    WHERE Id = @Id";
                cmd.Parameters.AddWithValue("@Id", Id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
