using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace CdnBundle.Core
{
    public class Promise<T>
    {
        public List<Promise<T>.State> promiseStates { get; set; }
        public Promise<T>.State state { get; set; }
        private Action<T> success { get; set; }
        private List<Action<T>> then = new List<Action<T>>();
        private Action done { get; set; }
        private Action<Exception> error { get; set; }
        private Func<T> work { get; set; }
        private int timeout { get; set; }
        private ManualResetEvent manualR = new ManualResetEvent(false);
        private CancellationTokenSource cts = new CancellationTokenSource();

        public Promise(Func<T> func)
        {
            this.state = State.Pending;
            this.promiseStates = new List<State>();
            this.promiseStates.Add(State.Pending);
            this.work = func;
            this.Execute();
        }

        public static Promise<T> Create(Func<T> func)
        {
            return new Promise<T>(func);
        }

        private void Execute()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
            {
                CancellationToken token = (CancellationToken)obj;
                token.WaitHandle.WaitOne(10000);
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation has been requested ... ");
                    return;
                }
                try
                {
                    T result = work();
                    manualR.Set();
                    if (this.success != null)
                    {
                        this.success(result);
                    }
                    if (then.Count > 0)
                    {
                        then.ForEach((action) =>
                        {
                            action(result);
                        });
                    }
                    if (state.Equals(State.Pending))
                    {
                        state = State.Fulfilled;
                        this.promiseStates.Add(State.Fulfilled);
                    }
                }
                catch (Exception ex)
                {
                    state = State.Rejected;
                    this.promiseStates.Add(State.Rejected);
                    if (error != null) error(ex);
                    else
                    {
                        Console.WriteLine(ex);
                        throw ex;
                    }
                }
                try
                {
                    if (done != null) done();
                }
                catch (Exception ex)
                {
                    state = State.Rejected;
                    this.promiseStates.Add(State.Rejected);
                    if (error != null) error(ex);
                    else
                    {
                        Console.WriteLine(ex);
                        throw ex;
                    }
                }
            }), cts.Token);
        }

        public Promise<T> Wait()
        {
            this.manualR.WaitOne();
            this.state = State.Waiting;
            this.promiseStates.Add(State.Waiting);
            return this;
        }

        public Promise<T> Success(Action<T> act)
        {
            this.success = act;
            return this;
        }

        public Promise<T> Then(Action<T> act)
        {
            this.then.Add(act);
            return this;
        }

        public Promise<T> Done(Action act)
        {
            this.done = act;
            return this;
        }

        public Promise<T> Error(Action<Exception> act)
        {
            this.error = act;
            return this;
        }

        public enum State
        {
            Pending,
            Fulfilled,
            Rejected,
            Waiting
        }
    }
}
