using CodeCamp2016.Models;
using CodeCamp2016Service.Data;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace CodeCamp2016Service.Controllers
{
    [AllowAnonymous]
    public class LoginController : ApiController
    {
        private readonly string BasicAuthResponseHeaderValue = "Basic";        

        public LoginController()
        {
           
        }

        [HttpGet]
        public HttpResponseMessage Index()
        {
            try
            {
                var user = ParseAuthorizationHeader(this.Request);

                if (XmlReadWrite.FindUser(user[0], user[1]) == false)
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "username not found");

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "bad request");
            }
        }

        [HttpPut, ActionName("Register")]
        public HttpResponseMessage Register(Register data)
        {
            var user = ParseAuthorizationHeader(this.Request);

            var result = XmlReadWrite.Register(user[0], user[1]);
            if (result)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "username taken");
            }
        }

        protected virtual string[] ParseAuthorizationHeader(HttpRequestMessage request)
        {
            string authHeader = null;
            var auth = request.Headers.Authorization;
            if (auth != null && auth.Scheme == BasicAuthResponseHeaderValue)
                authHeader = auth.Parameter;

            if (string.IsNullOrEmpty(authHeader))
                return null;


            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(':');
            if (tokens.Length < 2)
                return null;

            return new string[] { tokens[0], tokens[1] };
        }
    }
}
