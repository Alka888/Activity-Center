using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using beltExam.Models;
using System.Collections.Generic;


namespace beltExam.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }

		[Required(ErrorMessage = "Please enter your first name")]
		[MinLength(2, ErrorMessage = "Name must be at least 2 letters")]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Please enter your last name")]
		[MinLength(2, ErrorMessage = "Name must be at least 2 letters")]
		public string LastName { get; set; }
		
		[Required(ErrorMessage = "Valid email address is required")]
		public string Email { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Password must be entered")]
		[MinLength(8, ErrorMessage = "Password must be 8 characters or longer!Contains at least 1 number, 1 letter and a special character")]
		public string Password { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public DateTime UpdateAt { get; set; } = DateTime.Now;
		// Will not be mapped to your users table!
		[NotMapped]
		[Compare("Password")]
		[DataType(DataType.Password)]
		public string Confirm { get; set; }
		public List<Reservation> Activities {get; set;}
		
	}
}