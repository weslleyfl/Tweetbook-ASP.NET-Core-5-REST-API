using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tweetbook.Web.Data
{
   
    public class WebContext : IdentityDbContext<IdentityUser>
    {
        public WebContext(DbContextOptions<WebContext> options) : base(options) { }

        //public WebContext() : base() { }
        //public WebContext(IdentityConfiguration config) : base(config) { }

    }
}
