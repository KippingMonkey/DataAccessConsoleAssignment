using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using static Assignment1.Utils;

namespace Assignment1
{
    public class MoviesContext : DbContext
    {
        public DbSet<Movies> Movies { get; set; }
        public DbSet<Screenings> Screenings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(@"Data Source=LAPTOP-9gj2bhv1;Initial Catalog=DataAccessConsoleAssignment;Integrated Security=SSPI");
        }
    }
    public class Movies
    {
        public int ID { get; set; }
        [MaxLength(255), Required]
        public string Title { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName ="Date")]
        public DateTime ReleaseDate { get; set; }
        public List<Screenings> Screenings { get; set; }
    }
    public class Screenings
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }
        [Required]   
        public Movies Movies { get; set; } 

        public Int16 Seats { get; set; }
    }

    
    public class Program
    {
        private static MoviesContext moviesContext;
  
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
          

            using (moviesContext = new MoviesContext())
            {
                // this is done to fill the dictionaries (if the database has content)
                ListMovies();
                Console.Clear();
                ListScreenings();
                Console.Clear();

                bool running = true;
                while (running)
                {
                    int selected = ShowMenu("What do you want to do?", new[] {
                    "List Movies",
                    "Add Movie",
                    "Delete Movie",
                    "Load Movies from CSV File",
                    "List Screenings",
                    "Add Screening",
                    "Delete Screening",
                    "Exit"
                });
                    Console.Clear();

                    if (selected == 0) ListMovies();
                    else if (selected == 1) AddMovie();
                    else if (selected == 2) DeleteMovie();
                    else if (selected == 3) LoadMoviesFromCSVFile();
                    else if (selected == 4) ListScreenings();
                    else if (selected == 5) AddScreening();
                    else if (selected == 6) DeleteScreening();
                    else running = false;

                    Console.WriteLine();
                } 
            }
        }
     

        public static void ListMovies()
        {
            if (moviesContext.Movies.Count() != 0)
            {
                foreach (var movie in moviesContext.Movies.OrderBy(m => m.Title).AsNoTracking())
                {
                    Console.WriteLine($"- {movie.Title} ({movie.ReleaseDate:yyyy})");
                } 
            }
            else
            {
                Console.WriteLine("I am sorry. There are no movies to list at this time.");
            }
        }

        public static void AddMovie()
        {
            while (true)
            {
                string title = ReadString("Please enter the name of the movie: ");
                DateTime releaseDate = ReadDate("Thank you. Now please enter date of release: ");

                Movies movie = new Movies
                {
                    Title = title,
                    ReleaseDate = releaseDate
                };

                if (moviesContext.Movies.Where(m => m.Title == title).Any()
                    && moviesContext.Movies.Where(m => m.ReleaseDate == releaseDate).Any()) 
                {
                    Console.WriteLine("I am sorry, this movie already exists. Please try again.");
                    Console.WriteLine("Press Any Key To Continue");
                    Console.ReadKey();
                }
                else
                {
                    moviesContext.Add(movie);
                    moviesContext.SaveChanges();
                    break;
                }
            }

        }

        public static void DeleteMovie()
        {
            if (moviesContext.Movies.Count() == 0)
            {
                Console.WriteLine("I Am Sorry, There are no movies to delete.");
                return;
            }

            int choice = ShowMenu("Which Movie Would You Like To Delete?", MovieArrayForShowMenu() );

            moviesContext.Remove(moviesContext.Movies.OrderBy( m => m.Title).Skip(choice).First());
            moviesContext.SaveChanges();
        }
       
        public static string[] MovieArrayForShowMenu()
        {
            var movies = new List<string>();
            foreach (var movie in moviesContext.Movies.OrderBy(m => m.Title).AsNoTracking())
            {
                movies.Add($"{movie.Title} ({movie.ReleaseDate:yyyy})");
            }

            return movies.ToArray();
        }

        public static void LoadMoviesFromCSVFile()
        {
            Console.WriteLine("Please note this will delete all current movies and clear all screenings");
            int answer = ShowMenu("Do you still wish to continue?", new [] { "Yes", "No" });

            if (answer == 0)
            {
                moviesContext.Movies.RemoveRange(moviesContext.Movies); //empties movies table and by cascade also screenings

                string sampleMoviesPath = ReadString("Please enter path to the desired csv file: ");

                string[] linesCSV = File.ReadAllLines(@$"{sampleMoviesPath}").ToArray();

                foreach (string line in linesCSV)
                {
                    string[] values = line.Split(',').Select(v => v.Trim()).ToArray();

                    string title = values[0];
                    DateTime releaseDate = DateTime.Parse(values[1]);

                    Movies movie = new Movies
                    {
                        Title = title,
                        ReleaseDate = releaseDate
                    };
                    
                    moviesContext.Add(movie);
                }

                moviesContext.SaveChanges();
            }
            else { }
        }

        public static void ListScreenings()
        {
            if (moviesContext.Screenings.Count() != 0)
            { 
                foreach (var screening in moviesContext.Screenings.OrderBy(s => s.DateTime).Include(s => s.Movies).AsNoTracking())
                {
                    Console.WriteLine($"- {screening.DateTime}: {screening.Movies.Title} ({screening.Seats})");
                }
            }
            else
            {
                Console.WriteLine("I am sorry. There are no screenings to list at this time.");
            }
        }

        public static void AddScreening()
        {
            while (true)
            {
                WriteHeading("Add Screening");

                int choice = ShowMenu("Movie:", MovieArrayForShowMenu());

                DateTime selectedDate = ReadFutureDate("Day");

                string timestring = ReadString("Time (HH:MM): ");
                TimeSpan time = TimeSpan.Parse(timestring);

                selectedDate = selectedDate.Add(time);

                Int16 noOfSeats = Convert.ToInt16(ReadInt("Seats: "));

                var movie = moviesContext.Movies.OrderBy(m => m.Title).Skip(choice).First();

                Screenings screening = new Screenings
                {
                    DateTime = selectedDate,
                    Seats = noOfSeats,
                    Movies = movie
                };

                if (moviesContext.Screenings.Where(s => s.DateTime == selectedDate).Any()
                   && moviesContext.Screenings.Where(s => s.Movies == movie ).Any())  //Check if this screening already exists
                {
                    Console.WriteLine("I am sorry, this screening already exists. Please try again.");
                    Console.WriteLine("Press Any Key To Continue");
                    Console.ReadKey();
                }
                else
                {
                    moviesContext.Add(screening);
                    moviesContext.SaveChanges();

                    Console.Clear();
                    Console.WriteLine($"{movie.Title} on {selectedDate} ({noOfSeats} seats) has been added.");
                    break;
                } 
            }
        }

        public static void DeleteScreening()
        {
            if (moviesContext.Screenings.Count() == 0)
            {
                Console.WriteLine("I Am Sorry, There Are No Screenings To Delete,");
                return;
            }

            var screenings = new List<string>();

            foreach (var s in moviesContext.Screenings.OrderBy(s => s.DateTime).Include(s => s.Movies).AsNoTracking())
            {
                screenings.Add($"{s.DateTime}: {s.Movies.Title} ({s.Seats})");
            }

            int choice = ShowMenu("Which Screening Would You Like To Delete?", screenings.ToArray());

            var screening = moviesContext.Screenings.AsNoTracking().OrderBy(s => s.DateTime).Skip(choice).First();

            moviesContext.Remove(screening);
            moviesContext.SaveChanges();
        }
    }

    public static class Utils
    {
       
        public static string ReadString(string prompt)
        {
            Console.Write(prompt + " ");
            string input = Console.ReadLine();
            return input;
        }

        public static int ReadInt(string prompt)
        {
            Console.Write(prompt + " ");
            int input = int.Parse(Console.ReadLine());
            return input;
        }

        public static DateTime ReadDate(string prompt)
        {
            Console.WriteLine(prompt);
            int year = ReadInt("Year:");
            int month = ReadInt("Month:");
            int day = ReadInt("Day:");
            var date = new DateTime(year, month, day);
            return date;
        }

        public static DateTime ReadFutureDate(string prompt)
        {
            var dates = new[]
            {
                DateTime.Now.Date,
                DateTime.Now.AddDays(1).Date,
                DateTime.Now.AddDays(2).Date,
                DateTime.Now.AddDays(3).Date,
                DateTime.Now.AddDays(4).Date,
                DateTime.Now.AddDays(5).Date,
                DateTime.Now.AddDays(6).Date,
                DateTime.Now.AddDays(7).Date
            };
            var wordOptions = new[] { "Today", "Tomorrow" };
            var nameOptions = dates.Skip(2).Select(d => d.DayOfWeek.ToString());
            var options = wordOptions.Concat(nameOptions);
            int daysAhead = ShowMenu(prompt, options.ToArray());
            var selectedDate = dates[daysAhead];
            return selectedDate;
        }

        public static void WriteHeading(string text)
        {
            Console.WriteLine(text);
            string underline = new string('-', text.Length);
            Console.WriteLine(underline);
        }

        public static int ShowMenu(string prompt, string[] options)
        {
            if (options == null || options.Length == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty array of options.");
            }

            Console.WriteLine(prompt);

            int selected = 0;

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                // If this is not the first iteration, move the cursor to the first line of the menu.
                if (key != null)
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop = Console.CursorTop - options.Length;
                }

                // Print all the options, highlighting the selected one.
                for (int i = 0; i < options.Length; i++)
                {
                    var option = options[i];
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine("- " + option);
                    Console.ResetColor();
                }

                // Read another key and adjust the selected value before looping to repeat all of this.
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Length - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }
            }

            // Reset the cursor and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }
}