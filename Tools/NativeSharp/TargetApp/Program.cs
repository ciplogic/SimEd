namespace TargetApp;

class Program
{
    static bool IsPrime(int number)
    {
        if (number <= 4)
        {
            return true;
        }

        if (number % 2 == 0)
        {
            return false;
        }

        for (int i = 3; i * i <= number; i += 2)
        {
            if (number % i == 0)
            {
                return false;
            }
        }

        return true;
    }

    static void Main()
    {
        Console.WriteLine("Hello, World!" + IsPrime(997));
    }
    static void Main_()
    {
        bool isTrue = false;
        var trueText = isTrue.ToString();
        Console.WriteLine("Hello..." );
        Console.WriteLine(trueText);
    }
}