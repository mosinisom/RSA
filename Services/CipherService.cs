using System;
using System.Text;
using System.Numerics;

namespace Backend.Services
{
  public class CipherService
  {
    private bool IsPrime(int n)
    {
      if (n < 2) return false;
      for (int i = 2; i <= Math.Sqrt(n); i++)
        if (n % i == 0) return false;
      return true;
    }

    private int GeneratePrime(int min, int max)
    {
      Random rand = new Random();
      int num;
      do
      {
        num = rand.Next(min, max);
      } while (!IsPrime(num));
      return num;
    }

    private int GetE(int phi)
    {
      for (int e = 3; e < phi; e += 2)
        if (GCD(e, phi) == 1)
          return e;
      return 3;
    }

    private int GCD(int a, int b)
    {
      while (b != 0)
      {
        int temp = b;
        b = a % b;
        a = temp;
      }
      return a;
    }

    private int ModInverse(int e, int phi)
    {
      int d = 1;
      while (((d * e) % phi) != 1)
        d++;
      return d;
    }

    public string GenerateKey()
    {
      int p = GeneratePrime(100, 500);
      int q = GeneratePrime(100, 500);
      int n = p * q;
      int phi = (p - 1) * (q - 1);
      int e = GetE(phi);
      int d = ModInverse(e, phi);

      return $"{n},{e},{d}";
    }

    public string Encrypt(string text, string key)
    {
      var parts = key.Split(',');
      int n = int.Parse(parts[0]);
      int e = int.Parse(parts[1]);

      StringBuilder result = new StringBuilder();
      foreach (char c in text)
      {
        BigInteger m = (int)c;
        BigInteger encrypted = BigInteger.ModPow(m, e, n);
        result.Append(encrypted).Append(",");
      }
      return result.ToString().TrimEnd(',');
    }

    public string Decrypt(string text, string key)
    {
      var parts = key.Split(',');
      int n = int.Parse(parts[0]);
      int d = int.Parse(parts[2]);

      StringBuilder result = new StringBuilder();
      var numbers = text.Split(',');
      foreach (string number in numbers)
      {
        BigInteger c = BigInteger.Parse(number);
        BigInteger decrypted = BigInteger.ModPow(c, d, n);
        result.Append((char)(int)decrypted);
      }
      return result.ToString();
    }
  }
}