using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CodeCamp2016Service.Controllers
{
    public class ItemsController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage ForSale()
        {
            var items = new List<CodeCamp2016.Models.Items>();

            items.Add(new CodeCamp2016.Models.Items("Great Value Tall Kitchen Bags, 13 gal, 100ct", 12.52));
            items.Add(new CodeCamp2016.Models.Items("Ultra Downy April Fresh with Silk Touch Liq", 8.94));
            items.Add(new CodeCamp2016.Models.Items("Windex Original Glass Cleaner Refill 67.6", 5.44));
            items.Add(new CodeCamp2016.Models.Items("Tide Simply Clean & Fresh HE Liquid Laundry", 9.22));
            items.Add(new CodeCamp2016.Models.Items("Kleenex Everyday Facial Tissues, 160 Tissue", 8.48));
            items.Add(new CodeCamp2016.Models.Items("Dawn Original Dishwashing Liquid, 56 fl oz", 3.74));
            items.Add(new CodeCamp2016.Models.Items("Great Value Sandwich Bags, 300 count", 4.84));
            items.Add(new CodeCamp2016.Models.Items("Clorox Disinfecting Wipes Value Pack, Scent", 5.00));
            items.Add(new CodeCamp2016.Models.Items("Goodyear Wrangler Sr - A Tire P275/ 60R20 / SLT", 129.68));
            items.Add(new CodeCamp2016.Models.Items("Goodyear Wrangler", 130.99));
            items.Add(new CodeCamp2016.Models.Items("Kumho SOLUS TA11 Tire 205 / 60R16 92T", 72.65));
            items.Add(new CodeCamp2016.Models.Items("Stanley 1000 - Amp Peak Jump Starter with Com", 69.98));
            items.Add(new CodeCamp2016.Models.Items("Hankook DynaPro ATm Tire P255", 125.73));
            items.Add(new CodeCamp2016.Models.Items("Hankook DynaPro", 125.73));
            items.Add(new CodeCamp2016.Models.Items("15 QT OIL DRAIN", 8.04));
            items.Add(new CodeCamp2016.Models.Items("Pilot Automotive Dash Cam", 35.99));

            return Request.CreateResponse(HttpStatusCode.OK, items);

        }
    }
}
