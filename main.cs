using System;
using System.Threading;
using System.Collections.Generic;
using static System.Console;
using static Utilities.Library;
using Utilities;
using Demo;

class Program {
  public static void Main (string[] args) {
    executeDemos(3);
  }

  private static void basic() {
    Func<int, int, int> Add = (int x, int y) => x + y;
    Func<string, string> Echo = (string str) => str;
    
    Func<double, double> memoizedSqrt = Memoize(Log<double, double>(Math.Sqrt));
    Func<int, int, int> memoizedAdd = Memoize(Log<int, int, int>(Add));
    Func<string, string> memoizedEcho = Memoize(Log<string, string>(Echo));

    // First time calls
    WriteLine("\n");
    WriteLine("Computed (Basic Function Demo):");
    WriteLine("---------------------------------");
    WriteLine($"Square root result: {memoizedSqrt(16)}");
    WriteLine($"Addition result: {memoizedAdd(16, 10)}");
    WriteLine($"Echo result: {memoizedEcho("Echoeeeeeeeeeeeeeeeed")}");

    // Subsequent calls
    WriteLine("\n");
    WriteLine("Optimized (Basic Function Demo):");
    WriteLine("--------------------------------");
    WriteLine($"Square root result: {memoizedSqrt(16)}");
    WriteLine($"Addition result: {memoizedAdd(16, 10)}");
    WriteLine($"Echo result: {memoizedEcho("Echoeeeeeeeeeeeeeeeed")}");
  }

  private static void composition() {
    Func<double, double, double> Add = (double x, double y) => x + y;
    Func<string, string> Echo = (string str) => str;
    
    List<Func<Func<double, double>, Func<double, double>>> doubleFuncs = new List<Func<Func<double, double>, Func<double, double>>>(){Memoize,Log};
    Composer<double, double> doublePipelineComposer = new Composer<double, double>(doubleFuncs);

    List<Func<Func<double, double, double>, Func<double, double, double>>> biFuncs = new List<Func<Func<double, double, double>, Func<double, double, double>>>(){Memoize,Log};
    BiComposer<double, double, double> biPipelineComposer = new BiComposer<double, double, double>(biFuncs);

    List<Func<Func<string, string>, Func<string, string>>> stringFuncs = new List<Func<Func<string, string>, Func<string, string>>>(){Memoize,Log};
    Composer<string, string> stringPipelineComposer = new Composer<string, string>(stringFuncs);

    Func<double, double> memoizedSqrt = doublePipelineComposer.CreatePipeline(Math.Sqrt);
    Func<double, double, double> memoizedAdd = biPipelineComposer.CreatePipeline(Add);
    Func<string, string> memoizedEcho = stringPipelineComposer.CreatePipeline(Echo);

    // First time calls
    WriteLine("\n");
    WriteLine("Computed (Composition Function Demo):");
    WriteLine("---------------------------------");
    WriteLine($"Square root result: {memoizedSqrt(16)}");
    WriteLine($"Addition result: {memoizedAdd(16, 10)}");
    WriteLine($"Echo result: {memoizedEcho("Echoeeeeeeeeeeeeeeeed")}");

    // Subsequent calls
    WriteLine("\n");
    WriteLine("Optimized (Composition Function Demo):");
    WriteLine("--------------------------------");
    WriteLine($"Square root result: {memoizedSqrt(16)}");
    WriteLine($"Addition result: {memoizedAdd(16, 10)}");
    WriteLine($"Echo result: {memoizedEcho("Echoeeeeeeeeeeeeeeeed")}");
  }

  // Invoke transaction pipelines defined demo usecase
  private static void usecaseDemo() {
    /*--------------Execute business transaction pipelines------------------------------*/
    DemoUsecase demo = new DemoUsecase();
    // Create email for the user now
    demo.startCreateEmailTransaction("sandeep.mohanty");
    // Sent out an organization announcement after 5 seconds
    Thread.Sleep(5000);
    demo.startAnnouncement("Announcement: Business will remain closed tomorrow");

    // Create email for the malicious user
    Thread.Sleep(10000);
    demo.startCreateEmailTransaction("malicious.user");
  }

  // Calculating sum using tail-optimized trampoline method (prevents stack-overflow)
  private static void invoketailOptimizedSum() {
    Func<long,long,Result<long>> optimizedSum = tailOptimizedSum;
    Func<Func<long,long,Result<long>>,Func<long, long, long>> Trampoline = trampoline;
    WriteLine($"\nTail Optimized Sum: {Trampoline(optimizedSum)(10000000, 0)}");
  }

  // Calculating sum using normal recursive method which is not tail-optimized (could result in stack-overflow)
  private static void invokeRecursiveSum() {
    WriteLine($"\nRecursive Sum: {recursiveSum(1000000)}");
  }

  private static void executeDemos(int demoId) {
    Dictionary<int, Action> invocations = new Dictionary<int, Action>();
    invocations.Add(1, basic);
    invocations.Add(2, composition);
    invocations.Add(3, usecaseDemo);
    invocations.Add(4, invoketailOptimizedSum);
    invocations.Add(5, invokeRecursiveSum);
    var selectedFunction = invocations[demoId];
    if ( selectedFunction != null ) {
      selectedFunction();
    } else {
      WriteLine("Invalid option");
    }
  }
}