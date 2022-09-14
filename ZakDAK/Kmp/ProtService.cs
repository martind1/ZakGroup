using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using System.Text.Json;
using ZakDAK.Shared;

namespace ZakDAK.Kmp
{
    /// <summary>
    /// Service für Protokollierung und Statusmeldung
    /// </summary>
    public class ProtService : ComponentBase
    {
#region Statustext, Eventconsole
        public string StatusText { get; set; } = "Statustext";
        public event Action OnStatusTextChange;
        private void StatusTextChanged() => OnStatusTextChange?.Invoke();

        public EventConsole console;


        public void SMess(String Text)
        {
            StatusText = Text;
            //Ereignis zum Anzeigen woanders auslösen:
            StatusTextChanged();
        }

        //Statuszeile mit Object->JSON
        public void SMess(object value)
        {
            SMess(JsonSerializer.Serialize(value));
        }


        //Statuszeile und Protlist
        public void SMessL(String Text)
        {
            SMess(Text);
            if (console != null)
                console.Log(Text);
            else
                Debug0();
        }

        //Logfile und Statuszeile

        public void Debug0()
        {
            //macht nix. Nur für Brakpoint
        }

#endregion

    }
}