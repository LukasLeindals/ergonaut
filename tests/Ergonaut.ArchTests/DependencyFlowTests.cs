using System.Reflection;
using NetArchTest.Rules;
using Xunit;

namespace Ergonaut.ArchTests;

public sealed class DependencyFlowTests
{
    private static readonly string InfrastructureNamespace = "Ergonaut.Infrastructure";
    private static readonly string AppNamespace = "Ergonaut.App";
    private static readonly string UiNamespace = "Ergonaut.UI";
    private static readonly string ApiNamespace = "Ergonaut.Api";

    [Fact(DisplayName = "API controllers stay isolated from infrastructure")]
    public void ApiControllers_Should_Not_Depend_On_Infrastructure()
    {
        CheckAssembly(typeof(Api.Controllers.ProjectsController).Assembly, ApiNamespace, InfrastructureNamespace);
    }

    [Fact(DisplayName = "Application layer does not take an infrastructure dependency")]
    public void App_Should_Not_Depend_On_Infrastructure()
    {
        CheckAssembly(typeof(App.Services.ProjectService).Assembly, AppNamespace, InfrastructureNamespace);
    }

    [Fact(DisplayName = "UI layer does not take an infrastructure dependency")]
    public void Ui_Should_Not_Depend_On_Infrastructure()
    {
        CheckAssembly(typeof(UI.Components.Pages.Projects.Projects).Assembly, UiNamespace, InfrastructureNamespace);
        CheckAssembly(typeof(UI.Components.Pages.WorkItems.WorkItems).Assembly, UiNamespace, InfrastructureNamespace);
    }

    [Fact(DisplayName = "Infrastructure remains unaware of App and UI layers")]
    public void Infrastructure_Should_Not_Depend_On_App_Or_Ui()
    {
        CheckAssembly(typeof(Infrastructure.Repositories.ProjectRepository).Assembly, InfrastructureNamespace, AppNamespace, UiNamespace);
    }

    [Fact(DisplayName = "DTO contracts stay decoupled from infrastructure")]
    public void Contracts_Should_Not_Depend_On_Infrastructure()
    {
        CheckAssembly(typeof(App.Models.ProjectRecord).Assembly, "Ergonaut.App.Models", InfrastructureNamespace);
    }

    /// <summary>
    /// Checks that types in the specified namespace of the given assembly do not depend on any of the forbidden dependencies.
    /// NOTE: The assembly can be found via a type within it, e.g. typeof(SomeTypeInAssembly).Assembly
    /// </summary>
    /// <param name="assembly">The assembly to check.</param>
    /// <param name="namespace">The namespace to check within the assembly.</param>
    /// <param name="forbiddenDependencies">The namespaces that should not be depended upon.</param>
    private static void CheckAssembly(Assembly assembly, string @namespace, params string[] forbiddenDependencies)
    {
        var result = Types.InAssembly(assembly)
            .That()
            .ResideInNamespaceStartingWith(@namespace)
            .Should()
            .NotHaveDependencyOnAny(forbiddenDependencies)
            .GetResult();

        Assert.True(result.IsSuccessful, DescribeFailures(result));
    }

    private static string DescribeFailures(TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : "Violations:\n" + string.Join(Environment.NewLine, result.FailingTypes.Select(type => $" - {type.FullName}"));
}
