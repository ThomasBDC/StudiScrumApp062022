using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SrumApp.Repository.UserRepository
{
    public class UserRepository : BaseRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration): base(configuration)
        {
            _configuration = configuration;
        }

        public void toto()
        {
            this.OpenConnexion();
        }
    }
}
