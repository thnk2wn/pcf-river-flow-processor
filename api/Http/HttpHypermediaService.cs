using Flurl;
using Microsoft.AspNetCore.Http;
using RiverflowApi.Data.Services;
using RiverFlowApi.Data.Models;
using RiverFlowApi.Data.Models.Hypermedia;

namespace RiverFlowApi.Http
{
    public class HttpHypermediaService : IHypermediaService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpHypermediaService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public HyperlinkModel Hyperlink(string path, string rel, string method)
        {
            var baseUrl = GetBaseUrl();
            var url = baseUrl.AppendPathSegment(path);
            return new HyperlinkModel(href: url, rel: rel, method: method);
        }

        private string GetBaseUrl()
        {
            var context = this.httpContextAccessor.HttpContext;
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}";
            return baseUrl;
        }
    }
}