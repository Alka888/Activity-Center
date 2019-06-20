using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using beltExam.Models;

namespace beltExam
{
	public class Activity
	{
		[Key]
		public int ActivityId { get; set; }
		public int UserId { get; set; }
		public string Coordinator { get; set; }

		[Required(ErrorMessage = "Please type correct name on this event")]
		[MinLength(2, ErrorMessage = "Title must be 2 characters or longer!")]
		public string Title { get; set; }

		[Required (ErrorMessage = "Please type what date will be this event")]
		[Display (Name = "Date: ")]
		public DateTime Date { get; set; }

		[Required(ErrorMessage = "Please select the time for event")]
		[Display (Name = "TimeType")]
		public string Timetype { get; set; }

		[Required(ErrorMessage = "How long will this event last?")]
		[Display (Name = "Duration: ")]
		public int Duration { get; set; }

		[Required(ErrorMessage = "Please type the description of the event")]
		[MinLength(10, ErrorMessage = "Description must be 10 characters or longer!")]
		public string Description { get; set; }
		
		public List<Reservation> Users { get; set; }
	}
}