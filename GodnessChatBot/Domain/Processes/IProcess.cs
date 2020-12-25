namespace GodnessChatBot.Domain.Processes
{
    public interface IProcess
    {
        Information Execute(string id, string message);
        Information Finish(string id);
    }
}