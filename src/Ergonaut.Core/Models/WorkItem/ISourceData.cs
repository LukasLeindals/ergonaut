namespace Ergonaut.Core.Models.WorkItem
{
    public interface ISourceData
    {
        DateTime UpdatedAt { get; }

        string? ToJson();
        ISourceData FromJson(string json);
    }

}
