
using System.ComponentModel.DataAnnotations;

public class LoginModel{
    public string UserName{get;set;} = string.Empty; 

    [Required]
    public string Password{get;set;} = string.Empty;
}