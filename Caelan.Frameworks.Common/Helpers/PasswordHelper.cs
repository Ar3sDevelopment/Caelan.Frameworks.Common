using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Caelan.Frameworks.Common.Helpers
{
    public abstract class PasswordHelper
    {
        public abstract string GetSalt();
        protected abstract string GetDefaultPassword();

        public virtual string GetDefaultPasswordEncrypted()
        {
            return EncryptPassword(GetDefaultPassword());
        }

        private static string Sha512Encrypt(string password)
        {
            return String.Join("", (new SHA512CryptoServiceProvider()).ComputeHash(Encoding.Default.GetBytes(password)).Select(b => b.ToString("x2").ToLower()));
        }

        public virtual string EncryptPassword(string password)
        {
            return Sha512Encrypt(GetSalt() + Sha512Encrypt(password));
        }
    }
}
