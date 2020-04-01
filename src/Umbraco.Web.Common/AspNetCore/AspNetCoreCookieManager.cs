﻿using System;
using Microsoft.AspNetCore.Http;
using Umbraco.Core.Cookie;

namespace Umbraco.Web.Common.AspNetCore
{
    public class AspNetCoreCookieManager : ICookieManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AspNetCoreCookieManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void ExpireCookie(string cookieName)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null) return;

            var cookieValue = httpContext.Request.Cookies[cookieName];

            httpContext.Response.Cookies.Append(cookieName, cookieValue, new CookieOptions()
            {
                Expires = DateTime.Now.AddYears(-1)
            });
        }

        public string GetCookieValue(string cookieName)
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[cookieName];
        }

        public void SetCookieValue(string cookieName, string value)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(cookieName, value);
        }

        public bool HasCookie(string cookieName)
        {
            return !(GetCookieValue(cookieName) is null);
        }

    }
}
