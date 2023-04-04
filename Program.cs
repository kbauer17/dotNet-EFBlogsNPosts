using NLog;
using System.Linq;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

string choice = "";
try
{
    Console.ForegroundColor = ConsoleColor.Green;

    // Ask which type of item to display
    Console.WriteLine("\nSelect an activity:\n1) Display all blogs\n2) Add Blog\n3) Create Post\n4) Display Posts");

    // input selection
    choice = Console.ReadLine();
        logger.Info("User choice: {choice}\n", choice);
    
    switch(choice){
        case "1":       //Display all blogs
            // Display all Blogs from the database
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine(item.Name);
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Black;        
            break;

        case "2":       //Add blog
            // Create and save a new Blog
            Console.Write("Enter a name for a new Blog: ");
            var name = Console.ReadLine();
            // create an instance of Blog class
            var blog = new Blog { Name = name };

            db = new BloggingContext();
            db.AddBlog(blog);
                logger.Info("Blog added - {name}", name);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Black; 
            break;

        case "3":       //Create post
            // Determine which blog in which to create the post
            db = new BloggingContext();
            query = db.Blogs.OrderBy(b => b.BlogId);
            Console.WriteLine("Select the blog in which you would like to post:");
            foreach (var item in query)
            {
                Console.WriteLine(item.BlogId + " - " + item.Name);
            }

            //create an instance of Post
            var newPost = new Post();

            // input selection
            newPost.BlogId = Convert.ToInt32(Console.ReadLine());
                logger.Info("User choice: {newPost.BlogId}\n", newPost.BlogId);

            // check if user selection is valid
            try
            {
                if(newPost.BlogId < 1 || newPost.BlogId > query.Count()){
                    throw new ArgumentOutOfRangeException($" Entry of {newPost.BlogId} is outside selection range");
                }
            }
            catch(ArgumentOutOfRangeException arg)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                    logger.Error(arg.Message);
                Console.WriteLine($"Error: {arg.Message}\nProgram will exit");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Black; 
                break;
            }


            Console.Write("Enter name of post >>  ");
            newPost.Title = Console.ReadLine();
            Console.Write("Enter content of post >>  ");
            newPost.Content = Console.ReadLine();
            
            db.AddPost(newPost);
                logger.Info("Post added - {title}", newPost.Title);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Black; 
            break;

        case "4":       //Display post
            // Determine which blog to list all posts
            db = new BloggingContext();
            query = db.Blogs.OrderBy(b => b.BlogId);
            Console.WriteLine("Select the blog to display all posts:");
            foreach (var item in query)
            {
                Console.WriteLine(item.BlogId + " - " + item.Name);
            }

            var subChoice = Convert.ToInt32(Console.ReadLine());
                logger.Info("User choice: {subChoice}\n", subChoice);

            // check if user selection is valid
            try
            {
                if(subChoice < 1 || subChoice > query.Count()){
                    throw new ArgumentOutOfRangeException($" Entry of {subChoice} is outside selection range");
                }
            }
            catch(ArgumentOutOfRangeException arg)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                    logger.Error(arg.Message);
                Console.WriteLine($"Error: {arg.Message}\nProgram will exit");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Black; 
                break;
            }
            
            var postsToDisplay = db.Posts.Where(p => p.BlogId == subChoice);

            Console.WriteLine($"\nThere are {postsToDisplay.Count()} Posts in this blog:");
            foreach (var item in postsToDisplay)
            {
                Console.WriteLine("Blog Name:  " + item.Blog.Name + "\tPost Title:  " + item.Title + "\t\tPost Content:  " + item.Content);

            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Black;
            break;
        default:
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Invalid input detected.  Program will exit.\n");
            Console.ForegroundColor = ConsoleColor.Black;
            break;
    }

}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine("Invalid input detected.  Program will exit.");
        logger.Error(ex.Message);
    Console.ForegroundColor = ConsoleColor.Black;
    Console.WriteLine();
}

logger.Info("Program ended");
