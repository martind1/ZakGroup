using NuGet.Protocol;
using System.Security.Policy;

namespace ZakDAK.Kmp
{
    [Flags]
    public enum ColumnListItemFlags
    {
        Speicherwert = 1, //S=Auswahlfeld: Speicherwert (statt Anzeigewert) anzeigen
        Summe = 2,        //M=Spalte summieren
        OptiBreite = 4,   //O=Optimale Breite 
        MaxBreite = 8,    //X=Maximale Breite (in letzter Spalte)
        Hilfetext = 16,   //H=vollständigen Text in Statuszeile anzeigen

    }

    // Beschreibung einer Spalte
    public class ColumnListItem
    {
        public string DisplayLabel { get; set; }
        public int DisplayWidth { get; set; } = 0;
        public bool IsVisible
        {
            get => DisplayWidth > 0;
            set
            {
                if (value)
                {
                    if (DisplayWidth == 0)
                        DisplayWidth = 8;
                }
                else
                {
                    if (DisplayWidth > 0)
                        DisplayWidth = 0;
                }
            }
        }
        public ColumnListItemFlags Flags { get; set; }

        //Spalte anhand KMP Beschreibung anlegen idF <Display>:<Width>,<[Option]*>
        public ColumnListItem(string ColDesc)
        {
            var SL1 = ColDesc.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (SL1.Length >= 1)
            { 
                var SL2 = SL1[0].Split(':');
                DisplayLabel = SL2[0];
                if (SL2.Length >= 2)
                    DisplayWidth = int.Parse(SL2[1]);
                foreach (var option in SL1[1..])
                {
                    ColumnListItemFlags flags = option switch
                    {
                        "S" => ColumnListItemFlags.Speicherwert,
                        "M" => ColumnListItemFlags.Summe,
                        "O" => ColumnListItemFlags.OptiBreite,
                        "X" => ColumnListItemFlags.MaxBreite,
                        "H" => ColumnListItemFlags.Hilfetext,
                        _ => throw new NotImplementedException()
                    };
                    Flags |= flags;
                }
            }
        }
    }

    public class ColumnList
    {
        //idF <ColDesc>=<FieldName>
        public IDictionary<string, ColumnListItem> Columns { get; set; }

        public ColumnList()
        {
            Columns = new Dictionary<string, ColumnListItem>();
        }

        public ColumnList(string columnlist) : base()   
        {
            List<string> list = new(
                columnlist.Split(new string[] { "\r\n", "\n", "\r" },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            foreach (var s in list)
            {
                var SL = s.Split("=");
                Columns.Add(SL[0], new ColumnListItem(SL[1]));
            }
        }
    }

    public class LocalService<TItem>
    {
        public LocalService()
        {
        }
    }
}
