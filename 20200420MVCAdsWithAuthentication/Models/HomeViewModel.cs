using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdsAuth.Data;

namespace _20200420MVCAdsWithAuthentication.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Ad> Ads {get; set;}
        public int CurrentUser { get; set; }
    }
}
