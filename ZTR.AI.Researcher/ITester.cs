namespace ZT.AI.Researcher;

public interface ITester<T> where T : IGlobalOptions
{
    public void Run(T options);
}