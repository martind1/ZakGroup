using NuGet.Protocol;
using System.Security.Policy;

namespace ZakDAK.Kmp
{
    // Datenstrukturen für LNav, GNav:
    // Columnlist: Metadaten für Grid Column
    // FltrList: Metadaten für Suchkriterien / .where
    // SqlFieldList: Metadaten für Feldliste / .insert

#region ColumnList
    //ColumnList: Verbindung zur KMP Welt. Für SQL Where Clause
    public class ColumnList
    {
        public IDictionary<string, ColumnListItem> Columns { get; set; }

        public ColumnList()
        {
            Columns = new Dictionary<string, ColumnListItem>();
        }

        public ColumnList(string columnlist) : this()
        {
            //fltrlist Zeile idF <ColDesc>=<FieldName>
            List<string> list = new(
                columnlist.Split(new string[] { "\r\n", "\n", "\r" },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            foreach (var s in list)
            {
                if (!s.StartsWith(";"))
                {
                    var SL = s.Split("=");
                    var cli = new ColumnListItem(SL[0]);
                    Columns.Add(SL[1], cli);
                }
            }
        }
    }


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
        public string WidthPx { get { int dw8 = DisplayWidth * 8; return $"{dw8}px"; } }
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
    #endregion

#region FltrList und SQL generieren (Where Clause)
    //FltrList: Verbindung zur KMP Welt. Für SQL Where Clause
    public class FltrList
    {
        public IDictionary<string, FltrListItem> Fltrs { get; set; }

        public FltrList()
        {
            Fltrs = new Dictionary<string, FltrListItem>();
        }

        public FltrList(string fltrlist) : this()
        {
            //fltrlist Zeile idF <FieldName>=<Suchkriterien>
            List<string> list = new(
                fltrlist.Split(new string[] { "\r\n", "\n", "\r" },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
            foreach (var s in list)
            {
                if (!s.StartsWith(";"))
                {
                    var SL = s.Split("=");
                    if (SL.Length >= 2 && !String.IsNullOrEmpty(SL[1]))
                    {
                        var fli = new FltrListItem(SL[1]);
                        Fltrs.Add(SL[0], fli);
                    }
                }
            }
        }

        //ergibt komplette Where-Clause für die FltrList bzw LNav.References+Fltrlist
        public string GetWhere()
        {
            //todo: Kmp.SqlService.GetWhere(fltrlist)
            return "";
        }
    }


    // Beschreibung Suchkriterien eines Felds
    // Kmp (Kmp.Fltrlist): a >a <a == >= a;b;c [raw Feld-Argument] {raw Zeile ohne Feld}
    // Where (für Linq): <FldName> = "a" oder <FldName> > b
    // OrFlag (Kmp beginnt mit ';'): true = Verknüpfung mit 'or' mit anderen Feldern (und entspr Klammerung)
    public class FltrListItem
    {
        public string Kmp { get; set; }
        public string Where { get; set; }
        public bool OrFlag { get; set; } = false;

        //Item anhand KMP Beschreibung anlegen. Noch kein Where/SQL
        public FltrListItem(string kmp)
        {
            Kmp = kmp;
        }
    }
#endregion

}
