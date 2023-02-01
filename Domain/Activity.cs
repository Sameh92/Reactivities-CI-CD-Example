using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Activity
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public bool IsCancelled { get; set; }


        // to configure many to many relationship by convention
        // public ICollection<AppUser> Attendees1 { get; set; } = new List<AppUser>();
        public ICollection<ActivityAttendee> Attendees { get; set; } = new List<ActivityAttendee>();

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}