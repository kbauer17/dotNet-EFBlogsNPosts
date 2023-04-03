﻿using NLog;
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
    Console.WriteLine("Select an activity:\n1) Display all blogs\n2) Add Blog\n3) Create Post\n4) Display Posts");

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
        Console.ForegroundColor = ConsoleColor.Black;        
        break;

        case "2":       //Add blog
        // Create and save a new Blog
        Console.Write("Enter a name for a new Blog: ");
        var name = Console.ReadLine();

        var blog = new Blog { Name = name };

        db = new BloggingContext();
        db.AddBlog(blog);
        logger.Info("Blog added - {name}", name);
        Console.ForegroundColor = ConsoleColor.Black; 
        break;

        case "3":       //Create post
        // Determine which blog in which to create the post
        db = new BloggingContext();
        query = db.Blogs.OrderBy(b => b.BlogId);
        Console.WriteLine("Select the blog in which you would like to post:");
        foreach (var item in query)
        {
            Console.Write(item.BlogId);
            Console.WriteLine(" - " + item.Name);
        }

        //create an instance of Post
        var newPost = new Post();

        // input selection
        newPost.BlogId = Convert.ToInt32(Console.ReadLine());  //want this to be the BlogID
        logger.Info("User choice: {choice}\n", choice);

        Console.Write("Enter name of post>>  ");
        newPost.Title = Console.ReadLine();
        Console.WriteLine("Enter content of post>>  ");
        newPost.Content = Console.ReadLine();
        
        db.AddPost(newPost);
        logger.Info("Post added - {title}", newPost.Title);


        Console.ForegroundColor = ConsoleColor.Black; 
        break;

        case "4":       //Display post

        break;
    }




    // Create and save a new Blog
    /*Console.Write("Enter a name for a new Blog: ");
    name = Console.ReadLine();

    var blog = new Blog { Name = name };

    var db = new BloggingContext();
    db.AddBlog(blog);
    logger.Info("Blog added - {name}", name);

    // Display all Blogs from the database
    var query = db.Blogs.OrderBy(b => b.Name);

    Console.WriteLine("All blogs in the database:");
    foreach (var item in query)
    {
        Console.WriteLine(item.Name);
    }*/
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");
