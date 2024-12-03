using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options=>options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"])); 
builder.Services.AddIdentityCore<AppUser>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options=>{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options=>{
    var secret = builder.Configuration["JwtConfig:Secret"]; 
    Console.Write(secret + "SECRET");
    var issuer = builder.Configuration["JwtConfig:ValidIssuer"]; 
    var audience = builder.Configuration["JwtConfig:ValidAudience"]; 
    if(secret is null || issuer is null || audience is null){
        throw new ApplicationException("Jwt is not set in the configuration"); 
    }
    options.SaveToken = true; 
    options.RequireHttpsMetadata = false; 
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters(){
        ValidateIssuer = true, 
        ValidateAudience = true, 
        ValidAudience = audience, 
        ValidIssuer = issuer, 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Description = "JWT Authorization header using the Bearer scheme", 
        Name = "Authorization", 
        In = ParameterLocation.Header, 
        Type = SecuritySchemeType.ApiKey, 
        Scheme = "Bearer"
    }); 
    c.AddSecurityRequirement(new OpenApiSecurityRequirement{{
        new OpenApiSecurityScheme{
            Reference = new OpenApiReference{
                Type = ReferenceType.SecurityScheme, 
                Id = "Bearer"
            }
        }, 
        new string[]{}
    }});
});

var app = builder.Build();
app.UseAuthentication(); 
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
