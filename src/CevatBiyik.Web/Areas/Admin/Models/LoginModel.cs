using System.ComponentModel.DataAnnotations;

namespace CevatBiyik.Web.Areas.Admin.Models;

public sealed class LoginModel
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur."), StringLength(256)]
    public string UserName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Şifre zorunludur."), StringLength(256)]
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
}
