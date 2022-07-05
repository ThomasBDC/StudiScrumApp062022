using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace StudiScrumApp062022.Utils
{
    public static class ControllerExtension
    {
        public static int GetIdUserConnecte(this Controller controller)
        {
            var id= controller.HttpContext.User.Claims.FirstOrDefault
                (claim => claim.Type == ClaimTypes.NameIdentifier).Value;

            return Convert.ToInt16(id);
        }
    }
}
