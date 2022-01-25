using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Base.Models.ViewModels
{
    public class UpdateVendorProfileViewModel
    {
        public UpdateVendorProfileRequest UpdateVendorProfileRequest { get; set; }
        public IEnumerable<Country> Countries { get; set; }
    }
}
