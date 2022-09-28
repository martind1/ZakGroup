using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Reflection;
using ZakDAK.Entities.DPE;

namespace ZakDAK.Kmp
{
    internal static class DposUtils
    {
        #region Feldnamen korrigieren (Groß/Klein)

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

        //Fieldliste einer Entity. Mit FieldInfos.
        public static IDictionary<string, FieldInfo> GetFieldlist(Type TEntity)
        {
            PropertyInfo[] propertyInfos;
            propertyInfos = TEntity.GetProperties();  // BindingFlags.Public);

            // write property names
            var fieldlist = new Dictionary<string, FieldInfo>();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                //keine erweiterten Felder
                if (propertyInfo.SetMethod != null)
                    fieldlist.Add(propertyInfo.Name, new FieldInfo(propertyInfo.PropertyType));
            }
            return fieldlist;
        }

        public static bool HasFilterChar(string kmpstr)
        {
            return kmpstr.Contains('%') || kmpstr.Contains('_');
        }

        #endregion

    }

    public class FieldInfo
    {
        private Type _propertyType;
        public Type PropertyType
        {
            get => _propertyType;
            set
            {
                _propertyType = value;

                if (value == typeof(int) || value == typeof(short) || value == typeof(byte) ||
                    value == typeof(sbyte) || value == typeof(uint) || value == typeof(long) ||
                    value == typeof(bool) || value == typeof(Int32) || value == typeof(Int64) ||
                    value == typeof(int?) || value == typeof(short?) || value == typeof(byte?) ||
                    value == typeof(sbyte?) || value == typeof(uint?) || value == typeof(long?) ||
                    value == typeof(bool?) || value == typeof(Int32?) || value == typeof(Int64?)
                    )
                        fieldType = FieldType.ftInt;
                    
                else if (value == typeof(double) || value == typeof(float) || value == typeof(decimal) ||
                         value == typeof(double?) || value == typeof(float?) || value == typeof(decimal?)
                    )
                    fieldType = FieldType.ftFloat;

                else if (value == typeof(string)   //kein nullable string??? || value == typeof(string?)
                    )
                    fieldType = FieldType.ftString;

                else if (value == typeof(DateTime) || value == typeof(DateTime?))
                    fieldType = FieldType.ftDateTime;

                else
                    throw new NotImplementedException($"propertyType={value}");

            }
        }
        public FieldType fieldType { get; set; }

        public FieldInfo(Type propertyType)
        {
            PropertyType = propertyType;
        }
    }

    public enum FieldType
    {
        ftString,
        ftInt,
        ftFloat,
        ftDateTime
    }

    


}