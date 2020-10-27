using System;

class Reader
{
  public static int ReadInt32()
  {
    return int.Parse(Console.ReadLine());
  }

  public static decimal ReadDecimal()
  {
    return decimal.Parse(Console.ReadLine());
  }

  public static float ReadFloat()
  {
    return float.Parse(Console.ReadLine());
  } 
}

class Program
{
  public static void Main(string[] args)
  {
    Console.WriteLine(Problem3("szafa_taboret"));
    Console.WriteLine(Problem3("pijany.mistrz"));
  }

  static void Print(decimal value)
  {
    Console.WriteLine(String.Format("{0:F02}", value));
  }

  static void Problem1()
  {
    decimal a = Reader.ReadDecimal();
    decimal b = (decimal)Reader.ReadFloat();
    Print(a + b);
    Print(a - b);
    Print(a * b);
    Print(a / b);
  }

  static void Problem1Star()
  {
    int sum = 0;
    int num_count = 0;
    for (int _ = 0; _ < 10; _++)
    {
      int value = Reader.ReadInt32();
      if (value % 3 == 0 || value % 5 == 0)
      {
        sum += value;
        num_count++;
      }
    }
    decimal avg = (decimal)sum / num_count;
    Print(avg);
    Print((decimal)sum);
  }

  static bool Problem2(int value)
  {
    while (value > 0)
    {
      if (value % 10 == 3)
        return true;
      value /= 10;
    }
    return false;
  }

  static bool Problem2Star(int value)
  {
    return value.ToString().Contains('3');
  }

  static string Problem3(string expression)
  {
    string separator = "";
    foreach (char c in expression)
      if (!Char.IsLetter(c))
        separator = c.ToString();
    var split = expression.Split(separator);
    return split[1] + separator + split[0];
  }

  static int Problem4(int value)
  {
    while (value > 9)
      value = DigitSum(value);
    return value;
  }
  
  static int DigitSum(int value)
  {
    int sum = 0;
    while (value > 0)
    {
      sum += value % 10;
      value /= 10;
    }
    return sum;
  }

  static string Problem5(string text)
  {
    string compiled = "";
    int repetition_count = 0;
    char previous_char = text[0];
    foreach (char c in text)
    {
      if (c == previous_char)
        repetition_count++;
      else
      {
        compiled += previous_char + repetition_count.ToString();
        repetition_count = 0;
      }
      previous_char = c;
    }
    compiled += previous_char + repetition_count.ToString();
    return compiled;
  }
}
