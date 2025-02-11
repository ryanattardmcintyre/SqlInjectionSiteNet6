namespace SqlInjectionVulnerable.Models
{
    
        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }  // ⚠ Storing plaintext passwords is bad practice!

            public int Mobile{ get; set; }
        }
    

}
