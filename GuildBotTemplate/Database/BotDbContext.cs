using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace GuildBotTemplate.Database;

// For integrate with dotnet-ef
public class BotDbContextFactory : IDesignTimeDbContextFactory<BotDbContext> {
    public BotDbContext CreateDbContext(string[] args) {
        return new BotDbContext(BotDbContext.DbOptions);
    }
}

public class BotDbContext : DbContext {
    internal static readonly DbContextOptions<BotDbContext> DbOptions = new DbContextOptionsBuilder<BotDbContext>()
        .UseNpgsql(App.PostgreSqlConnectionString)
        //.UseLoggerFactory(App.LogFactory)
        //.LogTo(a => Debug.WriteLine(a))
        .Options;

    private static readonly PooledDbContextFactory<BotDbContext> ContextFactory = new(DbOptions);

    public DbSet<JsonSingleton> JsonSingletons { get; set; }

    public static BotDbContext Get() {
        return ContextFactory.CreateDbContext();
    }

    public static async Task<BotDbContext> GetAsync() {
        return await ContextFactory.CreateDbContextAsync();
    }

    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }

    public async Task<T> GetSingleton<T>() {
        var ret = await JsonSingletons.FirstOrDefaultAsync(a => a.Name == typeof(T).Name);
        return ret == null ? default : ret.Value.Deserialize<T>();
    }

    public async Task SaveSingleton<T>(T value) {
        var v = await JsonSingletons.FindAsync(value.GetType().Name);
        if (v == null) {
            v = new JsonSingleton {
                Name = value.GetType().Name,
                Value = JsonSerializer.SerializeToDocument(value)
            };
            JsonSingletons.Add(v);
        } else {
            v.Value = JsonSerializer.SerializeToDocument(value);
            Update(v);
        }
        await SaveChangesAsync();
    }
}