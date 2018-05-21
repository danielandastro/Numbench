using System;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace Numbench
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static string Process(int digits)
        {
            var result = new StringBuilder();
            result.Append("3.");
            var startTime = DateTime.Now;
            if (digits <= 0) return result.ToString();
            for (var i = 0; i < digits; i += 9)
            {
                var ds = CalculatePiDigits(i + 1);
                var digitCount = Math.Min(digits - i, 9);
                if (ds.Length < 9) ds = $"{int.Parse(ds):D9}";
                result.Append(ds.Substring(0, digitCount));
            }

            return result.ToString();
        }

        private static int Mul_mod(int a, int b, int m)
        {
            return (int)(a * (long)b % m);
        }

        private static int inv_mod(int x, int y)
        {
            var u = x;
            var v = y;
            var c = 1;
            var a = 0;
            do
            {
                var q = v / u;
                var t = c;
                c = a - q * c;
                a = t;
                t = u;
                u = v - q * u;
                v = t;
            } while (u != 0);

            a = a % y;
            if (a < 0) a = y + a;
            return a;
        }

        private static int pow_mod(int a, int b, int m)
        {
            var r = 1;
            var aa = a;
            while (true)
            {
                if ((b & 1) != 0) r = Mul_mod(r, aa, m);
                b = b >> 1;
                if (b == 0) break;
                aa = Mul_mod(aa, aa, m);
            }

            return r;
        }

        private static bool is_prime(int n)
        {
            if (n % 2 == 0) return false;
            var r = (int)Math.Sqrt(n);
            for (var i = 3; i <= r; i += 2)
                if (n % i == 0)
                    return false;
            return true;
        }

        private static int next_prime(int n)
        {
            do
            {
                n++;
            } while (!is_prime(n));

            return n;
        }

        private static string CalculatePiDigits(int n)
        {
            var N = (int)((n + 20) * Math.Log(10) / Math.Log(2));
            double sum = 0;
            for (var a = 3; a <= 2 * N; a = next_prime(a))
            {
                var vmax = (int)(Math.Log(2 * N) / Math.Log(a));
                var av = 1;
                for (var i = 0; i < vmax; i++) av = av * a;
                var s = 0;
                var num = 1;
                var den = 1;
                var v = 0;
                var kq = 1;
                var kq2 = 1;
                int t;
                for (var k = 1; k <= N; k++)
                {
                    t = k;
                    if (kq >= a)
                    {
                        do
                        {
                            t = t / a;
                            v--;
                        } while (t % a == 0);

                        kq = 0;
                    }

                    kq++;
                    num = Mul_mod(num, t, av);
                    t = 2 * k - 1;
                    if (kq2 >= a)
                    {
                        if (kq2 == a)
                            do
                            {
                                t = t / a;
                                v++;
                            } while (t % a == 0);

                        kq2 -= a;
                    }

                    den = Mul_mod(den, t, av);
                    kq2 += 2;
                    if (v > 0)
                    {
                        t = inv_mod(den, av);
                        t = Mul_mod(t, num, av);
                        t = Mul_mod(t, k, av);
                        for (var i = v; i < vmax; i++) t = Mul_mod(t, a, av);
                        s += t;
                        if (s >= av) s -= av;
                    }
                }

                t = pow_mod(10, n - 1, av);
                s = Mul_mod(s, t, av);
                sum = (sum + s / (double)av) % 1.0;
            }

            var result = (int)(sum * 1e9);
            var stringResult = $"{result:D9}";
            return stringResult;
        }

        private static string BreakDigitsIntoGroupsOf10(string digits)
        {
            var result = "";
            while (digits.Length > 10)
            {
                result += digits.Substring(0, 10) + " ";
                digits = digits.Substring(10, digits.Length - 10);
            }

            result += digits;
            return result;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var nwatch = new Stopwatch();
            var watch = new Stopwatch();
            ulong n1 = 0, n2 = 1;
            long i;
            watch.Start();
            Process(999);
            watch.Stop();
            met1.Text = watch.Elapsed.ToString();
            nwatch.Start();
            for (i = 2; i < 999999999.0; ++i)
            {
                var n3 = n1 + n2;
                n1 = n2;
                n2 = n3;
            }

            nwatch.Stop();
            met2.Text = nwatch.Elapsed.ToString();
            double temp = nwatch.ElapsedMilliseconds * watch.ElapsedMilliseconds;
            temp = Math.Log10(temp);
            temp = 1 / temp;
            met3.Text = temp.ToString();
        }
    }
}


