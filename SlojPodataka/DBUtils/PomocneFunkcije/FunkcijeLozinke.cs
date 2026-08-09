using System;
using System.Security.Cryptography;
using System.Text;

namespace BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije
{
    public static class FunkcijeLozinke
    {
        private const int BrojBajtova = 64;
        private const int Iteracije = 100_000;

        // ========== 1. Create hash + salt (used when registering) ==========
        public static void KreirajHash(string lozinka, out byte[] hash, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(16); // 16-byte salt (good default)

            using var pbkdf2 = new Rfc2898DeriveBytes(
                lozinka,
                salt,
                Iteracije,
                HashAlgorithmName.SHA512);

            hash = pbkdf2.GetBytes(BrojBajtova);
        }

        // ========== 2. Verify password (used when logging in) ==========
        public static bool VerifikujLozinku(string lozinka, byte[] hash, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                lozinka,
                salt,
                Iteracije,
                HashAlgorithmName.SHA512);

            byte[] IzracunatHash = pbkdf2.GetBytes(BrojBajtova);

            // Constant-time comparison (prevents timing attacks)
            return CryptographicOperations.FixedTimeEquals(IzracunatHash, hash);
        }

        public static string GenerisiSalt(int duzina = 16)
        {
            byte[] saltBajtovi = new byte[duzina];
            using (var randomBroj = RandomNumberGenerator.Create())
            {
                randomBroj.GetBytes(saltBajtovi);
            }
            return Convert.ToBase64String(saltBajtovi);
        }

        public static Guid GenerisiGUIDSalt(int duzina = 16)
        {
            byte[] saltBajtovi = new byte[duzina];
            using (var randomBroj = RandomNumberGenerator.Create())
            {
                randomBroj.GetBytes(saltBajtovi);
            }
            return Guid.Parse(Convert.ToBase64String(saltBajtovi));

        }
        public static string IzracunajHash(string lozinka, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedLozinka = lozinka + salt;
                byte[] hashBajtovi = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedLozinka));
                return Convert.ToBase64String(hashBajtovi);
            }
        }

        public static bool ProveriLozinku(string lozinka, string salt, string hash)
        {
            string izracunatiHash = IzracunajHash(lozinka, salt);
            return izracunatiHash == hash;
        }
    }
}
