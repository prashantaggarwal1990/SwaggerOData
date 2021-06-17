using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SwaggerOData.Models
{
	public class Employee : BaseEntity
	{
	    public Employee()
	    {
	    }

	    public Employee(string firstName, string lastName, DateTime birthDate)
		{
			FirstName = firstName;
			LastName = lastName;
			BirthDate = birthDate;
		}

		[Required]
	    [MaxLength(128)]
	    [Display(Name = "FirstName", Description="Employee first name")]
		public string FirstName { get; set; } 

	    [Required]
	    [MaxLength(128)]
	    [Display(Name = "LastName", Description="Employee last name")]
	    public string LastName { get; set; }
	    
	    [Required]
	    [Display(Name = "BirthDate", Description="Employee Birth date")]
		public DateTime BirthDate { get; set; }

		public virtual ICollection<Book> Books { get; set; }

		// dynamic
		public IDictionary<string, object> Properties { get; set; }
	}

	public class Book : BaseEntity
	{
		[Required]
		public string Name { get; set; }

		public Guid AuthorId { get; set; }

		[JsonIgnore]
		public Employee Author { get; set; }
	}
}
