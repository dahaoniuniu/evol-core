﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Evol.TMovie.Domain.QueryEntries;

namespace Evol.TMovie.Website.Controllers
{
    public class CinemaController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}