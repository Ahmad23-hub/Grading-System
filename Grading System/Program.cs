Console.WriteLine("Grading System");
Console.WriteLine("Enter a score from (0 - 100)");
int score= Convert.ToInt32(Console.ReadLine());
if (score < 0 || score > 100)
{
    Console.WriteLine("invalid score! Please enter a score between 0 and 100 ");
}
else if (score >= 90)
{
    Console.WriteLine("CONGRATULATIONS,  GRADE A");
}
else if (score >= 80)
{
    Console.WriteLine("VERY GOOD,  GRADE B");
}
else if (score >= 70)
{
    Console.WriteLine("GOOD,  GRADE C");
}
else if (score >= 60)
{
    Console.WriteLine("FAIR,  GRADE D");
}
else
{
    Console.WriteLine("POOR. YOU'RE AN OLODO,  GRADE F");
}
