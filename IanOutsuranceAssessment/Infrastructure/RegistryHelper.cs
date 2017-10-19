using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    public static class RegistryHelper
    {
        public static string GetListSeparatorCharacter()
        {
            string listSeparatorCharacter = String.Empty;

            listSeparatorCharacter = Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\International", "sList", ";").ToString();
            return listSeparatorCharacter;
        }
    }
}
