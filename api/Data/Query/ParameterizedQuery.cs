using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RiverFlowApi.Data.Query
{
    public abstract class ParameterizedQuery<TResult, TParam>
    {
        public event EventHandler<AfterQuerySuccessEventArgs<TResult, TParam>> AfterQuerySuccess;

        public event EventHandler<AfterQueryFailureEventArgs<TParam>> AfterQueryFailure;

        public event EventHandler<TParam> BeforeQuery;

        public async Task<IEnumerable<TResult>> RunAsync(TParam param)
        {
            OnBeforeQuery(param);
            var sw = Stopwatch.StartNew();

            try
            {
                var results = await this.QueryAsync(param);
                sw.Stop();
                OnAfterQuerySuccess(
                    new AfterQuerySuccessEventArgs<TResult, TParam>(param, sw.Elapsed, results));
                return results;
            }
            catch (Exception ex)
            {
                sw.Stop();
                OnAfterQueryFailure(
                    new AfterQueryFailureEventArgs<TParam>(param, sw.Elapsed, ex));
                throw;
            }
        }

        public async Task<List<TResult>> RunListAsync(TParam param)
        {
            var items = await this.RunAsync(param);
            return items.ToList();
        }

        protected abstract Task<IEnumerable<TResult>> QueryAsync(TParam param);

        protected virtual void OnBeforeQuery(TParam param)
        {
            this.BeforeQuery?.Invoke(this, param);
        }

        protected virtual void OnAfterQuerySuccess(AfterQuerySuccessEventArgs<TResult, TParam> e)
        {
            this.AfterQuerySuccess?.Invoke(this, e);
        }

        protected virtual void OnAfterQueryFailure(AfterQueryFailureEventArgs<TParam> e)
        {
            this.AfterQueryFailure?.Invoke(this, e);
        }
    }
}