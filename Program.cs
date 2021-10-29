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
    }
    public class Screenings
    {
        public int ID { get; set; }
        public DateTime DateTime { get; set; }

        public int MovieID { get; set; }
        [ForeignKey("MovieID")]
        public Movies MovieClass { get; set; }

        public Int16 Seats { get; set; }
    }

    
    public class Program
    {
        private static MoviesContext MoviesContext;
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            // Method not used as we are only turning in the cs-file. Still wanted to include it.
           //MoveFileSampleMoviesToTemp();

            using (MoviesContext = new MoviesContext())
            {
                //if (MoviesContext.Database.EnsureCreated()) //Checks if database exists. If not it creates it. NOTE. Does not use migrations
                //{
                //    Console.WriteLine("Database exists");
                //}
                //else if (!MoviesContext.Database.EnsureCreated())
                //{
                //    Console.WriteLine("No such database exists. Please hold while I created it.");
                //};

                bool running = true;
                while (running)
                {
                    int selected = Utils.ShowMenu("What do you want to do?", new[] {
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
        /// <summary>
        /// Method for moving "SampleMovies" to local harddrive when sending complete solution
        /// </summary>
        private static void MoveFileSampleMoviesToTemp()
        {
            string projectPath = @"C:\Users\nelsc\source\repos\DataAccessConsoleAssignment\SampleMovies.csv";
            string tempPath = @"C:\Windows\Temp\SampleMovies.csv";
            try
            {
                if (!File.Exists(projectPath)) //checks if the file to move is in the project
                {
                    Console.WriteLine("I am sorry, there has been an error. The file SampleMovies does not exist in your project.");
                }
                else
                { 
                    // Ensure that the target does not exist. If it does, delete it.
                    if (File.Exists(tempPath))
                    {
                        File.Delete(tempPath);
                    }
                    // Move the file.
                    File.Move(projectPath, tempPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
            }
        }

        public static string[] MovieListToArry()
        {
            var movies = new string[MoviesContext.Movies.Count()];
            int counter = 0;

            foreach (var movie in MoviesContext.Movies.AsNoTracking())
            {
                movies[counter] = $"{movie.Title} ({movie.ReleaseDate:yyyy})";
                counter++;
            }
            return movies;
        }

        public static void ListMovies()
        {
            if (MoviesContext.Movies.Count() != 0)
            {
                foreach (var movie in MoviesContext.Movies.AsNoTracking())
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
            string title = ReadString("Please enter the name of the movie: ");
            DateTime releaseDate = ReadDate("Thank you. Now please enter date of release: ");

            Movies movie = new Movies
            {
                Title = title,
                ReleaseDate = releaseDate
            };

            MoviesContext.Add(movie);
            MoviesContext.SaveChanges();

        }

        public static void DeleteMovie()
        {
            string[] movies = MovieListToArry();
            // list all the movies with showmenu
            int selected = ShowMenu("Which movie would you like to delete?", movies);
            //find movie selected in database
            var movieToDelete = MoviesContext.Movies.Where(m => m.ID == selected + 1).First();
            //delete movie from database
            MoviesContext.Movies.Remove(movieToDelete);
            MoviesContext.SaveChanges();
        }

        public static void LoadMoviesFromCSVFile()
        {
            //Not possible due to foreignkey. Need to remove constraints, then truncate, then recreate constraints (i.e. FK)
            //MoviesContext.Database.ExecuteSqlRaw("TRUNCATE TABLE Screenings");
            //MoviesContext.Database.ExecuteSqlRaw("TRUNCATE TABLE Movies"); 


            string[] linesCSV = File.ReadAllLines(@"C:\Users\nelsc\source\repos\DataAccessConsoleAssignment\SampleMovies.csv").ToArray();
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

                MoviesContext.Add(movie);
                MoviesContext.SaveChanges();
            }
        }

        public static void ListScreenings()
        {
            if (MoviesContext.Screenings.Count() != 0)
            {
                var join = MoviesContext.Movies
                    .Join(
                    MoviesContext.Screenings,
                    movie => movie.ID,
                    screening => screening.MovieClass.ID,
                    (movie, screening) => new
                    {
                        ScreeningDate = screening.DateTime,
                        MovieTitle = movie.Title,
                        Seats = screening.Seats
                    }
                    );
       
                foreach (var screening in join)
                {
                    //var movieScreening = MoviesContext.Movies.Where(m => m.ID == screening.MovieID + 1).First();
                    Console.WriteLine($"- {screening.ScreeningDate:g}: {screening.MovieTitle} ({screening.Seats})");
                }
            }
            else
            {
                Console.WriteLine("I am sorry. There are no screenings to list at this time.");
            }
        }

        public static void AddScreening()
        {
            string[] movies = MovieListToArry();
            int selected = ShowMenu("Add Screening", movies);
            //find movie selected in database
            //var movieSelected = MoviesContext.Movies.Where(m => m.ID == selected + 1).First();

            //choose day
            DateTime selectedDate = ReadFutureDate("Day");
            //choose time and parse to TimeSpan
            string timestring = ReadString("Time (HH:MM): ");
            TimeSpan time = TimeSpan.Parse(timestring);
            // add chosen time to chosen day
            selectedDate.Add(time);

            Int16 noOfSeats = Convert.ToInt16(ReadInt("Seats: "));

            Screenings screening = new Screenings
            {
                DateTime = selectedDate,
                Seats = noOfSeats,
                MovieID = selected
            };

            MoviesContext.Add(screening);
            MoviesContext.SaveChanges();

        }

        public static void DeleteScreening()
        {
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