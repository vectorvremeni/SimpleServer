using System;

namespace GNGame
{
    public class The_GNGame
    {
        public static string Gr_lose = "lose";
        public static string Gr_less = "less";
        public static string Gr_more = "more";
        public static string Gr_equal = "equal";
        
        Random rnd = new Random(DateTime.Now.Millisecond);

        int TheNumber = 0;
        int TryCounter = 0;
        int MaxTry = 0;
        int GetRandom()
        {
            return rnd.Next(1,80);
        }

        public void init(int trycount)
        {
            TheNumber = GetRandom();
            MaxTry = trycount;
            TryCounter = 0;
        }

        private string tryguess(int user_input)
        {
            if(this.TheNumber == user_input)
            {
                return Gr_equal;
            }
            else if(this.TheNumber < user_input)
            {
                return Gr_more;
            }
            else
            {
                return Gr_less;
            }
        }
        public string guess(int user_input)
        {

            if (TryCounter < MaxTry-1)
            {
                this.TryCounter++;
                return tryguess(user_input);
            }
            else
            {
                return Gr_lose;
            }

        }
        public void init_test(int thenumber)
        {
            this.TheNumber = thenumber;
        }
    }
}
