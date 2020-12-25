using System.Collections.Generic;

namespace GodnessChatBot
{
    public interface IProcess
    {
        Telegramma Start(string id, object obj);
        Telegramma Execute(string id, string message);
        Telegramma Finish(string id);
    }
}