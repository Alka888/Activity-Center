using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using beltExam.Models;
namespace beltExam
{
	public class Reservation
	{
		[Key]
		public int ReservationId { get; set; }
		public int UserId {get; set;}
		public User User {get; set;}

		public int ActivityId {get; set;}
		public Activity Activity {get; set;}
	}
}