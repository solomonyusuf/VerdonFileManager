using System;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VerdonFileManager.Services
{
    [ApiController]
    [SecurityPermission(SecurityAction.Demand, ControlThread = true)]
    public class UploadService : ControllerBase
    {
        [Route("blocked")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> LocalUpload(IBrowserFile seed)
        {
            try
            {
                var file = seed.OpenReadStream(50000000);
                var folderName = Path.Combine("wwwroot", "StaticFiles");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileName = seed.Name;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [Route("verdon.filemanager.api/{appToken}")]
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> ExternalUpload([FromRoute]string appToken)
        {
            try
            {
               

                var file = Request.Form.Files[0];
                var folderName = Path.Combine("wwwroot", "StaticFiles", appToken);
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if(!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                if (file.Length > 0)
                {
                    var fileName = file.Name;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
                    

                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }



    }

 }


       