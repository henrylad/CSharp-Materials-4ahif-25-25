const double HourInput = 52;
const double DayPrice = 10;
const double HourPrice = 1.8;


var totalDayPrice = Math.Floor(HourInput / 24) * DayPrice;
var totalHourPrice = (HourInput % 24) * HourPrice;
var totalPrice = totalDayPrice + totalHourPrice;

Console.WriteLine($"Total days: {totalDayPrice}");
Console.WriteLine($"Total hours: {totalHourPrice}");
Console.WriteLine($"Total price: {totalPrice}");
