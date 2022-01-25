//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Store.DataAccess.Repository.Interfaces;

//namespace Store.Controllers.API
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class VendorApiController : ControllerBase
//    {
//        private readonly IVendorRepository _vendorRepository;
//        public VendorApiController(IVendorRepository vendorRepository)
//        {
//            _vendorRepository = vendorRepository;
//        }

//        [HttpGet]
//        [Route("allVendors")]
//        public IActionResult AllVendors()
//        {
//            return Ok(_vendorRepository.GetAllVendors());

//        }

//        [HttpGet]
//        [Route("notApprovedVendors")]
//        public IActionResult NotApprovedVendors()
//        {
//            return Ok(_vendorRepository.GetNotApprovedVendors());
//        }

//        //[HttpGet]
//        //[Route("newVendors")]
//        //public IActionResult NewVendors()
//        //{
//        //    return Ok(_vendorRepository.GetAll());
//        //}

//        [HttpPost]
//        [Route("lock")]
//        public async Task<IActionResult> Lock([FromBody]string id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var vendor = await _vendorRepository.FindVendor(id);

//            if (vendor == null)
//            {
//                return NotFound();
//            }
//            vendor.User.LockoutEnd = DateTime.Now.AddYears(1000);


//            return Ok();
//        }

//        [HttpPost]
//        [Route("unlock")]
//        public async Task<IActionResult> UnLock(string id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var vendor = await _vendorRepository.FindVendor(id);

//            if (vendor == null)
//            {
//                return NotFound();
//            }
//            vendor.User.LockoutEnd = DateTime.Now;


//            return Ok();
//        }



//    }
//}