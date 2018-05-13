using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using IniParser;
using IniParser.Model;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;



namespace progLab2
{
    public class TaskRunner
    {
        public Menu MainMenu;

        private const string dateFormat = "dd-mm-yyyy";
        private Regex isbnRegex = new Regex(@"(?=[-0-9xX ]{13}$)(?:[0-9]+[- ]){3}[0-9]*[xX0-9]$");

        [DataMember]
        private List<Books> books = new List<Books>();

        public TaskRunner()
        {
            MainMenu = new Menu("Book catalog");

            MainMenu.Items.AddRange(new List<MenuItem>
            {
                new MenuItem()
                {
                    Label = "Add book",
                    Key = '1',
                    Function = AddBook
                },
                new MenuItem()
                {
                    Label = "Find a book by ISBN",
                    Key = '2',
                    Function = GetBookByISBN
                },
                new MenuItem()
                {
                    Label = "Find a book by keywords",
                    Key = '3',
                    Function = GetBooksByKeywords
                },

                new MenuItem()
                {
                    Label = "Library",
                    Key = '4',
                    Function = ListBooks
                },
                new MenuItem()
                {
                    Label = "SaveFromINILibrary",
                    Key = '5',
                    Function = SaveLibFromINI
                },
                new MenuItem()
                {
                    Label = "WriteToINI",
                    Key = '6',
                    Function = WriteBookToINI
                },
                new MenuItem()
                {
                    Label = "SaveFromJSONLibrary",
                    Key = '7',
                    Function = ReadLibraryFromJSON
                },


                new MenuItem()
                {
                    Label = "WriteToJSON",
                    Key = '8',
                    Function = WriteBookToJSON
                },
                new MenuItem()
                {
                    Label = "Quit",
                    Key = '0',
                    Function = () => false
                },

            });
        }

        public void Run()
        {
            MainMenu.Display();
        }

        private bool GetBooksByKeywords()
        {
            string input;

            do
            {
                Console.Write("Enter keywords separated by space: ");
                input = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(input));

            var keywords = input.Split(' ');
            var search = new List<(Books book, int amountOfFoundWords)>();
            var annotationSearch = new List<(Books book, IEnumerable<string> wordsFoundInAnnotation)>();

            foreach (var book in books)
            {
                var words = new List<string>((book.Title + ' ' + book.Author + ' ' + book.PublicationDate + ' ' + book.ISBN).Split(' '));
                var annotationWords = new List<string>(book.Annotation.Split(' '));

                var found = words.FindAll(s => keywords.Contains(s));
                var foundInAnnotation = annotationWords.FindAll(s => keywords.Contains(s));

                search.Add((book, found.Count + foundInAnnotation.Count));
                annotationSearch.Add((book, foundInAnnotation.Distinct()));
            }


            foreach (var t in search.OrderByDescending(x => x.amountOfFoundWords))
            {
                (var book, var amountOfFoundWords) = t;
                if (amountOfFoundWords > 0)
                {
                    Console.WriteLine(book.ToString(true));
                    Console.WriteLine($"Words amount: {amountOfFoundWords}");
                    Console.WriteLine("Words in annotation: ");

                    var words = annotationSearch.First(x => x.book == book).wordsFoundInAnnotation;

                    foreach (var word in words)
                    {
                        Console.WriteLine($"\t+ {word}");
                    }
                }
            }
            return true;
        }

        private bool GetBookByISBN()
        {
            if (books.Count == 0)
            {
                Console.WriteLine("There's no books in the library.");
                return true;
            }

            string input;

            do
            {
                Console.Write("Enter book ISBN: ");
                input = Console.ReadLine().Trim();
            } while (!isbnRegex.IsMatch(input));

            var book = books.Find(b => string.Compare(b.ISBN, input, true) == 0);

            if (book is null)
            {
                Console.WriteLine("No books found.");
            }
            else
            {
                Console.WriteLine($"Book info:\n{book}");
            }

//            var parser = new FileIniDataParser();
//            IniData data = parser.ReadFile("Library.ini");
//
//            string findAuthorByIBSN = data[input]["Author"];
//            string findTitleByIBSN = data[input]["Title"];
//            string findPublicationDateByIBSN = data[input]["Author"];
//            string findAnnotationByIBSN = data[input]["Title"];
//
//            if (findAuthorByIBSN is null)
//            {
//                Console.WriteLine("No books found.");
//            }
//            else
//            {
//                Console.WriteLine($"Author = {findAuthorByIBSN}");
//                Console.WriteLine($"Title = {findTitleByIBSN}");
//                Console.WriteLine($"PublicationDate = {findPublicationDateByIBSN}");
//                Console.WriteLine($"Annotation = {findAnnotationByIBSN}");
//            }



            return true;
        }

//        private class Boo
//        {
//            public string Title { get; set; }
//            public string Author { get; set; }
//            public string ISBN { get; set; }
//            public string PublicationDate { get; set; }
//            public string Annotation { get; set; }
//        }

        
        private bool WriteBookToJSON()
        {
//            foreach (var book in books)
//            {
//                var json = JsonConvert.SerializeObject(book, Formatting.Indented);
//                Console.WriteLine(json);
//
//                // serialize JSON to a string and then write string to a file
//                File.AppendAllText(@"movies.json", json);
//            }
//            foreach (var book in books)
//            {
//                var json = new JavaScriptSerializer().Serialize(book);
//                File.AppendAllText(@"movies.json", json);
//            }
            Books[] bookArray = books.ToArray();
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Books[]));
            if (bookArray.Length == 0)
                Console.WriteLine("Library is empty");
            else
            {
                using (FileStream fs = new FileStream("Library.json", FileMode.OpenOrCreate))
                {

                    jsonFormatter.WriteObject(fs, bookArray);
                }
            }

            return true;

        }

        private bool ReadLibraryFromJSON()
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Books[]));
            using (FileStream fs = new FileStream("Library.json", FileMode.OpenOrCreate))
            {
                if (fs.Length == 0)
                {
                    Console.WriteLine("JSON is empty");
                }
                else
                {


                    Books[] newBooks = (Books[]) jsonFormatter.ReadObject(fs);
                    List<Books> lst = newBooks.OfType<Books>().ToList();
                    Console.WriteLine("Done reading from json");
                    foreach (var b in lst)
                    {
                        var book = new Books();

                        book.Author = b.Author;
                        book.Title = b.Title;
                        book.Annotation = b.Annotation;
                        book.ISBN = b.ISBN;
                        book.PublicationDate = b.PublicationDate;
                        books.Add(book);
                    }
                }
            }

            return true;
        }



        private bool SaveLibFromINI()
        {
            var parser = new IniParser.FileIniDataParser();
            IniData data = parser.ReadFile("Lib2.ini");

            foreach (SectionData section in data.Sections)
            {
                var book = new Books();
                book.ISBN = section.SectionName;
                book.Title = data[section.SectionName]["Title"];
                book.Author = data[section.SectionName]["Author"];
                book.PublicationDate = data[section.SectionName]["PublicationDate"];
                book.Annotation = data[section.SectionName]["Annotation"];
                books.Add(book);
            }
            return true;
        }


        private bool WriteBookToINI()
        {
            var parser = new IniParser.FileIniDataParser();
            IniData data = parser.ReadFile("Library.ini");

            foreach (var book in books)
            {
                //Add a new selection and some keys
                data.Sections.AddSection(book.ISBN);
                data[book.ISBN].AddKey("Author", book.Author);
                data[book.ISBN].AddKey("Title", book.Title);
                //data[book.ISBN].AddKey("PublicationDate", book.PublicationDate.ToString("d"));
                data[book.ISBN].AddKey("PublicationDate", book.PublicationDate);
                data[book.ISBN].AddKey("Annotation", book.Annotation);

                parser.WriteFile("Library.ini", data);
            }

            return true;
        }


        private bool ListBooks()
        {

            foreach (var book in books)
                Console.WriteLine($"{book}");

            return true;
        }

        private bool AddBook()
        {
            var book = new Books();
            string input;

            do
            {
                Console.Write("Input title: ");
                input = Console.ReadLine().Trim();
            } while (string.IsNullOrWhiteSpace(input));

            book.Title = input;

            do
            {
                Console.Write("Input author: ");
                input = Console.ReadLine().Trim();
            } while (string.IsNullOrWhiteSpace(input));

            book.Author = input;

            do
            {
                Console.Write("Input annotation: ");
                input = Console.ReadLine().Trim();
            } while (string.IsNullOrWhiteSpace(input));

            book.Annotation = input;

            do
            {
                Console.Write("Input ISBN(1-333-55555-1): ");
                input = Console.ReadLine().Trim();
            } while (!isbnRegex.IsMatch(input));

            book.ISBN = input;

            if (books.Find(b => string.Compare(b.ISBN, book.ISBN, true) == 0) != null)
            {
                Console.WriteLine("There's a book with the same ISBN in library.");
                return true;
            }

            DateTime date;

            do
            {
                Console.Write("Input publication date (mm-dd-yyyy): ");
                input = Console.ReadLine().Trim();
            } while (!DateTime.TryParse(input, out date));

            book.PublicationDate = String.Format("{0:d}", date);
            books.Add(book);

            return true;
        }
    }
}
