using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Windows_Personal_Assistant
{
    class Controller: ServerConnection
    {
        
        public Controller()
        {

        }

        public async Task<string> Get_TasksAsync()
        {
            var result = await Post_DataAsync(null, "task", "get");
            return result;
        }

        public async Task<string> Get_NotesAsync()
        {
            var result = await Post_DataAsync(null, "note", "get");
            return result;
        }

        public async Task<string> Get_EventsAsync()
        {
            var result = await Post_DataAsync(null, "event", "get");
            return result;
        }

        

    }
}
