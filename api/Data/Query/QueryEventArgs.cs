using System;
using System.Collections.Generic;
using Humanizer;

namespace RiverFlowApi.Data.Query
{
    public class QueryEventArgs<TParam> : EventArgs
    {
        public QueryEventArgs(TParam param, TimeSpan elapsed)
        {
            this.Param = param;
            this.Elapsed = elapsed;
            this.ElapsedText = elapsed.Humanize();
        }

        public TParam Param { get; }

        public TimeSpan Elapsed { get; }

        public string ElapsedText { get; }
    }

    public class AfterQuerySuccessEventArgs<TResult, TParam> : QueryEventArgs<TParam>
    {
        public AfterQuerySuccessEventArgs(TParam param, TimeSpan elapsed, IEnumerable<TResult> results)
            : base(param, elapsed)
        {
            this.Results = results;
        }

        public IEnumerable<TResult> Results { get; }
    }

    public class AfterQueryFailureEventArgs<TParam> : QueryEventArgs<TParam>
    {
        public AfterQueryFailureEventArgs(TParam param, TimeSpan elapsed, Exception error)
            : base(param, elapsed)
        {
            this.Error = error;
        }

        public Exception Error { get; }
    }
}