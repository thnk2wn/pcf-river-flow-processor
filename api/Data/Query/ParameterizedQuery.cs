using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RiverFlowApi.Data
{
    public abstract class ParameterizedQuery<TResult, TParam>
    {
        public event EventHandler AfterQuery;

        public event EventHandler BeforeQuery;

        public TimeSpan QueryTime { get; private set; }

        public async Task<IEnumerable<TResult>> RunAsync(TParam param)
        {
            var sw = Stopwatch.StartNew();
            var results = await this.QueryAsync(param);
            sw.Stop();
            this.QueryTime = sw.Elapsed;
            return results;
        }

        public async Task<List<TResult>> RunListAsync(TParam param)
        {
            var items = await this.RunAsync(param);
            return items.ToList();
        }

        protected abstract Task<IEnumerable<TResult>> QueryAsync(TParam param);

        protected virtual void OnBeforeQuery()
        {
            this.BeforeQuery?.Invoke(this, new EventArgs());
        }

        protected virtual void OnAfterQuery()
        {
            this.AfterQuery?.Invoke(this, new EventArgs());
        }
    }
}