namespace InfoInfo.Infrastructure
{
    public static class Variety
    {
        public static string Phrase(string one, string twofour, string others, int count)
        {
            if (count == 1)
            {
                return one;
            }
            else if ((count % 10 >= 2 && count % 10 <= 4) && (count % 100 < 10 || count % 100 >= 20))
            {
                return twofour;
            }
            else
            {
                return others;
            }
        }
    }
}