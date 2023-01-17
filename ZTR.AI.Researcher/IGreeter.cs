namespace ZT.AI.Researcher;

interface IGreeter
{
    void Greet(string name) => Console.WriteLine($"Hello, {name ?? "anonymous"}");
}