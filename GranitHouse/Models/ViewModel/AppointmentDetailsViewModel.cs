﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GranitHouse.Models.ViewModel
{
    public class AppointmentDetailsViewModel
    {
        public Appointments Appointments { get; set; }
        public List<ApplicationUser> SalesPerson { get; set; }

        public List<Products> Products { get; set; }
    }
}
