using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Controls.Primitives;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CloudLib.Client.UI.Controls
{
    [TemplatePart(Name = LoginButtonPart, Type = typeof(Button)),
     TemplatePart(Name = LogoutButtonPart, Type = typeof(ButtonBase))]
    public partial class LoginButton : Control
    {
        private const string LoginButtonPart = "PART_LoginButton";
        private const string LogoutButtonPart = "PART_LogoutButton";

        private Button _loginuButton;
        private ButtonBase _logoutButton;

        private bool _isLoading;

        public LoginButton()
        {
            this.DefaultStyleKey = typeof(LoginButton);
        }
    }
}
