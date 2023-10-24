﻿using Microsoft.EntityFrameworkCore;
using Rocky.Models;

namespace Rocky.Data
{
    public class ApplicationDBcontext:DbContext
    {


        public ApplicationDBcontext(DbContextOptions<ApplicationDBcontext> options):base(options) {
        
        
        
        }

        public DbSet<Category> Category { get; set; }




    }
}