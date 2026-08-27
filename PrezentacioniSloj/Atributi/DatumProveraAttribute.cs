using System.ComponentModel.DataAnnotations;

namespace PrezentacioniSloj.Atributi
{
    public class DatumProveraAttribute : ValidationAttribute
    {
        public DatumProveraAttribute()
            : base("Datum takmičenja ne može biti u budućnosti.") { }

        public override bool IsValid(object? value)
        {
            if (value is DateTime dt)
                return dt <= DateTime.Now;
            return true;
        }
    }
}

