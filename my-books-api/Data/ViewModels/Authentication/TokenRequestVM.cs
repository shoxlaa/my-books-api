namespace my_books_api.Data.ViewModels.Authentication
{
    public class TokenRequestVM
    {
        public string Token { get; set; }= string.Empty;
        public string RefreshToken { get; set; } = string.Empty;    
    }
}
