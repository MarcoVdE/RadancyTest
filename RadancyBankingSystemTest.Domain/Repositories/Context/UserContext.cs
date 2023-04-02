using System.Diagnostics.CodeAnalysis;

namespace RadancyBankingSystemTest.Domain.Repositories.Context;

using Microsoft.EntityFrameworkCore;
using Models;

[ExcludeFromCodeCoverage(Justification = "This is just for the DB example, usually tied to SQL DB")]
public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; } = null!;
}