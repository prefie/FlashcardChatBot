namespace GodnessChatBot.Domain.Processes
{
    public interface IDialogBranch
    {
        ReplyMessage Execute(string id, string message);
        ReplyMessage Finish(string id);
    }
}