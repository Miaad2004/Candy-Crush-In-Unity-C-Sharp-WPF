using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandyCrush.UI.Controls
{
    public class UserEventArgs : EventArgs
    {
        public string Username { get; }

        public UserEventArgs(string username)
        {
            Username = username;
        }
    }
}
