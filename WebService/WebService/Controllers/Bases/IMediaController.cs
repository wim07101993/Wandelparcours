﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers.Bases
{
    public interface IMediaController
    {
        Task<IActionResult> GetAsync(string id);
    }
}