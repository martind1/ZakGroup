namespace ZakDAK.Kmp
{
    public class Asws
    {
        public string Param { get; set; }
        public string Value { get; set; }
        //für Ceckbox:
        //public bool? this[string index]
        //{
        //    get { return index == "J" ? true : index == "N" ? false : null; }
        //    set { }
        //}
    }


    /// <summary>
    /// Feste Auswahlen Param=Gespeicherter Wert, Value=angezeigter Wert
    /// </summary>
    public static class Auswahl
    {

        public static readonly IEnumerable<Asws> aswOK = new Asws[] {
            new Asws(){ Param = "J", Value = "OK" },
            new Asws(){ Param = "N", Value = "Nicht OK" } };

        public static readonly IEnumerable<Asws> aswOKStrich = new Asws[] {
            new Asws(){ Param = "J", Value = "OK" },
            new Asws(){ Param = "N", Value = "-" } };

        public static readonly IEnumerable<Asws> aswLityp = new Asws[] {
            new Asws(){ Param = "A", Value = "Entsorgung" },
            new Asws(){ Param = "B", Value = "Abgänge" },
            new Asws(){ Param = "W", Value = "Intern" } };
    }
}
