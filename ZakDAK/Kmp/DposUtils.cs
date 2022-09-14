using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;
using ZakDAK.Entities.DPE;

namespace ZakDAK.Kmp
{
    internal static class DposUtils
    {
        // Feldnamen korrigieren (Groß/Klein) anhand Entity Property Names und Relflection

        public static string AdjustFieldname(string Fieldname, IList<string> FieldList)
        {
            foreach (var field in FieldList)
            {
                string fieldname = field.ToString();
                if (fieldname.ToUpper() == Fieldname.ToUpper())
                    return fieldname;
            }
            return Fieldname;
        }

        //Feldnamen korrigieren: IDicionary Keys korrigieren
        //zB für FormatList, FLD1=Asw,Status -> fld1=Asw,Status
        public static void AdjustListKeys(IDictionary Fieldnames, IList<string> FieldList)
        {
            foreach (string oldname in Fieldnames.Keys)
            {
                string newname = AdjustFieldname(oldname, FieldList);
                if (newname != oldname)
                {
                    Fieldnames[newname] = Fieldnames[oldname];
                    Fieldnames.Remove(oldname);
                }
            }
        }

        //Feldnamen korrigieren: IDicionary Values korrigieren
        //zB für ColumnList, Nummer:5=FLD1 -> Nummer:5=fld1
        public static void AdjustListValues(IDictionary Fieldnames, IList<string> FieldList)
        {
            foreach (string fieldname in Fieldnames.Keys)
            {
                string oldvalue = (string)Fieldnames[fieldname];
                string newvalue = AdjustFieldname(oldvalue, FieldList);
                if (newvalue != oldvalue)
                {
                    Fieldnames[fieldname] = newvalue;
                }
            }
        }

        //Fieldliste einer Entity
        public static IList<string> GetFieldlist(Type TEntity)
        {
            PropertyInfo[] propertyInfos;
            propertyInfos = TEntity.GetProperties();  // BindingFlags.Public);

            // write property names
            var Result = new List<string>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //keine erweiterten Felder
                //if (propertyInfo.GetMethod == null)
                    Result.Add(propertyInfo.Name);
            }
            return Result;
        }
    }
}