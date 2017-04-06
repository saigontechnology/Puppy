# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

```c#
public DbSet<UserEntity> UserEntities { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    Console.WriteLine($"{nameof(DbContext)} is Created", nameof(OnModelCreating));

    modelBuilder.AddConfiguration(new UserEntityMapping());

    // Convention Table Name is Entity Name without EntityMapping Postfix
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        entity.Relational().TableName = entity.DisplayName().Replace(nameof(EntityMapping), string.Empty);
        Console.WriteLine($"Table {entity.Relational().TableName} is Created", nameof(DbContext));
    }

    base.OnModelCreating(modelBuilder);
}

// In userEntity.cs
public class UserEntity : IdentityUserEntityBase
{
    public UserEntity()
    {
    }

    public UserEntity(string userName) : base(userName)
    {
    }
}

// In UserEntityMapping.cs

public class UserEntityMapping : EntityTypeConfiguration<UserEntity>
{
    public override void Map(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable(nameof(UserEntityMapping));
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Key).IsRequired();
        builder.Property(x => x.Version).IsRowVersion();
    }

```