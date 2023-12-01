namespace AdventOfCode2023;
public interface IDay
{
    bool Ready { get; set; }
    int Day { get; set; }
    Task RunPart1();
    Task RunPart2();
}
