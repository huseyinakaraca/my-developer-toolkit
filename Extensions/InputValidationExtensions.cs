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
            if (isimKismi.Length < 2)
                return $"Ad kýsmýnýz ('{isimKismi}') biraz kýsa görünüyor, en az 2 karakter olmalý.";
            if (soyisim.Length < 2)
                return $"Soyadýnýz ('{soyisim}') en az 2 karakter olmalý.";
            foreach (var isim in parcalar.Take(parcalar.Length - 1))
            {
                if (isim.Length < 2)
                    return $"Ad kýsmýndaki '{isim}' kelimesi çok kýsa. Her bir ad en az 2 karakter olmalýdýr.";
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
            int haneten = (((teklerToplami * 7) - ciftlerToplami) % 10 + 10) % 10;
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
        //Telefon 
        public static string GetPhoneNumberValidationMessage(this string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "Telefon numarasý alaný boþ býrakýlamaz.";
            if (phone.Any(c => char.IsLetter(c)))
                return "Telefon numarasý harf içeremez, sadece sayý girilmelidir.";
            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());
            if (digitsOnly.StartsWith("905") && digitsOnly.Length == 12)
            {
                digitsOnly = "0" + digitsOnly.Substring(2);
            }
            else if (digitsOnly.StartsWith("5") && digitsOnly.Length == 10)
            {
                digitsOnly = "0" + digitsOnly;
            }
            if (digitsOnly.Length != 11)
                return "Telefon numarasý 11 hane olmalýdýr (Örn: 05xx xxx xx xx).";
            if (!digitsOnly.StartsWith("05"))
                return "Telefon numarasý '05' ile baþlamalýdýr.";
            string aboneKismi = digitsOnly.Substring(4);
            if (aboneKismi.Distinct().Count() == 1)
                return "Lütfen rastgele (spam) bir numara girmeyiniz.";
            if ("0123456789".Contains(aboneKismi) || "9876543210".Contains(aboneKismi))
                return "Sýralý rakamlardan oluþan rastgele numaralar kabul edilmemektedir.";
            string[] gecerliPrefixler =
              {
              "0501", "0505", "0506", "0507", // Türk Telekom
              "0530", "0531", "0532", "0533", "0534", "0535", "0536", "0537", "0538", "0539", // Turkcell
              "0540", "0541", "0542", "0543", "0544", "0545", "0546", "0547", "0548", "0549", // Vodafone
              "0551", "0552", "0553", "0554", "0555", "0559", // BIMcell, PTTCell vb.
              "0561" // Emniyet / Netgsm vb.
              };
            string inputPrefix = digitsOnly.Substring(0, 4);
            if (!gecerliPrefixler.Contains(inputPrefix))
                return $"'{inputPrefix}' ile baþlayan geçerli bir operatör bulunamadý.";
            return "OK";
        }
        //E-Posta
        public static string GetEmailValidationMessage(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "E-posta alaný boþ býrakýlamaz.";
            if (email.Contains(" "))
                return "E-posta adresi boþluk içeremez.";
            if (email.Length > 60)
                return "E-posta adresi çok uzun, en fazla 60 karakter olabilir.";
            string[] gecerliUzantilar = { "@gmail.com", "@outlook.com", "@hotmail.com" };
            if (!gecerliUzantilar.Any(uzanti => email.EndsWith(uzanti)))
                return "Geçersiz e-posta! Adresinizin sonuna '@gmail.com', '@outlook.com' veya '@hotmail.com' eklemelisiniz (Örn: isim@gmail.com).";
            string secilenUzanti = gecerliUzantilar.First(uzanti => email.EndsWith(uzanti));
            if (email.Length <= secilenUzanti.Length)
                return "Lütfen '@' iþaretinden önceki kýsmý boþ býrakmayýnýz (Örn: huseyin@gmail.com).";
            string kullaniciAdi = email.Replace(secilenUzanti, "");
            if (kullaniciAdi.Length < 3)
                return "E-postanýzýn '@' iþaretinden önceki kýsmý en az 3 karakter olmalýdýr.";
            if (!kullaniciAdi.Any(char.IsLetter))
                return "E-postanýzýn '@' iþaretinden önceki kýsmý sadece rakamlardan oluþamaz, harf de içermelidir.";
            string izinVerilenKarakterler = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-";
            if (!kullaniciAdi.All(c => izinVerilenKarakterler.Contains(c)))
                return "E-posta adresi Türkçe karakter (ç,ð,ý,ö,þ,ü) veya geçersiz sembol içeremez.";
            return "OK";
        }
        // Þifre
        public static string GetPasswordValidationMessage(this string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Þifre alaný boþ býrakýlamaz.";
            if (password.Contains(" "))
                return "Þifreniz boþluk içeremez.";
            List<string> hatalar = new List<string>();
            if (password.Length < 8)
                hatalar.Add("- En az 8 karakter olmalýdýr.");
            if (password.Length > 50)
                hatalar.Add("- En fazla 50 karakter olabilir.");
            if (!password.Any(char.IsUpper))
                hatalar.Add("- En az bir büyük harf içermelidir.");
            if (!password.Any(char.IsLower))
                hatalar.Add("- En az bir küçük harf içermelidir.");
            if (!password.Any(char.IsDigit))
                hatalar.Add("- En az bir rakam içermelidir.");
            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                hatalar.Add("- En az bir özel karakter (!, ?, * vb.) içermelidir.");
            if (hatalar.Any())
            {
                return "Þifreniz zayýf. Lütfen þunlarý ekleyin:\n" + string.Join("\n", hatalar);
            }
            return "OK";
        }
        //Kullanýcý Adý
        public static string GetUsernameValidationMessage(this string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return "Kullanýcý adý alaný boþ býrakýlamaz.";
            if (username.Contains(" "))
                return "Kullanýcý adýnda boþluk bulunamaz. Lütfen bitiþik yazýnýz.";
            if (username.Length < 3)
                return "Kullanýcý adý çok kýsa, en az 3 karakter olmalýdýr.";
            if (username.Length > 15)
                return "Kullanýcý adý çok uzun, en fazla 15 karakter olabilir.";
            if (username.All(char.IsDigit))
                return "Kullanýcý adý sadece rakamlardan oluþamaz, en az bir harf içermelidir.";
            string izinVerilenKarakterler = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
            if (!username.All(c => izinVerilenKarakterler.Contains(c)))
                return "Kullanýcý adý Türkçe karakter (ç,ð,ý,ö,þ,ü) veya geçersiz sembol içeremez. Sadece harf, rakam ve alt çizgi (_) kullanabilirsiniz.";
            return "OK";
        }
        // Doðum Tarihi ve Yaþ Sýnýrý
        public static string GetBirthDateValidationMessage(this string birthDateText)
        {
            if (string.IsNullOrWhiteSpace(birthDateText))
                return "Doðum tarihi alaný boþ býrakýlamaz.";
            if (!DateTime.TryParse(birthDateText, out DateTime dogumTarihi))
                return "Lütfen geçerli bir tarih formatý giriniz (Örn: 25.05.2005 veya 25/05/2005).";
            DateTime bugun = DateTime.Today;
            if (dogumTarihi > bugun)
                return "Doðum tarihi bugünden ileri bir tarih olamaz. Gelecekten mi geldiniz?";
            int yas = bugun.Year - dogumTarihi.Year;
            if (dogumTarihi.Date > bugun.AddYears(-yas))
            {
                yas--;
            }
            if (yas > 120)
                return "Lütfen geçerli bir doðum yýlý girdiðinizden emin olun.";
            if (yas < 13)
                return $"Uygulamamýzý kullanabilmek için en az 13 yaþýnda olmalýsýnýz. (Mevcut yaþýnýz: {yas})";
            return "OK";
        }
    }
}