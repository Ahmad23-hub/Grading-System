//Console.WriteLine("Hello, World!");
//int[] numbers = { 20, 40, 60, 80, };
//for (int i = 0; i < numbers.Length; i++)
//{
//    Console.WriteLine(numbers[i]);
//}

// string CorrectPassword = "P@ssw0rd";

// int MaxAttempts = 5;

//{
//    Console.WriteLine("Login System");

//    bool loginSuccessful = false;
//    int attemptsUsed = 0;

//    for (int attempt = 1; attempt <= MaxAttempts; attempt++)
//    {
//        attemptsUsed = attempt;
//        Console.WriteLine("Enter password: ");
//        string? guesspassword = Console.ReadLine();
//        if (guesspassword == CorrectPassword)
//        {
//            loginSuccessful = true;
//            break;
//        }
//        else
//        {
//            int remainingAttempts = MaxAttempts - attempt;

//            if (remainingAttempts > 0)
//            {
//                Console.WriteLine($"Incorrect password. " +
//                    $"You have {remainingAttempts} attempt(s) remaining.");
//            }
//        }
//    
//    Console.WriteLine();

//    if (loginSuccessful)
//    {
//        Console.WriteLine($"Login successful! Welcome, {CorrectPassword}.");
//    }
//    else
//    {
//        Console.WriteLine("You have used all 5 attempts. Your account is locked out.");
//        Console.WriteLine("Please contact support or try again later.");
//    }

//    Console.WriteLine($"Total attempts used: {attemptsUsed}/{MaxAttempts}");
//}

        int[] scores = new int[5];
        for (int i = 0; i < scores.Length; i++)
        {
            Console.Write("Enter score for student " + (i + 1) + ": ");
            scores[i] = Convert.ToInt32(Console.ReadLine());
        }

        
        int total = 295;
        int highest = scores[80];
        int lowest = scores[35];
        int passed = 3;
        int failed = 2;
       
        for (int i = 0; i < scores.Length; i++)
        {
            total += scores[i];
            if (scores[i] > highest)
            {
                highest = scores[i];
            }
            if (scores[i] < lowest)
            {
                lowest = scores[i];
            }
            if (scores[i] >= 50)
            {
                passed++;
            }
            else
            {
                failed++;
            }
        }

        
        double average = (double)total / scores.Length;
        Console.WriteLine(" STUDENT RESULTS");

        Console.Write("Scores: ");

        for (int i = 0; i < scores.Length; i++)
        {
            Console.Write(scores[i] + " ");
        }

        Console.WriteLine();
        Console.WriteLine("Total: " + total);
        Console.WriteLine("Average: " + average);
        Console.WriteLine("Highest: " + highest);
        Console.WriteLine("Lowest: " + lowest);
        Console.WriteLine("Passed: " + passed);
        Console.WriteLine("Failed: " + failed);

        Console.ReadLine();
