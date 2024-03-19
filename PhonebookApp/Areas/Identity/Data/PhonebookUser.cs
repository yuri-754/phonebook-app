using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PhonebookApp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the PhonebookUser class
public class PhonebookUser : IdentityUser
{
    public virtual ICollection<Contact> Contacts { get; set; }
}

