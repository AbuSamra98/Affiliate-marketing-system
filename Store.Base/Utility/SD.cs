using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Base.Utility
{
    public class SD
    {
        public const string DefaultProductImage = "default_image.png";
        public const string ImageFolder = @"images\CampaignImage";          //add (@) for \

        //for roles
        public const string SuperAdminEndUser = "SuperAdmin";
        public const string AdminEndUser = "Admin";
        public const string VendorEndUser = "Vendor";
        public const string MarketerEndUser = "Marketer";

        //for claims
        public const string AdminControlClaim = "Admin control";
        public const string VendorControlClaim = "Vendor control";
        public const string MarketerControlClaim = "Marketer control";
        public const string CampainControlClaim = "Campain control";
    }
}
