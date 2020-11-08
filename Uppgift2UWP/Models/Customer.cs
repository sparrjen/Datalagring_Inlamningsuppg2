﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uppgift2UWP.Models
{
    public class Customer
    {        
        public long SSNo { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long PhoneNo { get; set; } 
        public string Email { get; set; }
        public string DisplayName => $"{FirstName} {LastName}"; 

        public Customer(long sSNo, string firstName, string lastName, long phoneNo, string email)
        {
            SSNo = sSNo;
            FirstName = firstName;
            LastName = lastName;
            PhoneNo = phoneNo;
            Email = email;
            
        }

        public Customer()
        {

        }

    }

}
//Diskutera m Maria, sätta private set på ngn?
