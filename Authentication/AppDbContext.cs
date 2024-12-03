

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration):IdentityDbContext<AppUser>(options){
 
}
