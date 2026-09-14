Console.WriteLine("Hello, World!");
int[] numbers = { '2', '4', '6', '8', };
for (int i = 0; i < numbers.Length; i++)
{
    Console.WriteLine(numbers[i]);
}
//hkioiy897y8
         string CorrectPassword = "P@ssw0rd";

         int MaxAttempts = 5;

        
        {
            Console.WriteLine("Login System");

            bool loginSuccessful = false;
            int attemptsUsed = 0;

            for (int attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                attemptsUsed = attempt;
                Console.WriteLine("Enter password: ");
                string? guesspassword = Console.ReadLine();

                if (guesspassword == CorrectPassword)
                {
                    loginSuccessful = true;
                    break;
                }
                else
                {
                    int remainingAttempts = MaxAttempts - attempt;

                    if (remainingAttempts > 0)
                    {
                        Console.WriteLine($"Incorrect password. " +
                            $"You have {remainingAttempts} attempt(s) remaining.");
                    }
                }
            }

            Console.WriteLine();

            if (loginSuccessful)
            {
                Console.WriteLine($"Login successful! Welcome, {CorrectPassword}.");
            }
            else
            {
                Console.WriteLine("You have used all 5 attempts. Your account is locked out.");
                Console.WriteLine("Please contact support or try again later.");
            }

            Console.WriteLine($"Total attempts used: {attemptsUsed}/{MaxAttempts}");
        }


