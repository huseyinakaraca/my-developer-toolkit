using System.Text.RegularExpressions;
using System.Linq;
namespace my_developertoolkit.Extensions
{
    public static class InputValidationExtensions
    {
        //Ad ve Soyad
        public static string GetFullNameValidationMessage(this string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Ad ve soyad alaný boþ býrakýlamaz.";
            if (name.Length > 50)
                return "Ad ve soyad toplamda en fazla 50 karakter olabilir.";
            if (name.StartsWith(" ") || name.EndsWith(" "))
                return "Lütfen ad ve soyad baþýnda veya sonunda boþluk býrakmayýn.";
            if (name.Contains("  "))
                return "Kelimeler arasýnda sadece bir tane boþluk býrakabilirsiniz.";
            if (name.Any(c => char.IsDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c)))
                return "Sadece harf kullanýlabilirsin.";
            var parcalar = name.Split(' ');
            if (parcalar.Length < 2)
                return "Lütfen hem adýnýzý hem de soyadýnýzý girdiðinizden emin olun.";
            string soyisim = parcalar.Last();
            string isimKismi = string.Join(" ", parcalar.Take(parcalar.Length - 1));
            if (isimKismi.Length < 3)
                return $"Ad kýsmýnýz ('{isimKismi}') biraz kýsa görünüyor, en az 3 karakter olmalý.";
            if (soyisim.Length < 2)
                return $"Soyadýnýz ('{soyisim}') en az 2 karakter olmalý.";
            foreach (var isim in parcalar.Take(parcalar.Length - 1))
            {
                if (isim.Length < 3)
                    return $"Ad kýsmýndaki '{isim}' kelimesi çok kýsa. Her bir ad en az 3 karakter olmalýdýr.";
                string idealIsim = char.ToUpper(isim[0]) + isim.Substring(1).ToLower();

                if (isim != idealIsim)
                    return $"'{isim}' yazýmý uygun deðil. Adlarýn ilk harfi büyük, diðer harfleri küçük olmalýdýr (Örn: {idealIsim}).";
            }
            if (soyisim != soyisim.ToUpper())
                return $"Soyadýnýzý ('{soyisim}') tamamen büyük harf yazmalýsýnýz (Örn: {soyisim.ToUpper()}).";
            return "OK";
        }
        //Tc_Kimlik
        public static string GetTcNoValidationMessage(this string tcNo)
        {
            if (string.IsNullOrWhiteSpace(tcNo))
                return "TC Kimlik alaný boþ býrakýlamaz.";
            if (tcNo.Length != 11)
                return "TC Kimlik numarasý tam 11 hane olmalýdýr.";
            if (tcNo.Any(c => !char.IsDigit(c)))
                return "TC Kimlik numarasý sadece rakamlardan oluþmalýdýr.";
            if (tcNo.StartsWith("0"))
                return "TC Kimlik numarasý 0 ile baþlayamaz.";
            int[] n = tcNo.Select(c => int.Parse(c.ToString())).ToArray();
            int teklerToplami = n[0] + n[2] + n[4] + n[6] + n[8];
            int ciftlerToplami = n[1] + n[3] + n[5] + n[7];
            int haneten = ((teklerToplami * 7) - ciftlerToplami) % 10;
            if (haneten != n[9])
                return "Geçersiz bir TC Kimlik numarasý girdiniz.";
            int ilkOnToplam = 0;
            for (int i = 0; i < 10; i++)
            {
                ilkOnToplam += n[i];
            }
            if (ilkOnToplam % 10 != n[10])
                return "Geçersiz bir TC Kimlik numarasý girdiniz.";
            return "OK";
        }
    }
}