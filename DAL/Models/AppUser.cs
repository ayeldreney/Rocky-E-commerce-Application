using Microsoft.AspNetCore.Identity;

namespace Rocky.DAL.Models;
public class AppUser : IdentityUser
{
    public string FirstName { get; internal set; }
    public string LastName { get; internal set; }
}