using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities {
  static class Library {
     public static Func<T,TResult> Log <T,TResult> (Func<T,TResult> f) {
       Func<T, TResult> returnFunc = (T args) => {
         WriteLine($"Computing for argument(s): {args.ToString()}");
         TResult results = f(args);
         return results;
       };
       return returnFunc;
     }

     public static Func<T1,T2,TResult> Log <T1,T2,TResult> (Func<T1,T2,TResult> f) {
       Func<T1,T2,TResult> returnFunc = (T1 arg1, T2 arg2) => {
         WriteLine($"Computing for argument(s): {arg1.ToString()},{arg2.ToString()} ");
         TResult results = f(arg1, arg2);
         return results;
       };
       return returnFunc;
     }

     public static Func<T,TResult> Memoize <T, TResult> (Func<T, TResult> f) {
       Dictionary<string, TResult> values = new Dictionary<string, TResult>();
       Func<T, TResult> returnFunc = (T arg) => {
         var key = arg.ToString();
         TResult value = (values.ContainsKey(key)) ? values[key] : (values[key] = f(arg));
         return value;
       };
       return returnFunc;
     }

     public static Func<T1,T2,TResult> Memoize <T1, T2, TResult> (Func<T1, T2, TResult> f) {
       Dictionary<string, TResult> values = new Dictionary<string, TResult>();
       Func<T1, T2, TResult> returnFunc = (T1 arg1, T2 arg2) => {
         var key = arg1.ToString() + arg2.ToString();
         TResult value = (values.ContainsKey(key)) ? values[key] : (values[key] = f(arg1, arg2));
         return value;
       };
       return returnFunc;
     }

     public static long recursiveSum(long count){
      long sum = count;
      if (count == 0) {
        return sum;
      } else {
        return sum + recursiveSum(count-1);
      }
     }

     public static Func<T1,T2,TResult> trampoline <T1, T2, TResult>(Func<T1,T2,Result<TResult>> f) {
       Func<T1,T2,TResult> returnedFunc = (T1 arg1, T2 arg2) => {
         Result<TResult> res = f(arg1, arg2);
         while (res._nextCall != null ) {
           res = res._nextCall();
         }
         return res._value;
       };
       return returnedFunc;
     }

     public static Result<long> tailOptimizedSum(long count, long accu = 0) {
      Result<long> continuation;
      if (count == 0) {
        continuation = new Result<long>(accu, null);
      } else {
        accu += count;
        count -= 1;
        continuation = new Result<long>(accu, () => tailOptimizedSum(count, accu));
      }
      return continuation;
     }

  }

  class Composer <T, TResult> {

    private List<Func<Func<T, TResult>, Func<T, TResult>>> _fns;

    public Composer (List<Func<Func<T, TResult>, Func<T, TResult>>> fns) {
      fns.Reverse();
      _fns = fns;
    }

    public Func<T, TResult> CreatePipeline(Func<T, TResult> source) {
      Func<T, TResult> pipeline = source;
      foreach ( Func<Func<T, TResult>, Func<T, TResult>> fn in _fns ) {
        pipeline = fn(pipeline);
      }
      return pipeline;
    }
  }

  class BiComposer <T1, T2, TResult> {

    private List<Func<Func<T1,T2,TResult>, Func<T1,T2,TResult>>> _fns;

    public BiComposer (List<Func<Func<T1,T2,TResult>, Func<T1,T2,TResult>>> fns) {
      fns.Reverse();
      _fns = fns;
    }

    public Func<T1,T2,TResult> CreatePipeline(Func<T1,T2,TResult> source) {
      Func<T1,T2,TResult> pipeline = source;
      foreach ( Func<Func<T1,T2,TResult>, Func<T1,T2,TResult>> fn in _fns ) {
        pipeline = fn(pipeline);
      }
      return pipeline;
    }
  }

  class MessageBroker <T> {
    private Dictionary<string, List<Action<T>>> topics;

    public MessageBroker() {
      topics =  new Dictionary<string, List<Action<T>>>();
    }

    public void addSubscription(string topicName, Action<T> action) {
      if ( topics.ContainsKey(topicName)) {
        topics[topicName].Add(action);
      } else {
        topics.Add(topicName, new List<Action<T>>());
        topics[topicName].Add(action);
      }
    }

    public void sendMessage(string topicName, T message) {
      if (topics.ContainsKey(topicName)) {
        topics[topicName].ForEach( subscriber => subscriber(message));
      }
    }
  }

  class Result <T> {
    public T _value {get; set;}
    public Func<Result<T>> _nextCall {get; set;}
    public Result(T value, Func<Result<T>> nextCall) {
      _value = value;
      _nextCall = nextCall;
    }
  }
}