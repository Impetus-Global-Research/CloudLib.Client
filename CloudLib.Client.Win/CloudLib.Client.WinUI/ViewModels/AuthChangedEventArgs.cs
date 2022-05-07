using System;

namespace CloudLib.Client.WinUI.ViewModels
{
    public class AuthChangedEventArgs : EventArgs
    {
        public AuthChangedEventArgs(bool oldValue, bool value)
        {
            this.OldValue = oldValue;
            this.IsLoggedIn = value;
        }

        public bool OldValue { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}