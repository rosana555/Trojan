using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Trojan.Core.Models;

namespace Trojan.Services.Factories
{
    internal class JokeFactory
    {
        private static readonly string[] Jokes = new[]
        {
            "Why do programmers prefer dark mode? Because light attracts bugs!",
            "Why did the developer go broke? Because he used up all his cache!",
            "How many programmers does it take to change a light bulb? None, that's a hardware problem.",
            "Why do Java developers wear glasses? Because they don't C#.",
            "Why did the programmer quit his job? Because he didn't get arrays.",
            "What's a programmer's favorite hangout place? Foo bar.",
            "Why do programmers always get Christmas and Halloween mixed up? Because DEC 25 = OCT 31.",
            "What did the Java developer say? 'Let me pair with you.'",
            "Why is Python the best programming language? Because it's a snake that you can actually code with!",
            "How do you comfort a JavaScript bug? You console it.",
            "Why did the database admin break up with their partner? There was no relation.",
            "What do you call a programmer from Finland? A nerdic programmer."
        };

        public static List<Joke> Generate(int count)
        {
            var faker = new Faker<Joke>("en_US")
                .RuleFor(j => j.Id, f => 0)
                .RuleFor(j => j.Content, f => f.PickRandom(Jokes))
                .RuleFor(j => j.Category, f => f.PickRandom<JokeCategory>())
                .RuleFor(j => j.Rating, f => f.Random.Int(1, 5));

            return faker.Generate(count);
        }
    }
}
