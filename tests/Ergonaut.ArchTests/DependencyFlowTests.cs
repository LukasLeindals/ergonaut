using System.Linq;
using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace Ergonaut.ArchTests;

public sealed class DependencyFlowTests
{
    private static readonly string InfrastructureNamespace = "Ergonaut.Infrastructure";
    private static readonly string AppNamespace = "Ergonaut.App";
    private static readonly string UiNamespace = "Ergonaut.UI";

    private static readonly Assembly ApiAssembly = typeof(Api.Controllers.ProjectsController).Assembly;
    private static readonly Assembly AppAssembly = typeof(App.Features.Projects.ProjectService).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.Repositories.ProjectRepository).Assembly;

    [Fact(DisplayName = "API controllers stay isolated from infrastructure")]
    public void ApiControllers_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types.InAssembly(ApiAssembly)
            .That()
            .ResideInNamespace("Ergonaut.Api.Controllers")
            .Should()
            .NotHaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, DescribeFailures(result));
    }

    [Fact(DisplayName = "Application layer does not take an infrastructure dependency")]
    public void App_Should_Not_Depend_On_Infrastructure()
    {
        var result = Types.InAssembly(AppAssembly)
            .That()
            .ResideInNamespaceStartingWith(AppNamespace)
            .Should()
            .NotHaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, DescribeFailures(result));
    }

    [Fact(DisplayName = "Infrastructure remains unaware of App and UI layers")]
    public void Infrastructure_Should_Not_Depend_On_App_Or_Ui()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .ResideInNamespaceStartingWith(InfrastructureNamespace)
            .Should()
            .NotHaveDependencyOnAny(AppNamespace, UiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, DescribeFailures(result));
    }

    private static string DescribeFailures(TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : "Violations:\n" + string.Join(Environment.NewLine, result.FailingTypes.Select(type => $" - {type.FullName}"));
}
