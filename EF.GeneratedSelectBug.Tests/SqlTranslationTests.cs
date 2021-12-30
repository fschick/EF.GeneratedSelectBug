using EF.GeneratedSelectBug.DbContexts;
using EF.GeneratedSelectBug.Models;
using EF.GeneratedSelectBug.Tests.Logger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EF.GeneratedSelectBug.Tests;

[TestClass]
[DoNotParallelize]
public class SqlTranslationTests
{
    private const string CUSTOMER_A_ID = "E32652E1-4E0B-4602-BDEF-59D0DA798BE0";
    private const string CUSTOMER_B_ID = "B52572D3-B8A3-42EE-8CBE-B7CFD66B80CF";

    private const string CUSTOMER_A_NAME = "Customer A";
    private const string CUSTOMER_B_NAME = "Customer B";

    private const string PROJECT_A_ID = "C4B833D5-2B51-4563-901E-A10FAC789EC8";
    private const string PROJECT_B_ID = "E391705B-4204-4658-9BB5-ECD226F6EBCD";

    private const string ORDER_A1_ID = "3C9B1D98-BB88-48EA-8385-376191F63CBC";
    private const string ORDER_A2_ID = "A70884A3-AB9F-4BA5-8661-739D76E71010";
    private const string ORDER_B1_ID = "C4283FF7-1E9B-4E9C-B355-4BA9AB5D658F";

    private const string TIME_SHEET_A_ID = "DCBE0A9A-6C97-4276-937F-049E708DAB87";
    private const string TIME_SHEET_B_ID = "E6AFD937-31E6-45FC-A32C-382C23323E10";

    [TestMethod]
    public async Task WhenGroupWithMinIsUsed_CustomerIdAndTitleMatches()
    {
        var serviceProvider = await CreateAndConfigureServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<TimeTrackingDbContext>();

        var timSheets = await dbContext
            .Set<TimeSheet>()
            .Where(x => x.OrderId != null)
            .GroupBy(x => x.OrderId)
            .Select(x => new
            {
                HourlyRate = x.Min(f => f.Order.HourlyRate),
                CustomerId = x.Min(f => f.Project.Customer.Id),
                CustomerName = x.Min(f => f.Project.Customer.Name),
            })
            .ToListAsync();

        var customerAName = timSheets.Single(x => x.CustomerId == new Guid(CUSTOMER_A_ID)).CustomerName;
        var customerBName = timSheets.Single(x => x.CustomerId == new Guid(CUSTOMER_B_ID)).CustomerName;

        Assert.AreEqual(CUSTOMER_A_NAME, customerAName, $"Used SQL: {TestLogger.Logs.Last()}");
        Assert.AreEqual(CUSTOMER_B_NAME, customerBName, $"Used SQL: {TestLogger.Logs.Last()}");
    }

    [TestMethod]
    public async Task WhenSubSelectIsUsed_MinHourlyRatePerCustomerIsSelectedProperly()
    {
        var serviceProvider = await CreateAndConfigureServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<TimeTrackingDbContext>();

        Expression<Func<Order, bool>> someFilterFromOutside = x => x.Number != "A1";

        var orders = await dbContext
            .Set<Order>()
            .Where(someFilterFromOutside)
            .GroupBy(x => new { x.CustomerId, x.Number })
            .Select(x => new
            {
                x.Key.CustomerId,
                CustomerMinHourlyRate = dbContext.Set<Order>().Where(n => n.CustomerId == x.Key.CustomerId).Min(h => h.HourlyRate),
                HourlyRate = x.Min(f => f.HourlyRate),
                Count = x.Count()
            })
            .ToListAsync();

        var customerAMinHourlyRate = orders.Single(x => x.CustomerId == new Guid(CUSTOMER_A_ID)).CustomerMinHourlyRate;
        var customerBMinHourlyRate = orders.Single(x => x.CustomerId == new Guid(CUSTOMER_B_ID)).CustomerMinHourlyRate;

        Assert.AreEqual(10, customerAMinHourlyRate, $"Used SQL: {TestLogger.Logs.Last()}");
        Assert.AreEqual(20, customerBMinHourlyRate, $"Used SQL: {TestLogger.Logs.Last()}");
    }

    private static async Task<IServiceProvider> CreateAndConfigureServiceProvider()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddProvider(new Logger.SqlTranslationTests.TestLoggerProvider()))
            .AddDbContext<TimeTrackingDbContext>()
            .BuildServiceProvider();

        var dbContext = serviceProvider.GetRequiredService<TimeTrackingDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
        await SeedTestData(dbContext);
        TestLogger.Logs.Clear();

        return serviceProvider;
    }

    private static async Task SeedTestData(DbContext dbContext)
    {
        var customerA = new Customer { Id = new Guid(CUSTOMER_A_ID), Name = CUSTOMER_A_NAME };
        var customerB = new Customer { Id = new Guid(CUSTOMER_B_ID), Name = CUSTOMER_B_NAME };

        var projectA = new Project { Id = new Guid(PROJECT_A_ID), Customer = customerA };
        var projectB = new Project { Id = new Guid(PROJECT_B_ID), Customer = customerB };

        var orderA1 = new Order { Id = new Guid(ORDER_A1_ID), Number = "A1", Customer = customerA, HourlyRate = 10 };
        var orderA2 = new Order { Id = new Guid(ORDER_A2_ID), Number = "A2", Customer = customerA, HourlyRate = 11 };
        var orderB1 = new Order { Id = new Guid(ORDER_B1_ID), Number = "B1", Customer = customerB, HourlyRate = 20 };

        var timeSheetA = new TimeSheet { Id = new Guid(TIME_SHEET_A_ID), Order = orderA1, Project = projectA };
        var timeSheetB = new TimeSheet { Id = new Guid(TIME_SHEET_B_ID), Order = orderB1, Project = projectB };

        await dbContext.AddRangeAsync(customerA, customerB);
        await dbContext.AddRangeAsync(projectA, projectB);
        await dbContext.AddRangeAsync(orderA1, orderA2, orderB1);
        await dbContext.AddRangeAsync(timeSheetA, timeSheetB);
        await dbContext.SaveChangesAsync();
    }
}