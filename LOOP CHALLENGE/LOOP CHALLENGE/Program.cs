Console.WriteLine("Hello, World!");
static (int day, double volume) WaterReservoirLevel(
            double initial = 120, double capacity = 500,
            double refill = 85, double oddConsumption = 40,
            double evenConsumption = 65, double lowerBound = 50)
        {
            double volume = initial;
            int day = 0;

            while (true)
            {
                day++;
                volume += refill;
                volume -= (day % 2 != 0) ? oddConsumption : evenConsumption;

                if (volume >= capacity || volume < lowerBound)
                    break;
            }
    return (day, volume);
}