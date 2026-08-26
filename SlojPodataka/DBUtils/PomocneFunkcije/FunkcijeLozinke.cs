using System;
using System.Security.Cryptography;
using System.Text;

namespace BibliotekaKlasa.TehnoloskeKlase.PomocneFunkcije
{
    public static class FunkcijeLozinke
    {
        private const int BrojBajtova = 64;
        private const int Iteracije = 100_000;

        public static void KreirajHash(string lozinka, out byte[] hash, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(BrojBajtova);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                lozinka,
                salt,
                Iteracije,
                HashAlgorithmName.SHA512);

            hash = pbkdf2.GetBytes(BrojBajtova);
        }

        public static bool VerifikujLozinku(string lozinka, byte[] hash, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(
                lozinka,
                salt,
                Iteracije,
                HashAlgorithmName.SHA512);

            byte[] IzracunatHash = pbkdf2.GetBytes(BrojBajtova);

            return CryptographicOperations.FixedTimeEquals(IzracunatHash, hash);
        }

    }
}
