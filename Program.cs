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
    do{
        Console.ForegroundColor = ConsoleColor.Green;

        // Ask which type of item to display
        Console.WriteLine("\nSelect an activity:\n1) Display all blogs\n2) Add Blog\n3) Create Post\n4) Display Posts\n5) Exit");

        // input selection
        choice = Console.ReadLine();
            logger.Info("User choice: {choice}\n", choice);
        
        switch(choice){
            case "1":       //Display all blogs
                // Display all Blogs from the database
                var db = new BloggingContext();
                var query = db.Blogs.OrderBy(b => b.Name);

                Console.WriteLine($"There are {query.Count()} blogs in the database:",query.Count());
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

                // check for no name entered
                if(name == ""){
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    logger.Error("Blog name cannot be null");
                    break;
                }

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

                // check if user blog selection is valid
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


                Console.Write("Enter title of post >>  ");
                newPost.Title = Console.ReadLine();
                
                // check for no title entered
                if(newPost.Title == ""){
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    logger.Error("Post Title cannot be null");
                    break;
                }
                
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
                Console.WriteLine("Select the blog's posts to display:\n0 - Posts from all Blogs");
                foreach (var item in query)
                {
                    Console.WriteLine(item.BlogId + " - Posts from the Blog " + item.Name);
                }

                var subChoice = Convert.ToInt32(Console.ReadLine());
                    logger.Info("User choice: {subChoice}\n", subChoice);

                // check if user selection is valid
                try
                {
                    if(subChoice < 0 || subChoice > query.Count()){
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
                
                // display posts from all blogs or display posts in one selected blog
                if(subChoice == 0){
                    var postsToDisplay = db.Posts;

                    Console.WriteLine($"\n{postsToDisplay.Count()} post(s) returned:");
                    foreach (var item in postsToDisplay)
                    {
                        Console.WriteLine("{0,-30}{1,-40}{2,-80}","Blog Name:  " + item.Blog.Name,"Post Title:  "+ item.Title,"Post Content:  " + item.Content);
                    }
                }else{
                    var postsToDisplay = db.Posts.Where(p => p.BlogId == subChoice);

                    Console.WriteLine($"\n{postsToDisplay.Count()} post(s) returned:");
                    foreach (var item in postsToDisplay)
                    {
                        Console.WriteLine("{0,-30}{1,-40}{2,-80}","Blog Name:  " + item.Blog.Name,"Post Title:  "+ item.Title,"Post Content:  " + item.Content);
                    }

                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            case "5":       //Exit first time through
                Console.WriteLine("Program will exit.\n");
                Console.ForegroundColor = ConsoleColor.Black;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Invalid input detected.  Program will exit.\n");
                Console.ForegroundColor = ConsoleColor.Black;
                break;
        }
    }while(choice != "5");

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
