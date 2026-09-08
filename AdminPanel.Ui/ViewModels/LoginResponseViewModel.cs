namespace AdminPanel.Ui.ViewModels
{
    public class LoginResponseViewModel
    {
        public bool HasError { get; set; }
        public string? ErrorMessage { get; set; }
        public LoginValueViewModel? Value { get; set; }
    }

    public class LoginValueViewModel
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;


        public int UserType { get; set; }
        public int? CharityId { get; set; }

    }
}
