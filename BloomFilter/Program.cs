namespace BloomFilter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BloomFilter<int> b = new(16);
            b.Insert(7);
            b.Insert(43);
            b.Insert(25);
            
            bool q = b.ProbablyContains(7);
            bool m = b.ProbablyContains(458);
        }
    }
}