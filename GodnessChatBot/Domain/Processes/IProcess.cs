using System.Collections.Generic;

namespace GodnessChatBot
{
    public interface IProcess
    {
        Information Execute(string id, string message);
        Information Finish(string id);
    }
}