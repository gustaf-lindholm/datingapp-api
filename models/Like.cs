namespace DatingApp.API.models
{
    public class Like
    {
        // user ID's
        public int LikerId { get; set; }

        // user beeing liked 
        public int LikeeId { get; set; }
        
        public User Liker { get; set; }

        public User Likee { get; set; }
    }
}