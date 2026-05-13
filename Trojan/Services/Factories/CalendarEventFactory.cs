using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Trojan.Core.Models;

namespace Trojan.Services.Factories
{
    internal class CalendarEventFactory
    {
        private static readonly string[] EventTitles = new[]
        {
            "Team Meeting",
            "Project Kickoff",
            "Code Review Session",
            "Client Presentation",
            "Sprint Planning",
            "Dentist Appointment",
            "Gym Session",
            "Lunch with Sarah",
            "Conference Call",
            "Product Demo",
            "Training Workshop",
            "Team Lunch",
            "Budget Review Meeting",
            "Performance Evaluation"
        };

        private static readonly string[] EventDescriptions = new[]
        {
            "Discuss Q4 roadmap and priorities for the team.",
            "Kick off the new mobile app project with stakeholders.",
            "Review pull requests and provide feedback on code quality.",
            "Present the new dashboard design to the client.",
            "Plan sprint tasks and estimate story points.",
            "Annual checkup and teeth cleaning.",
            "Cardio and strength training session.",
            "Catch up over lunch at the Italian restaurant.",
            "Weekly sync with remote team members.",
            "Live demo of the new features to stakeholders.",
            "Introduction to microservices architecture.",
            "Team bonding and networking event.",
            "Q3 financial review and budget allocation.",
            "Career development and goals discussion."
        };

        private static readonly string[] colors = new[] { "#A13599", "#FF6B6B", "#4ECDC4", "#45B7D1", "#FFA07A", "#98D8C8", "#F7DC6F", "#BB8FCE" };

        public static List<CalendarEvent> Generate(int count)
        {
            var faker = new Faker<CalendarEvent>("en_US")
                .RuleFor(e => e.Id, f => 0)
                .RuleFor(e => e.Title, f => f.PickRandom(EventTitles))
                .RuleFor(e => e.Description, f => f.PickRandom(EventDescriptions))
                .RuleFor(e => e.StartDateTime, f => f.Date.Future(1).ToUniversalTime())
                .RuleFor(e => e.EndDateTime, (f, e) => e.StartDateTime.AddHours(f.Random.Int(1, 8)))
                .RuleFor(e => e.Location, f => f.Address.City())
                .RuleFor(e => e.ColorHex, f => f.PickRandom(colors));

            return faker.Generate(count);
        }
    }
}
