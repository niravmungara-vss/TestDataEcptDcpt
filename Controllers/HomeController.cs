using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestDataEcptDcpt.Attribute;
using TestDataEcptDcpt.Models;

namespace TestDataEcptDcpt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public HomeController()
        {
            Directory.CreateDirectory("Data");
        }

        [HttpGet, EncryptionDecryptionAttr]
        public async Task<IActionResult> Get()
        {
            List<UserModel> lstUsers = new List<UserModel>();
            var lstFiles = Directory.GetFiles("Data");
            foreach (var filePath in lstFiles)
            {
                var result = JsonConvert.DeserializeObject<UserModel>(System.IO.File.ReadAllText(filePath));
                lstUsers.Add(result);
            }
            return Ok(lstUsers);
        }

        [HttpPost, EncryptionDecryptionAttr]
        public async Task<IActionResult> Add([FromBody] UserModel user)
        {
            if (user != null)
            {
                System.IO.File.WriteAllText("Data/" + DateTime.Now.ToString("MMMDDYYYYHHmm") + ".json", JsonConvert.SerializeObject(user, Formatting.Indented));
            }
            return Ok();
        }
    }
}
