namespace DatingApp.API.models
{
  public class Like
  {
    // user ID's
    public int LikerId { get; set; }

    // user beeing liked 
    public int LikeeId { get; set; }

    public virtual User Liker { get; set; }

    public virtual User Likee { get; set; }
  }
}