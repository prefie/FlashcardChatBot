namespace GodnessChatBot.Domain.DialogBranches
{
    public interface IDialogBranch
    {
        ReplyMessage Execute(string id, string message);
        ReplyMessage Finish(string id);
    }
}