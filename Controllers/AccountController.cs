

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("[controller]")]
public class AccountController(UserManager<AppUser> userManager, IConfiguration _configuration):ControllerBase{

private string GenerateToken(string userName){
    var secret = _configuration["JwtConfig:Secret"]; 
    var issuer = _configuration["JwtConfig:ValidIssuer"]; 
    var audience = _configuration["JwtConfig:ValidAudience"]; 
    if(secret is null || issuer is null | audience is null){
        throw new ApplicationException("Jwt is not set in the configuration");
    }
    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)); 
    var tokenHandler = new JwtSecurityTokenHandler(); 
    var tokenDescriptor = new SecurityTokenDescriptor{
        Subject= new ClaimsIdentity( new [] {
            new Claim(ClaimTypes.Name, userName)
        }), 
        Expires = DateTime.UtcNow.AddDays(1), 
        Issuer = issuer, 
        Audience = audience, 
        SigningCredentials = new SigningCredentials(signingKey,SecurityAlgorithms.HmacSha256Signature)
    }; 
    var securityToken = tokenHandler.CreateToken(tokenDescriptor); 
    var token = tokenHandler.WriteToken(securityToken); 
    return token;

}
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] AddOrUpdateAppUserModel model){
    // check if the model is valid 
    if(ModelState.IsValid){
        var existedUser = await userManager.FindByNameAsync(model.UserName); 
        if(existedUser !=null){
            ModelState.AddModelError("","User name is already taken"); 
            return BadRequest(ModelState); 
        }
        // create a new user object 
        var user = new AppUser(){
            UserName = model.UserName, 
            Email = model.Email, 
            SecurityStamp = Guid.NewGuid().ToString()
        }; 
        // save the user 
        var result = await userManager.CreateAsync(user,model.Password); 
        if(result.Succeeded){
            var token = GenerateToken(model.UserName); 
            return Ok(new {token}); 
        }
        foreach(var error in result.Errors){
            ModelState.AddModelError("",error.Description);
        }
    }
    return BadRequest(ModelState);
}

[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginModel model){
    if(ModelState.IsValid){
        var user = await userManager.FindByNameAsync(model.UserName);
        if(user!=null){
            if(await userManager.CheckPasswordAsync(user,model.Password)){
                var token = GenerateToken(model.UserName); 
                return Ok(new {token});
            }
        }
    }
    return BadRequest(ModelState);
}


}
