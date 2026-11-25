using CourseRegistration.Core.Entities;
using CourseRegistration.Data.Repositories;
using CourseRegistration.Data.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CourseRegistration.Tests;

public class OperationCancellationTests
{
    [Fact]
    public async Task RegistrationTransaction_PreCancelledTokenStopsBeforeClearingTracking()
    {
        await using var context = TestDbContextFactory.Create(enableRetries: true);
        var category = await context.Categories.FirstAsync();
        var repository = new RegistrationRepository(context);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repository.ExecuteInSerializableTransactionAsync(
                _ => throw new InvalidOperationException("The operation should not run."),
                cancellation.Token));

        Assert.Equal(EntityState.Unchanged, context.Entry(category).State);
        Assert.Null(context.Database.CurrentTransaction);
    }

    [Fact]
    public async Task RegistrationTransaction_CancellationBeforeCommitRollsBackSavedChanges()
    {
        await using var context = TestDbContextFactory.Create(enableRetries: true);
        var repository = new RegistrationRepository(context);
        using var cancellation = new CancellationTokenSource();
        var name = Guid.NewGuid().ToString("N");

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repository.ExecuteInSerializableTransactionAsync(async token =>
            {
                Assert.Equal(cancellation.Token, token);
                context.Categories.Add(new Category { Name = name });
                await repository.SaveChangesAsync(token);
                cancellation.Cancel();
            }, cancellation.Token));

        await using var verification = TestDbContextFactory.Create();
        Assert.False(await verification.Categories.AnyAsync(category => category.Name == name));
    }

    [Fact]
    public async Task AdminSeeder_ObservesCancellationWhenCheckingForAnAdmin()
    {
        await using var context = TestDbContextFactory.Create();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["AdminSeed:Email"] = "cancelled-seed@example.test",
                ["AdminSeed:Password"] = Guid.NewGuid().ToString("N"),
                ["AdminSeed:FullName"] = "Cancelled seed"
            }).Build();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            AdminSeeder.SeedAsync(context, new PasswordHasher<User>(), configuration,
                isDevelopment: false, cancellation.Token));
    }
}
