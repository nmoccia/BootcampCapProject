﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BootcampCapstone
{
    internal sealed class UserMetaData
    {
        public int userID;

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string username;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password;

        [Required]
        [Display(Name = "First Name")]
        public string firstName;

        [Required]
        [Display(Name = "Last Name")]
        public string lastName;

        [Required]
        [Display(Name = "Address 1")]
        public string address1;

        [Display(Name = "Address 2")]
        public string address2;

        [Required]
        [Display(Name = "City")]
        public string city;

        [Required]
        [Display(Name = "State")]
        public string state;

        [Required]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip Code")]
        public string zip;

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Home Phone")]
        public string phoneHome;

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Cell Phone")]
        public string phoneCell;

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Office Phone")]
        public string phoneOffice;

        [Required]
        [Display(Name = "Company Name")]
        public string companyName;

        [Display(Name = "Branch Location")]
        public string branchLocation;

        [Required]
        [Display(Name = "Food Preference")]
        public Nullable<int> foodID;

        [Display(Name = "Additional Information")]
        public string additionalInfo;
    }
    
}