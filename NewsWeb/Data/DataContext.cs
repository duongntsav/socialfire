﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NewsWeb.Models;

namespace NewsWeb
{
    // [DbConfigurationType(typeof(MySqlEFConfiguration))]

    public class DataContext : DbContext
    {
        // Codelist
        public DbSet<CodeList> CodeListSet { get; set; }
        public DbSet<Article> ArticleSet { get; set; }
        public DbSet<Site> SiteSet { get; set; }
        public DbSet<Subject> SubjectSet { get; set; }

        // private const string connectionString = @"Data Source=10.62.142.50;Initial Catalog=Socialfire;User Id=sa;Password=HiPT@123456;";
        private const string connectionString = "Data Source=10.160.0.18;Initial Catalog=Socialfire;User Id=appsql;Password=Abc@123456;";
        protected readonly IConfiguration Configuration;

        public DataContext() : base()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        /**
        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public DataContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }
         */



        /** 
         * Use for Mariadb
        public DataContext() : base()
        {
        }

        public DataContext(DbConnection existingConnection, bool contextOwnsConnection)
          : base(existingConnection, contextOwnsConnection)
        {

        }
        */

    }
}
