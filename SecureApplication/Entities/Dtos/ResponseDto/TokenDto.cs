namespace Entities.Dtos.ResponseDto
{
    public class TokenDto
    {
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
    }
}