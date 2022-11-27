namespace SourceGeneratorSample;

class Program
{
    static void Main()
    {
        IRunner runner = 
            new RunnerStj();
            //new RunnerStjCustom();
            //new RunnerJsonNet();
        runner.Run();
    }
}
