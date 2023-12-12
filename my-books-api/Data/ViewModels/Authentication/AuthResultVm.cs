namespace my_books_api.Data.ViewModels.Authentication
{
    public class AuthResultVm
    {
        public string Token { get; set; } 
        public string RefreshToken { get; set; } 
        public DateTime ExpirestAt { get; set; }    

    }
}
